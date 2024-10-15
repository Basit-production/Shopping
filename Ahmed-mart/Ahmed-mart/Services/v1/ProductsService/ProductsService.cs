using Ahmed_mart.Dtos.v1.CategoryDtos;
using Ahmed_mart.Dtos.v1.FileDtos;
using Ahmed_mart.Dtos.v1.PrefixesDto;
using Ahmed_mart.Dtos.v1.ProductAttributesDto;
using Ahmed_mart.Dtos.v1.ProductDto;
using Ahmed_mart.Dtos.v1.ProductImagesDto;
using Ahmed_mart.Dtos.v1.ProductOptionDetailsDto;
using Ahmed_mart.Dtos.v1.ProductOptionsDto;
using Ahmed_mart.Dtos.v1.ProductsDto;
using Ahmed_mart.Dtos.v1.RelatedProductsDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Ahmed_mart.Services.v1.FileService;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System.Linq;
using System.Security.Claims;

namespace Ahmed_mart.Services.v1.ProductsService
{
    public class ProductsService :BaseService, IProductsService
    {
        protected override string CacheKey => "productsCacheKey";
        private readonly IFileService _fileService;
        public ProductsService(IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IFileService fileService):
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {
            _fileService = fileService;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
           .FindFirstValue(ClaimTypes.NameIdentifier)!);
        string? GetUserRole() => _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role); //return the role of current user
        public async Task <ServiceResponse<IEnumerable<GetProductsDto>>> GetProducts()
        {
            var serviceResponse=new ServiceResponse<IEnumerable<GetProductsDto>>();
            try
            {
                if(_memoryCache.TryGetValue(CacheKey, out IEnumerable<GetProductsDto>? data))
                {
                    serviceResponse.Data = data;
                }
                else
                {
                    var product = _unitOfWork.GetRepository<Products>();
                    var result = await product.SearchAsync(x => !x.IsDeleted,
                    x=>x.ProductAttributes,
                    x=>x.ProductImages,
                    x=>x.ProductOptions,
                    x=>x.RelatedProducts,
                    x => x.StockStatus
                    );
                    data=_mapper.Map<IEnumerable<GetProductsDto>>(result.OrderByDescending(x=>x.CreatedAt));
                    serviceResponse.Data = data;
                    _memoryCache.Set(CacheKey, data, GetCacheEntryOptions());
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetProducts));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetProductsDto>> GetProduct(int Id,int StoreId)
        {
            var serviceResponse = new ServiceResponse<GetProductsDto>();
            try
            {
                var _productsRepo = _unitOfWork.GetRepository<Products>();
               // var result = await product.GetByIdAsync(Id);
                var resultProducts = await _productsRepo.Search(x => x.ID == Id && x.StoreID == StoreId,
                    x => x.ProductAttributes,
                    x => x.ProductImages,
                    x => x.ProductOptions,
                    x => x.RelatedProducts,
                    x => x.StockStatus);
                var result = resultProducts.FirstOrDefault();
                var data = _mapper.Map<GetProductsDto>(result);
                serviceResponse.Data = data;
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetProduct));
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<GetProductsDto>> AddProduct(AddProductsDto addProductsDto)
        {
            var serviceResponse = new ServiceResponse<GetProductsDto>();
            try
            {
                bool TransactionNumberExist = false;
                ICollection<AddProductImagesDto> productImagesList = new List<AddProductImagesDto>();
                ICollection<ProductImages> EmptyImagesList = new List<ProductImages>();
                productImagesList = addProductsDto.ProductImages;
                var _productsRepo = _unitOfWork.GetRepository<Products>();
                var products = await _productsRepo.Search(x => x.StoreID == addProductsDto.StoreID);
                foreach (var p in products)
                {
                    if (p.TransactionNumber == addProductsDto.TransactionNumber)
                    {
                        TransactionNumberExist = true;
                    }
                }
                if (TransactionNumberExist == true)
                {
                    var _prefixes= _unitOfWork.GetRepository<Prefixes>();
                    var Prefix = await _prefixes.Search(x => x.TransactionType == 2 && x.StoreID == addProductsDto.StoreID);
                    string GetNextTransactionNumber = GenerateNextTransactionNumber(Prefix);
                    addProductsDto.TransactionNumber = GetNextTransactionNumber;
                }
                addProductsDto.Description = addProductsDto.Description == null ? "" : addProductsDto.Description;
                var data = _mapper.Map<Products>(addProductsDto);
                data.ProductImages = EmptyImagesList;
                data.CreatedBy = 1;//GetUserId();
                data.CreatedAt = DateTime.Now;
                await _productsRepo.AddAsync(data);
                await _unitOfWork.SaveChangesAsync();

                // update prefix
                var _prefixesRepo = _unitOfWork.GetRepository<Prefixes>();
                var UpdatePrefix = await _prefixesRepo.Search(x => x.TransactionType == 2 && x.StoreID == addProductsDto.StoreID);
                var TransResult = UpdatePrefix.FirstOrDefault();
                if (TransResult.CurrentNumber > 0)
                {
                    TransResult.CurrentNumber += 1;
                    TransResult.ModifiedBy = 1;// GetUserId();
                    TransResult.ModifiedAt = DateTime.Now;
                    await _prefixesRepo.UpdateAsync(TransResult);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    TransResult.CurrentNumber = TransResult.StartNumber;
                    TransResult.ModifiedBy = 1;// GetUserId();
                    TransResult.ModifiedAt = DateTime.Now;
                   await _prefixesRepo.UpdateAsync(TransResult);
                    await _unitOfWork.SaveChangesAsync();
                }

                // upload document
                if (addProductsDto.File != null && addProductsDto.File.Length > 0)
                {
                    var objUploadFileDto = new UploadFileDto()
                    {
                        Directory = $"Products/ProductImage/{data.ID}",
                        File = addProductsDto.File
                    };
                    var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                    if (dataFileService.Success)
                    {
                        data.Path = dataFileService.Data.Path;
                        await _productsRepo.UpdateAsync(data);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                if (productImagesList.Count() > 0)
                {
                    foreach (var img in addProductsDto.ProductImages)
                    {
                        var images = new AddProductImagesDto();
                        var imageData = _mapper.Map<ProductImages>(images);
                        var objUploadFileDto = new UploadFileDto()
                        {
                            Directory = $"Products/ProductImage/{data.ID}",
                            File = img.File
                        };
                        var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                        if (dataFileService.Success)
                        {
                            var _productImagesRepo = _unitOfWork.GetRepository<ProductImages>();
                            imageData.ProductsID = data.ID;
                            imageData.StoreID = img.StoreID;
                            imageData.Status = img.Status;
                            imageData.Path = dataFileService.Data.Path;
                            imageData.CreatedBy = GetUserId();
                            imageData.CreatedAt = DateTime.Now;
                            await _productImagesRepo.AddAsync(imageData);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }

                // check pricelist PriceList
                var _priceListRepo = _unitOfWork.GetRepository<PriceList>();
                var priceList = await _priceListRepo.Search(x => x.StoreID == addProductsDto.StoreID);
                if (!priceList.Any())
                {
                    // add price list
                    var addPriceList = new PriceList()
                    {
                        StoreID = addProductsDto.StoreID,
                        Code = "PL001",
                        Name = "Default Price List",
                        Status = true,
                        CreatedBy = 1,//GetUserId(),
                        CreatedAt = DateTime.Now
                    };
                    await _priceListRepo.AddAsync(addPriceList);
                    await _unitOfWork.SaveChangesAsync();

                    // add pricelist details
                    var _priceListDetailsRepo = _unitOfWork.GetRepository<PriceListDetails>();

                    var addPriceListDetails = new PriceListDetails()
                    {
                        PriceListID = addPriceList.ID,
                        ProductID = data.ID,
                        SalePrice = addProductsDto.SellingPrice,
                        PurchasePrice = addProductsDto.CostPrice,
                        Status = true,
                        CreatedBy = 1,//GetUserId(),
                        CreatedAt = DateTime.Now
                    };
                    await _priceListDetailsRepo.AddAsync(addPriceListDetails);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    // update price list
                    var PriceListObj = priceList.FirstOrDefault();
                    PriceListObj.ModifiedBy = 1;// GetUserId();
                    PriceListObj.ModifiedAt = DateTime.Now;
                   await _priceListRepo.UpdateAsync(PriceListObj);
                    await _unitOfWork.SaveChangesAsync();

                    // add pricelist details
                    var _priceListDetailsRepo = _unitOfWork.GetRepository<PriceListDetails>();
                    var addPriceListDetails = new PriceListDetails()
                    {
                        PriceListID = PriceListObj.ID,
                        ProductID = data.ID,
                        SalePrice = addProductsDto.SellingPrice,
                        PurchasePrice = addProductsDto.CostPrice,
                        Status = true,
                        CreatedBy =1,// GetUserId(),
                        CreatedAt = DateTime.Now
                    };
                    await _priceListDetailsRepo.AddAsync(addPriceListDetails);
                    await _unitOfWork.SaveChangesAsync();
                }
                await _unitOfWork.CommitAsync();
                serviceResponse.Data = _mapper.Map<GetProductsDto>(data);
                serviceResponse.Message = "Product added successfully.";
                // Clear cache after success
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(AddProduct));
            }
            return serviceResponse;
        }

        private string GenerateNextTransactionNumber(IEnumerable<Prefixes> prefix)
        {
            var first = prefix.FirstOrDefault();
            var result = _mapper.Map<GetPrefixesDto>(first);
            if (result.CurrentNumber > 0)
            {
                result.CurrentNumber += 1;
                var PrefixLength = result.Prefix.Length;
                var TransLength = result.TransactionLength;
                var CurrentNumberLength = result.CurrentNumber.ToString().Length;
                result.Prefix = result.Prefix.PadRight(TransLength - CurrentNumberLength, '0');
                if (PrefixLength + CurrentNumberLength <= TransLength)
                {
                    result.TransactionNumber = $"{result.Prefix}{result.CurrentNumber}";
                }
                else
                {
                    result.TransactionNumber = null;
                }
            }
            else
            {
                var TransLength = result.TransactionLength;
                var StartNumberLength = result.StartNumber.ToString().Length;
                result.Prefix = result.Prefix.PadRight(TransLength - StartNumberLength, '0');
                result.TransactionNumber = $"{result.Prefix}{result.StartNumber}";
            }
            return result.TransactionNumber;
        }
        //update
        public async Task<ServiceResponse<GetProductsDto>> UpdateProduct(UpdateProductsDto updateProductsDto)
        {
            var serviceResponse = new ServiceResponse<GetProductsDto>();
            try
            {
                var _productsRepo = _unitOfWork.GetRepository<Products>();
                var data = await _productsRepo.GetByIdAsync(updateProductsDto.ID);
                if (data != null)
                {
                    data.StockStatusID = updateProductsDto.StockStatusID;
                    data.Code = updateProductsDto.Code;
                    data.Name = updateProductsDto.Name;
                    data.Quantity = updateProductsDto.Quantity;
                    data.MinimumQuantity = updateProductsDto.MinimumQuantity;
                    data.ProductFor = updateProductsDto.ProductFor;
                    data.Description = updateProductsDto.Description;
                    data.SellingPrice = updateProductsDto.SellingPrice;
                    data.CostPrice = updateProductsDto.CostPrice;
                    data.IsShipping = updateProductsDto.IsShipping;
                    data.Status = updateProductsDto.Status;
                    data.MRP = updateProductsDto.MRP;
                    data.SKU = updateProductsDto.SKU;
                    data.BarCode = updateProductsDto.BarCode;
                    data.IsProductDeductable = updateProductsDto.IsProductDeductable;
                    data.IsTaxApplicable = updateProductsDto.IsTaxApplicable;
                    data.Tax = updateProductsDto.Tax;
                    data.ModifiedBy = GetUserId();
                    data.ModifiedAt = DateTime.Now;
                    // upload document
                    if (updateProductsDto.File != null && updateProductsDto.File.Length > 0)
                    {
                        var objUploadFileDto = new UploadFileDto()
                        {
                            Directory = $"Products/ProductImage/{data.ID}",
                            File = updateProductsDto.File
                        };
                        var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                        if (dataFileService.Success)
                        {
                            data.Path = dataFileService.Data.Path;
                        }
                    }

                    _productsRepo.Update(data);

                    // check pricelist
                    var _priceListRepo = _unitOfWork.GetRepository<PriceList>();
                    var priceList = await _priceListRepo.Search(x => x.StoreID == updateProductsDto.StoreID);
                    // update price list
                    var PriceListObj = priceList.FirstOrDefault();
                    PriceListObj.ModifiedBy = 1;// GetUserId();
                    PriceListObj.ModifiedAt = DateTime.Now;
                    await _priceListRepo.UpdateAsync(PriceListObj);

                    var _priceListDetailsRepo = _unitOfWork.GetRepository<PriceListDetails>();
                    var Details = await _priceListDetailsRepo.Search(x => x.PriceListID == PriceListObj.ID && x.ProductID == updateProductsDto.ID);
                    // update details
                    var DetailsObj = Details.FirstOrDefault();
                    DetailsObj.SalePrice = updateProductsDto.SellingPrice;
                    DetailsObj.PurchasePrice = updateProductsDto.CostPrice;
                    DetailsObj.ModifiedBy = GetUserId();
                    DetailsObj.ModifiedAt = DateTime.Now;
                    _priceListDetailsRepo.Update(DetailsObj);
                    await _unitOfWork.CommitAsync();

                    // update product 
                    if (updateProductsDto.ProductAttributes.Count() > 0)
                    {
                        var _productAttributesRepo = _unitOfWork.GetRepository<ProductAttributes>();
                        foreach (var r in updateProductsDto.ProductAttributes)
                        {
                            if (r.ID == 0)
                            {
                                //var _productAttributesRepo = _unitOfWork.GetRepository<ProductAttributes>();
                                var newAttribute = new AddProductAttributesDto();
                                var attributeData = _mapper.Map<ProductAttributes>(newAttribute);
                                attributeData.AttributesID = r.AttributesID;
                                attributeData.Value = r.Value;
                                attributeData.ProductsID = data.ID;
                                attributeData.Status = true;
                                attributeData.StoreID = data.StoreID;
                                attributeData.CreatedAt = DateTime.Now;
                                attributeData.CreatedBy = 1;// GetUserId();
                                await _productAttributesRepo.AddAsync(attributeData);
                                continue;
                            }
                           
                            var attributes = await _productAttributesRepo.GetByIdAsync(r.ID);
                            attributes.Value = r.Value;
                            attributes.AttributesID = r.AttributesID;
                            attributes.IsDeleted = r.IsDeleted;
                            attributes.ModifiedAt = DateTime.Now;
                            attributes.ModifiedBy = 1;// GetUserId();
                            await _productAttributesRepo.UpdateAsync(attributes);
                        }
                    }

                    //if (updateProductsDto.ProductCategories.Count() > 0)
                    //{
                    //    foreach (var r in updateProductsDto.ProductCategories)
                    //    {
                    //        if (r.Id == 0)
                    //        {
                    //            var newCategory = new AddProductCategoriesDto();
                    //            var categoryData = _mapper.Map<ProductCategories>(newCategory);
                    //            categoryData.ProductsId = data.Id;
                    //            categoryData.ProductCategoryId = r.ProductCategoryId;
                    //            categoryData.StoreId = data.StoreId;
                    //            categoryData.Status = true;
                    //            categoryData.CreatedAt = DateTime.Now;
                    //            categoryData.CreatedBy = GetUserId();
                    //            await _productCategoriesRepo.AddAsync(categoryData);
                    //            continue;
                    //        }

                    //        var categories = await _productCategoriesRepo.GetByIdAsync(r.Id);
                    //        categories.ProductsId = updateProductsDto.Id;
                    //        categories.ProductCategoryId = r.ProductCategoryId;
                    //        categories.IsDeleted = r.IsDeleted;
                    //        categories.ModifiedAt = DateTime.Now;
                    //        categories.ModifiedBy = GetUserId();
                    //        _productCategoriesRepo.Update(categories);
                    //    }
                    //}

                    if (updateProductsDto.ProductOptions.Count() > 0)
                    {
                        foreach (var r in updateProductsDto.ProductOptions)
                        {
                            var _productOptions = _unitOfWork.GetRepository<ProductOptions>();
                            if (r.ID == 0)
                            {
                                
                                var newOption = new AddProductOptionsDto();
                                var optionData = _mapper.Map<ProductOptions>(newOption);
                                optionData.ProductsID = data.ID;
                                optionData.OptionsID = r.OptionsId;
                                optionData.IsRequired = r.IsRequired;
                                optionData.Status = true;
                                optionData.CreatedAt = DateTime.Now;
                                optionData.CreatedBy = 1;// GetUserId();
                                await _productOptions.AddAsync(optionData);
                               // await _unitOfWork.CommitAsync();

                                if (r.ProductOptionDetails.Count() > 0)
                                {
                                    var newDetails = new AddProductOptionDetailsDto();
                                    foreach (var nd in r.ProductOptionDetails)
                                    {
                                        var _productOptionDetail = _unitOfWork.GetRepository<ProductOptionDetails>();
                                        var detailsData = _mapper.Map<ProductOptionDetails>(newDetails);
                                        detailsData.ProductOptionsID = optionData.ID;
                                        detailsData.OptionValue = nd.OptionValue;
                                        detailsData.Price = nd.Price;
                                        detailsData.Quantity = nd.Quantity;
                                        detailsData.IsSubstractFromPrice = nd.IsSubstractFromPrice;
                                        detailsData.Status = true;
                                        detailsData.CreatedAt = DateTime.Now;
                                        detailsData.CreatedBy = GetUserId();
                                        await _productOptionDetail.AddAsync(detailsData);
                                        continue;
                                    }
                                }
                                continue;
                            }
                            //var _productOptions = _unitOfWork.GetRepository<ProductOptions>();
                            var options = await _productOptions.GetByIdAsync(r.ID);
                            options.OptionsID = options.OptionsID;
                            options.IsRequired = r.IsRequired;
                            options.IsDeleted = r.IsDeleted;
                            options.ModifiedAt = DateTime.Now;
                            options.ModifiedBy = 1;//GetUserId();
                            _productOptions.Update(options);

                            // update details
                            var _productOptionDetails = _unitOfWork.GetRepository<ProductOptionDetails>();
                            var optionDetails = await _productOptionDetails.Search(x => x.ProductOptionsID == options.ID);
                            foreach (var od in optionDetails)
                            {
                                foreach (var rd in r.ProductOptionDetails)
                                {
                                    if (rd.ID == od.ID)
                                    {
                                        od.OptionValue = rd.OptionValue;
                                        od.IsSubstractFromPrice = rd.IsSubstractFromPrice;
                                        od.Quantity = rd.Quantity;
                                        od.IsDeleted = rd.IsDeleted;
                                        od.Price = rd.Price;
                                        od.ModifiedAt = DateTime.Now;
                                        od.ModifiedBy = GetUserId();
                                        _productOptionDetails.Update(od);
                                    }
                                }
                            }

                            if (r.ProductOptionDetails.Count() > 0)
                            {
                                var newDetails = new AddProductOptionDetailsDto();
                                foreach (var nd in r.ProductOptionDetails)
                                {
                                    if (nd.ID == 0)
                                    {
                                        var detailsData = _mapper.Map<ProductOptionDetails>(newDetails);
                                        detailsData.ProductOptionsID = r.ID;
                                        detailsData.OptionValue = nd.OptionValue;
                                        detailsData.Price = nd.Price;
                                        detailsData.IsSubstractFromPrice = nd.IsSubstractFromPrice;
                                        detailsData.Status = true;
                                        detailsData.CreatedAt = DateTime.Now;
                                        detailsData.CreatedBy = GetUserId();
                                        await _productOptionDetails.AddAsync(detailsData);
                                    }
                                }
                            }
                        }
                    }

                    if (updateProductsDto.ProductImages.Count() > 0)
                    {
                        var _productImagesRepo = _unitOfWork.GetRepository<ProductImages>();
                        foreach (var img in updateProductsDto.ProductImages)
                        {
                            if (img.ID == 0)
                            {
                                var images = new AddProductImagesDto();
                                var imageData = _mapper.Map<ProductImages>(images);
                                var objUploadFileDto = new UploadFileDto()
                                {
                                    Directory = $"Products/ProductImage/{data.ID}",
                                    File = img.File
                                };
                                var dataFileService = await _fileService.UploadFileAsync(objUploadFileDto);
                                
                                if (dataFileService.Success)
                                {
                                    imageData.ProductsID = data.ID;
                                    imageData.StoreID = img.StoreID;
                                    imageData.Status = img.Status;
                                    imageData.Path = dataFileService.Data.Path;
                                    imageData.CreatedBy = GetUserId();
                                    imageData.CreatedAt = DateTime.Now;
                                    await _productImagesRepo.AddAsync(imageData);
                                }
                                continue;
                            }

                            var savedImages = await _productImagesRepo.GetByIdAsync(img.ID);
                            savedImages.IsDeleted = img.IsDeleted;
                            savedImages.ModifiedAt = DateTime.Now;
                            savedImages.ModifiedBy = GetUserId();
                            _productImagesRepo.Update(savedImages);
                        }
                    }

                    if (updateProductsDto.RelatedProducts.Count() > 0)
                    {
                        var _relatedProductsRepo = _unitOfWork.GetRepository<RelatedProducts>();
                        foreach (var r in updateProductsDto.RelatedProducts)
                        {
                            if (r.ID == 0)
                            {
                                var newRelatedProduct = new AddRelatedProductsDto();
                                var relatedProductData = _mapper.Map<RelatedProducts>(newRelatedProduct);
                                relatedProductData.ProductsID = r.RelatedProductID;
                                relatedProductData.RelatedProductID = data.ID;
                                relatedProductData.StoreID = data.StoreID;
                                relatedProductData.Status = true;
                                await _relatedProductsRepo.AddAsync(relatedProductData);
                                continue;
                            }
                            else
                            {
                                var relatedProducts = await _relatedProductsRepo.GetByIdAsync(r.ID);
                                relatedProducts.RelatedProductID = data.ID;
                                relatedProducts.IsDeleted = r.IsDeleted;
                                relatedProducts.ModifiedAt = DateTime.Now;
                                relatedProducts.ModifiedBy = GetUserId();
                                _relatedProductsRepo.Update(relatedProducts);
                            }
                        }
                    }

                    await _unitOfWork.CommitAsync();

                    serviceResponse.Data = _mapper.Map<GetProductsDto>(data);
                    serviceResponse.Message = "Product updated successfully.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Product not found.";
                    // Clear cache after success
                    _memoryCache.Remove(CacheKey);
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateProduct)}");
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<DeleteProductsDto>> DeleteProduct(int Id)
        {
            var serviceResponse= new ServiceResponse<DeleteProductsDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var product = _unitOfWork.GetRepository<Products>();
                var result = await product.GetByIdAsync(Id);
                result.IsDeleted = true;
                result.ModifiedBy = 1;//GetUserId();
                result.ModifiedAt = DateTime.UtcNow;
                var data = await product.UpdateAsync(result);
                await _unitOfWork.SaveChangesAsync();

                // Other tbls inserts or updates

                await _unitOfWork.CommitAsync();

                serviceResponse.Data = _mapper.Map<DeleteProductsDto>(data);

                // Clear cache after success
                _memoryCache.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(DeleteProduct));
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<IList<GetProductsDto>>> GetProductsOnSearch(SearchProductsDto searchProductsDto)
        {
            var serviceResponse=new ServiceResponse<IList<GetProductsDto>>();
            try
            {
                IList<GetProductsDto> products = new List<GetProductsDto>();
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _productsRepo = _unitOfWork.GetRepository<Products>();
                var result = await _productsRepo.Search(
                        x => x.StoreID == searchProductsDto.StoreID &&
                        x.Code.ToLower().Contains(searchProductsDto.Code) ||
                        x.BarCode.ToLower().Contains(searchProductsDto.BarCode) ||
                        x.Name.ToLower().Contains(searchProductsDto.Name));
                var data = _mapper.Map<IEnumerable<GetProductsDto>>(result.OrderByDescending(x=>x.CreatedAt));
                foreach(var d in data)
                {
                    if (d.StoreID == searchProductsDto.StoreID)
                    {
                        products.Add(d);
                    }
                }

                serviceResponse.Data = products;
            }
            catch (Exception ex)
            {
                await RollbackAndHandleException(serviceResponse, ex, nameof(GetProductsOnSearch));
            }
            return serviceResponse;
        }
    }
}
