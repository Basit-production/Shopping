using Ahmed_mart.Dtos.v1.GetOrderByStoreDtos;
using Ahmed_mart.Dtos.v1.OrderDetailsDto;
using Ahmed_mart.Dtos.v1.OrdersDto;
using Ahmed_mart.Dtos.v1.ReportsDto;
using Ahmed_mart.Dtos.v1.StoreDto;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1;
using Ahmed_mart.Repository.v1.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Ahmed_mart.Services.v1.OrdersService
{
    public class OrdersService : BaseService, IOrdersService
    {
        protected override string CacheKey => "adminauthCacheKey";
        public OrdersService(IUnitOfWork unitOfWork,
            ILogger<BaseService> logger,
            IMapper mapper,
            IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration):
            base(unitOfWork, logger, mapper, memoryCache, httpContextAccessor, configuration)
        {}

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
             .FindFirstValue(ClaimTypes.NameIdentifier)!);

        private int GetUserTrackerId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue("UserTrackerID")!);

        public async Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetOrders()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetOrdersDto>>();
            try
            {
                if (_memoryCache.TryGetValue(CacheKey, out IEnumerable<GetOrdersDto>? data))
                {
                    serviceResponse.Data = data;
                }
                else
                {
                    var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                    var result = await _ordersRepo.SearchAsync(x=>!x.IsDeleted);
                    data = _mapper.Map<IEnumerable<GetOrdersDto>>(result.OrderByDescending(x=>x.CreatedAt));
                    serviceResponse.Data = data;
                    _memoryCache.Set(CacheKey, data, GetCacheEntryOptions());
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetOrders));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetOrdersDto>> GetOrder(int Id)
        {
            var serviceResponse = new ServiceResponse<GetOrdersDto>();
            try
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.GetSingleAsync(Id);
                if (result != null)
                {
                    var data = _mapper.Map<GetOrdersDto>(result);
                    serviceResponse.Data = data;
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Order not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetOrders));
            }
            return serviceResponse;
        }

        //public async Task<ServiceResponse<GetOrdersDto>> AddOrder(AddOrdersDto addOrdersDto)
        //{
        //    var serviceResponse = new ServiceResponse<GetOrdersDto>();
        //    try
        //    {
        //        const string cacheKey = "GetOrders";
        //        bool InvalidEntry = false;
        //        bool TransactionNumberExist = false;
        //        decimal ProductQty = 0;
        //        decimal balanceQty = 0;

        //        var orders = await _ordersRepo.Search(x=>x.StoreId == addOrdersDto.StoreId);
        //        foreach (var ord in orders)
        //        {
        //            if (ord.TransactionNumber == addOrdersDto.TransactionNumber)
        //            {
        //                TransactionNumberExist = true;
        //            }
        //        }
        //        if (TransactionNumberExist == true)
        //        {
        //            var Prefix = await _prefixesRepo.Search(x => x.TransactionType == 3 && x.StoreId == addOrdersDto.StoreId);
        //            string GetNextTransactionNumber = GenerateNextTransactionNumber(Prefix);
        //            addOrdersDto.TransactionNumber = GetNextTransactionNumber;
        //        }

        //        foreach (var a in addOrdersDto.OrderDetails)
        //        {
        //            var priceListDetails = await _priceListDetailsRepo.Search(x => x.ProductId == a.ProductsId);
        //            a.PriceListDetailsId = priceListDetails.FirstOrDefault().Id;
        //            a.CreatedAt = DateTime.Now;
        //            a.CreatedBy = GetUserId();
        //        }
        //        var data = _mapper.Map<Orders>(addOrdersDto);
        //        data.OrderType = addOrdersDto.OrderType;
        //        data.CreatedBy = GetUserId();
        //        data.CreatedAt = DateTime.Now;
        //        await _ordersRepo.AddAsync(data);
        //        await _unitOfWork.CommitAsync();

        //        // generate invoice number
        //        string InvoiceNumber = GenerateInvoiceNumber(data.Id);
        //        data.InvoiceNumber = InvoiceNumber;
        //        _ordersRepo.Update(data);

        //        // get store
        //        var objStore = await _storeRepo.GetByIdAsync(data.StoreId);
        //        if (objStore.IsFreeBirdModule)
        //        {
        //            // check preference
        //            var preference = await _freeBirdPreferenceRepo.Search(x => x.StoreId == objStore.Id);
        //            // check orders
        //            var freeBirdOrders = await _freeBirdRepo.Search(x => x.CustomersId == data.CustomersId);
                    
        //            if (preference.Any())
        //            {
        //                var objPreference = preference.FirstOrDefault();
        //                if (freeBirdOrders.Count() == 0)
        //                {
        //                    if (data.GrandTotal > objPreference.PriceRange)
        //                    {
        //                        decimal credit = data.GrandTotal * objPreference.ReturnCredits / 100;
        //                        var freeBird = new FreeBird()
        //                        {
        //                            StoreId = data.StoreId,
        //                            CustomersId = data.CustomersId,
        //                            OrdersId = data.Id,
        //                            TotalAmount = data.TotalAmount,
        //                            Discount = data.Discount,
        //                            DiscountValue = data.DiscountValue,
        //                            GrandTotal = data.GrandTotal,
        //                            Tax = data.Tax,
        //                            Credit = credit,
        //                            Debit = 0,
        //                            Status = true,
        //                            IsFirstOrder = true,
        //                        };
        //                        await _freeBirdRepo.AddAsync(freeBird);

        //                        // update customer credits
        //                        var cust = await _customersRepo.GetByIdAsync(data.CustomersId);
        //                        cust.Credits += credit;
        //                        cust.ModifiedAt = DateTime.Now;
        //                        cust.ModifiedBy = GetUserId();
        //                        _customersRepo.Update(cust);
        //                    }
        //                }
        //                else
        //                {
        //                    var totalOrders = 0;
        //                    foreach(var fb in freeBirdOrders)
        //                    {
        //                        if (!fb.IsFirstOrder)
        //                        {
        //                            totalOrders++;
        //                        }
        //                    }
        //                    if(objPreference.NoOfOrders > totalOrders)
        //                    {
        //                        var objFreeBird = freeBirdOrders.Where(x => x.IsFirstOrder);
        //                        var totalCredit = objFreeBird.FirstOrDefault().Credit;

        //                        var subCredit = totalCredit / objPreference.NoOfOrders;

        //                        //
        //                        var freeBird = new FreeBird()
        //                        {
        //                            StoreId = data.StoreId,
        //                            CustomersId = data.CustomersId,
        //                            OrdersId = data.Id,
        //                            TotalAmount = data.TotalAmount,
        //                            Discount = data.Discount,
        //                            DiscountValue = data.DiscountValue,
        //                            GrandTotal = data.GrandTotal,
        //                            Tax = data.Tax,
        //                            Credit = 0,
        //                            Debit = subCredit,
        //                            Status = true,
        //                        };
        //                        await _freeBirdRepo.AddAsync(freeBird);

        //                        // update customer credits
        //                        var cust = await _customersRepo.GetByIdAsync(data.CustomersId);
        //                        cust.Credits -= subCredit;
        //                        cust.ModifiedAt = DateTime.Now;
        //                        cust.ModifiedBy = GetUserId();
        //                        _customersRepo.Update(cust);
        //                    }
        //                }
        //            }
        //        }
        //        // deduct quantity from product
        //        foreach (var p in addOrdersDto.OrderDetails)
        //        {
        //            int UOMDID = p.UOMTemplateDetailsId ?? default;
        //            int PID = p.ProductsId ?? default;
        //            var uom = await _uOMTemplateDetailsRepo.GetByIdAsync(UOMDID);
        //            // get product
        //            decimal RelationToBase = uom.RelationToBaseUnit;
        //            var product = await _productsRepo.GetByIdAsync(PID);
        //            if (product.StockStatusId == 1 && product.IsProductDeductable == true)
        //            {
        //                if (product.Quantity > 0 && product.Quantity >= (p.Quantity / RelationToBase))
        //                {
        //                    product.Quantity -= (p.Quantity / RelationToBase);
        //                    if (product.Quantity == 0)
        //                    {
        //                        product.StockStatusId = 2;
        //                    }
        //                    product.ModifiedAt = DateTime.Now;
        //                    product.ModifiedBy = GetUserId();
        //                    _productsRepo.Update(product);
        //                }
        //                else
        //                {
        //                    InvalidEntry = true;
        //                }
        //            }
        //            // deduct from location qty
        //            ProductQty = p.Quantity / RelationToBase;
        //            var locQty = await _locationQuantityRepo.Search(x => x.ProductsId == PID);
        //            if (locQty.Any())
        //            {
        //                var objLocQty = locQty.FirstOrDefault();
        //                objLocQty.Balance -= p.Quantity / RelationToBase;
        //                objLocQty.ModifiedAt = DateTime.Now;
        //                objLocQty.ModifiedBy = GetUserId();
        //                _locationQuantityRepo.Update(objLocQty);

        //                // add item transaction
        //                var ItemTran = await _itemTransactionRepo.Search(x => x.LocationId == objLocQty.LocationId && x.ProductsId == PID && x.TransactionMode == 1 && x.BalanceQuantity > 0);
        //                if (ItemTran.Count() > 0)
        //                {
        //                    foreach (var ItmTrn in ItemTran)
        //                    {
        //                        if (ProductQty > 0)
        //                        {
        //                            if (ProductQty >= ItmTrn.BalanceQuantity)
        //                            {
        //                                balanceQty = ProductQty - ItmTrn.BalanceQuantity.GetValueOrDefault();
        //                                ItemTransaction AddItemTransaction = BuildItemTransaction(objLocQty.LocationId, PID, ItmTrn.BalanceQuantity.GetValueOrDefault(), ItmTrn.Cost, ItmTrn.Id, data.Id, data.CreatedAt, data.CreatedBy);
        //                                _itemTransactionRepo.Add(AddItemTransaction);
        //                                ProductQty = (decimal)balanceQty;

        //                                ItmTrn.BalanceQuantity = 0;
        //                                ItmTrn.ModifiedBy = GetUserId();
        //                                ItmTrn.ModifiedAt = DateTime.Now;
        //                                _itemTransactionRepo.Update(ItmTrn);
        //                            }
        //                            else
        //                            {
        //                                ItemTransaction AddItemTransaction = BuildItemTransaction(objLocQty.LocationId, PID, ProductQty, ItmTrn.Cost, ItmTrn.Id, data.Id, data.CreatedAt, data.CreatedBy);
        //                                _itemTransactionRepo.Add(AddItemTransaction);
        //                                ItmTrn.BalanceQuantity -= ProductQty;
        //                                ItmTrn.ModifiedBy = GetUserId();
        //                                ItmTrn.ModifiedAt = DateTime.Now;
        //                                _itemTransactionRepo.Update(ItmTrn);
        //                                ProductQty = 0;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // update coupon
        //        if(addOrdersDto.CouponID > 0)
        //        {
        //            var coupon = await _couponsRepo.GetByIdAsync(addOrdersDto.CouponID);
        //            if(coupon.UsePerCoupon == 0 || coupon.UsePerCustomer == 0)
        //            {
        //                InvalidEntry = true;
        //            }
        //            else
        //            {
        //                var customerCoupon = await _customerCouponsRepo.Search(x => x.CustomersId == addOrdersDto.CustomersId);
        //                if (customerCoupon.Any())
        //                {
        //                    if (coupon.UsePerCustomer > customerCoupon.Count())
        //                    {
        //                        // update coupon
        //                        coupon.UsePerCoupon--;
        //                        coupon.UsePerCustomer--;
        //                        coupon.ModifiedAt = currentDate;
        //                        coupon.ModifiedBy = GetUserId();
        //                        if(coupon.UsePerCoupon == 0 || coupon.UsePerCustomer == 0)
        //                        {
        //                            coupon.Status = false;
        //                        }
        //                        _couponsRepo.Update(coupon);

        //                        // update customer coupon
        //                        CustomerCoupons add = AddCustomerCoupon(coupon.Id, addOrdersDto.CustomersId);
        //                        await _customerCouponsRepo.AddAsync(add);
        //                    }
        //                    else
        //                    {
        //                        InvalidEntry = true;
        //                    }
        //                }
        //                else
        //                {
        //                    // update coupon
        //                    coupon.UsePerCoupon--;
        //                    coupon.UsePerCustomer--;
        //                    coupon.ModifiedAt = currentDate;
        //                    coupon.ModifiedBy = GetUserId();
        //                    if (coupon.UsePerCoupon == 0 || coupon.UsePerCustomer == 0)
        //                    {
        //                        coupon.Status = false;
        //                    }
        //                    _couponsRepo.Update(coupon);

        //                    // update customer coupon
        //                    CustomerCoupons add = AddCustomerCoupon(coupon.Id, addOrdersDto.CustomersId);
        //                    await _customerCouponsRepo.AddAsync(add);
        //                }
        //            }
        //        }

        //        // update prefix
        //        var UpdatePrefix = await _prefixesRepo.Search(x => x.TransactionType == 3 && x.StoreId == addOrdersDto.StoreId);
        //        var TransResult = UpdatePrefix.FirstOrDefault();
        //        if (TransResult.CurrentNumber > 0)
        //        {
        //            TransResult.CurrentNumber += 1;
        //            TransResult.ModifiedBy = GetUserId();
        //            TransResult.ModifiedAt = DateTime.Now;
        //            _prefixesRepo.Update(TransResult);
        //        }
        //        else
        //        {
        //            TransResult.CurrentNumber = TransResult.StartNumber;
        //            TransResult.ModifiedBy = GetUserId();
        //            TransResult.ModifiedAt = DateTime.Now;
        //            _prefixesRepo.Update(TransResult);
        //        }

        //        if (InvalidEntry == false)
        //        {
        //            await _unitOfWork.CommitAsync();

        //            if(data.OrderPayments.PaymentMode == 2)
        //            {
        //                // get payment preference
        //                var store = await _storeRepo.GetByIdAsync(data.StoreId);
        //                if(store.PaymentPreference == true)
        //                {

        //                    var paymentInfo = await _paymentGatewayConfigurationRepo.Search(x => x.StoreId == data.StoreId);
        //                    var objInfo = paymentInfo.FirstOrDefault();
        //                    var paymentGatewayServiceResponse = _paymentGatewayService.AddRazorpayOrder(objInfo.Key, objInfo.Secret, $"ORD{data.Id}", data.GrandTotal);

        //                    // update razor pay order details
        //                    data.RazorPayOrderId = paymentGatewayServiceResponse.Data["id"].ToString();
        //                    data.RazorPayStatus = paymentGatewayServiceResponse.Data["status"].ToString();
        //                    var dataOrder = _mapper.Map<GetOrdersDto>(data);
        //                    dataOrder.Key = objInfo.Key;
        //                    dataOrder.Secret = objInfo.Secret;
        //                    serviceResponse.Data = dataOrder;
        //                }
        //                else
        //                {
        //                    var paymentInfo = await _paymentGatewayConfigurationRepo.Search(x => x.StoreId == null);
        //                    var objInfo = paymentInfo.FirstOrDefault();
        //                    var paymentGatewayServiceResponse = _paymentGatewayService.AddRazorpayOrder(objInfo.Key, objInfo.Secret, $"ORD{data.Id}", data.GrandTotal);

        //                    // update razor pay order details
        //                    data.RazorPayOrderId = paymentGatewayServiceResponse.Data["id"].ToString();
        //                    data.RazorPayStatus = paymentGatewayServiceResponse.Data["status"].ToString();
        //                    var dataOrder = _mapper.Map<GetOrdersDto>(data);
        //                    dataOrder.Key = objInfo.Key;
        //                    dataOrder.Secret = objInfo.Secret;
        //                    serviceResponse.Data = dataOrder;
        //                }
        //                serviceResponse.Message = "Order added successfully.";
        //            }
        //            else
        //            {
        //                var dataOrder = _mapper.Map<GetOrdersDto>(data);
        //                serviceResponse.Data = dataOrder;
        //            }
        //        }
        //        else
        //        {
        //            serviceResponse.Success = false;
        //            serviceResponse.Message = "Sorry! Order cannot be place as one or more products are out of stock.";
        //        }
        //        await _distributedCache.RemoveAsync(cacheKey);//Remove cache
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Success = false;
        //        serviceResponse.Message = ex.GetType().ToString();
        //        _logger.LogError(ex, $"Something Went Wrong in the {nameof(AddOrder)}");
        //    }
        //    return serviceResponse;
        //}

        //private ItemTransaction BuildItemTransaction(int locationId, int pID, decimal productQty, decimal cost, int id1, int id2, DateTime createdAt, int createdBy)
        //{
        //    ItemTransaction trans = new ItemTransaction()
        //    {
        //        LocationId = locationId,
        //        ProductsId = pID,
        //        Quantity = productQty * (-1),
        //        Cost = cost,
        //        TransactionDate = createdAt,
        //        TransactionType = 1,
        //        TransactionMode = 2,
        //        OriginalID = id1,
        //        BalanceQuantity = productQty,
        //        TransactionID = id2,
        //        CreatedAt = createdAt,
        //        CreatedBy = createdBy,
        //    };
        //    return trans;
        //}

        //private string GenerateInvoiceNumber(int id)
        //{
        //    const string src = "abcdefghijklmnopqrstuvwxyz0123456789";
        //    int length = 5;
        //    var sb = new StringBuilder();
        //    Random RNG = new Random();
        //    for (var i = 0; i < length; i++)
        //    {
        //        var c = src[RNG.Next(0, src.Length)];
        //        sb.Append(c);
        //    }
        //    var inv = sb.ToString() + id.ToString();
        //    return inv;
        //}

        //private string GenerateNextTransactionNumber(IEnumerable<Prefixes> prefix)
        //{
        //    var first = prefix.FirstOrDefault();
        //    var result = _mapper.Map<GetPrefixesDto>(first);
        //    if (result.CurrentNumber > 0)
        //    {
        //        result.CurrentNumber += 1;
        //        var PrefixLength = result.Prefix.Length;
        //        var TransLength = result.TransactionLength;
        //        var CurrentNumberLength = result.CurrentNumber.ToString().Length;
        //        result.Prefix = result.Prefix.PadRight(TransLength - CurrentNumberLength, '0');
        //        if (PrefixLength + CurrentNumberLength <= TransLength)
        //        {
        //            result.TransactionNumber = $"{result.Prefix}{result.CurrentNumber}";
        //        }
        //        else
        //        {
        //            result.TransactionNumber = null;
        //        }
        //    }
        //    else
        //    {
        //        var TransLength = result.TransactionLength;
        //        var StartNumberLength = result.StartNumber.ToString().Length;
        //        result.Prefix = result.Prefix.PadRight(TransLength - StartNumberLength, '0');
        //        result.TransactionNumber = $"{result.Prefix}{result.StartNumber}";
        //    }
        //    return result.TransactionNumber;
        //}

        //private CustomerCoupons AddCustomerCoupon(int id, int? customersId)
        //{
        //    var CustomerCoupons = new CustomerCoupons()
        //    {
        //        CouponsId = id,
        //        CustomersId = customersId.GetValueOrDefault(),
        //        Status = true,
        //        CreatedAt = currentDate,
        //        CreatedBy = GetUserId()
        //    };
        //    return CustomerCoupons;
        //}

        public async Task<ServiceResponse<GetOrdersDto>> UpdateOrder(UpdateOrdersDto updateOrdersDto)
        {
            var serviceResponse = new ServiceResponse<GetOrdersDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var data = await _ordersRepo.GetByIdAsync(updateOrdersDto.ID);
                if (data != null)
                {
                    data.OrdersStatusID = updateOrdersDto.OrdersStatusID;
                    data.ModifiedBy = 1;// GetUserId();
                    data.ModifiedAt = DateTime.Now;

                    foreach (var d in data.OrderDetails)
                    {
                        d.ModifiedBy = 1;// GetUserId();
                        d.ModifiedAt = DateTime.Now;
                    }
                    await  _ordersRepo.UpdateAsync(data);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetOrdersDto>(data);
                    serviceResponse.Message = "Order updated successfully.";
                    // Clear cache after success
                    _memoryCache.Remove(CacheKey);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Order not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(UpdateOrder));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetOrdersDto>> DeleteOrder(int Id)
        {
            var serviceResponse = new ServiceResponse<GetOrdersDto>();
            try
            {
                using var transaction = _unitOfWork.BeginTransactionAsync();
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var data = await _ordersRepo.GetByIdAsync(Id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.ModifiedBy = GetUserId();
                    data.ModifiedAt = DateTime.Now;
                    await _ordersRepo.UpdateAsync(data);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    serviceResponse.Data = _mapper.Map<GetOrdersDto>(data);
                    serviceResponse.Message = "Order deleted successfully.";
                    _memoryCache.Remove(CacheKey);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Order not found.";
                }
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(DeleteOrder));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetOrdersByStore(GetOrderByStoreFilterDto addOrdersDto)
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetOrdersDto>>();
            try
            {
                if (addOrdersDto.Default == true)
                {
                    var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                    var result = await _ordersRepo.Search(x => x.CreatedAt.Date >= addOrdersDto.FromDate &&
                            x.CreatedAt.Date <= addOrdersDto.ToDate &&
                            x.StoreID == addOrdersDto.StoreID &&
                            x.IsDeleted == false,
                            x => x.OrdersStatus
                            // x => x.Customers //Customer Becassily users
                            );
                    var data = _mapper.Map<IEnumerable<GetOrdersDto>>(result.OrderByDescending(x => x.CreatedAt));
                    serviceResponse.Data = data;
                }
                else
                {
                    var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                    var result = await _ordersRepo.Search(x => x.StoreID == addOrdersDto.StoreID &&
                            x.IsDeleted == false,
                            x => x.OrdersStatus
                            // x => x.Customers //Customer Becassily users
                            );
                    var data = _mapper.Map<IEnumerable<GetOrdersDto>>(result.OrderByDescending(x => x.CreatedAt));
                    if (addOrdersDto.FromDate != null && addOrdersDto.ToDate != null)
                    {
                        data = data.Where(x => x.CreatedAt.Date >= addOrdersDto.FromDate &&
                            x.CreatedAt.Date <= addOrdersDto.ToDate);
                    }
                    if (addOrdersDto.OrderStatus != null)
                    {
                        data = data.Where(x => x.OrdersStatusId == addOrdersDto.OrderStatus);
                    }
                    //if (addOrdersDto.CustomersId != null)
                    //{
                    //    data = data.Where(x => x.CustomersId == addOrdersDto.CustomersId);
                    //}
                    serviceResponse.Data = data;
                }
            }
            catch(Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetOrdersByStore));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetRecentBuyersByStore(int storeId)
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetOrdersDto>>();
            try
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.Search(
                    x => x.IsDeleted == false &&
                    x.StoreID == storeId);
                var data = _mapper.Map<IEnumerable<GetOrdersDto>>(result.OrderByDescending(x => x.CreatedAt));
                serviceResponse.Data = data.Take(5);
            }
            catch (Exception ex)
            {
                HandleException(serviceResponse, ex, nameof(GetRecentBuyersByStore));
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IList<GetCustomerSalesReportDto>>> GetCustomerSalesReportByStore(GetOrderByStoreFilterDto addOrdersDto)
        {
            var serviceResponse = new ServiceResponse<IList<GetCustomerSalesReportDto>>();
            IList<GetCustomerSalesReportDto> report = new List<GetCustomerSalesReportDto>();
            int count = 0;
            if (addOrdersDto.Default == true)
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.Search(x => x.StoreID == addOrdersDto.StoreID &&
                        x.IsDeleted == false,
                        x => x.OrdersStatus
                        // x => x.Customers //Customer Becassily users
                        );

                //foreach (var res in result.GroupBy(x => x.CustomersId))
                //{
                //    count = 0;
                //    var data = new GetCustomerSalesReportDto();
                //    data.CustomersId = res.FirstOrDefault().CustomersId;
                //    data.CustomerGroupId = res.FirstOrDefault().Customers.CustomerGroupId;
                //    data.Path = res.FirstOrDefault().Customers.Path;
                //    data.Name = res.FirstOrDefault().Customers.Name;
                //    data.MobileNumber = res.FirstOrDefault().Customers.MobileNumber;
                //    data.Status = res.FirstOrDefault().Customers.Status;
                //    data.TotalOrders = res.Count();
                //    foreach (var list in res)
                //    {
                //        foreach (var l in list.OrderDetails.GroupBy(x => x.ProductsId))
                //        {
                //            count++;
                //        }
                //    }
                //    data.TotalProducts = count++;
                //    report.Add(data);
                //}
                serviceResponse.Data = report;
            }
            else
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.Search(x => x.CreatedAt.Date >= addOrdersDto.FromDate &&
                        x.CreatedAt.Date <= addOrdersDto.ToDate &&
                        x.StoreID == addOrdersDto.StoreID &&
                        x.IsDeleted == false,
                        x => x.OrdersStatus
                        //x => x.Customers
                        );

                //foreach (var res in result.GroupBy(x => x.CustomersId))
                //{
                //    count = 0;
                //    var data = new GetCustomerSalesReportDto();
                //    data.CustomersId = res.FirstOrDefault().CustomersId;
                //    data.CustomerGroupId = res.FirstOrDefault().Customers.CustomerGroupId;
                //    data.Path = res.FirstOrDefault().Customers.Path;
                //    data.Name = res.FirstOrDefault().Customers.Name;
                //    data.MobileNumber = res.FirstOrDefault().Customers.MobileNumber;
                //    data.Status = res.FirstOrDefault().Customers.Status;
                //    data.TotalOrders = res.Count();
                //    foreach (var list in res)
                //    {
                //        foreach (var l in list.OrderDetails.GroupBy(x => x.ProductsId))
                //        {
                //            count++;
                //        }
                //    }
                //    data.TotalProducts = count++;
                //    report.Add(data);
                //}

                if (addOrdersDto.CustomerStatus != null)
                {
                    bool Status = false;
                    Status = addOrdersDto.CustomerStatus == 1 ? true : false;
                    report = report.Where(x => x.Status == Status).ToList();
                }
                //if (addOrdersDto.CustomersId != null)
                //{
                //    report = report.Where(x => x.CustomersId == addOrdersDto.CustomersId).ToList();
                //}
                serviceResponse.Data = report;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IList<GetSoldProductsReportDto>>> GetSoldProductsReportByStore(GetOrderByStoreFilterDto addOrdersDto)
        {
            var serviceResponse = new ServiceResponse<IList<GetSoldProductsReportDto>>();
            IList<GetOrderDetailsDto> details = new List<GetOrderDetailsDto>();
            IList<GetSoldProductsReportDto> report = new List<GetSoldProductsReportDto>();
            if (addOrdersDto.Default == true)
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.Search(x => x.StoreID == addOrdersDto.StoreID &&
                        x.IsDeleted == false,
                        x => x.OrderDetails);
                //var dataOrders = result.AsQueryable().Select(x => x.OrderDetails);
                foreach (var res in result)
                {
                    foreach (var r in res.OrderDetails)
                    {
                        var list = _mapper.Map<GetOrderDetailsDto>(r);
                        details.Add(list);
                    }
                }

                foreach (var r in details.GroupBy(x => x.ProductsID))
                {
                    int Qty = 0;
                    decimal Total = 0;
                    var data = new GetSoldProductsReportDto();
                    data.Name = r.FirstOrDefault().Products.Name;
                    data.AdditionalCode = r.FirstOrDefault().Products.SKU;
                    foreach (var d in r)
                    {
                        Qty += d.Quantity;
                        Total += d.GrandTotal;
                    }
                    data.Quantity = Qty;
                    data.Total = Total;
                    report.Add(data);
                }
                serviceResponse.Data = report;
            }
            else
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.Search(x => x.StoreID == addOrdersDto.StoreID &&
                        x.IsDeleted == false,
                        x => x.OrderDetails);
                //var dataOrders = result.AsQueryable().Select(x => x.OrderDetails);
                var data = new GetSoldProductsReportDto();
                foreach (var res in result)
                {

                    foreach (var r in res.OrderDetails)
                    {
                        if ((addOrdersDto.FromDate != null && addOrdersDto.ToDate != null))
                        {
                            if (r.CreatedAt.Date >= addOrdersDto.FromDate && r.CreatedAt.Date <= addOrdersDto.ToDate)
                            {
                                var list = _mapper.Map<GetOrderDetailsDto>(r);
                                details.Add(list);
                            }
                        }
                        else
                        {
                            var list = _mapper.Map<GetOrderDetailsDto>(r);
                            details.Add(list);
                        }
                    }
                }

                foreach (var r in details.GroupBy(x => x.ProductsID))
                {
                    int Qty = 0;
                    decimal Total = 0;

                    data.Name = r.FirstOrDefault().Products.Name;
                    data.ProductsId = r.FirstOrDefault().ProductsID ?? default(int);
                    data.AdditionalCode = r.FirstOrDefault().Products.SKU;
                    foreach (var d in r)
                    {
                        Qty += d.Quantity;
                        Total += d.GrandTotal;
                    }
                    data.Quantity = Qty;
                    data.Total = Total;
                    report.Add(data);
                }

                if (addOrdersDto.ProductsID != null)
                {
                    report = report.Where(x => x.ProductsId == addOrdersDto.ProductsID).ToList();
                }
                serviceResponse.Data = report;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IList<GetSalesReportDto>>> GetSalesReportByStore(GetOrderByStoreFilterDto addOrdersDto)
        {
            var serviceResponse = new ServiceResponse<IList<GetSalesReportDto>>();
            IList<GetSalesReportDto> report = new List<GetSalesReportDto>();
            if (addOrdersDto.Default == true)
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.Search(x => x.StoreID == addOrdersDto.StoreID &&
                        x.IsDeleted == false,
                        x => x.OrdersStatus,
                        x => x.OrderDetails);
                var data = new GetSalesReportDto();
                data.TotalOrders = result.Count();
                foreach (var res in result)
                {
                    data.Tax += res.Tax;
                    data.GrandTotal += res.GrandTotal;
                    data.TotalProducts += res.OrderDetails.Count();
                }
                report.Add(data);
                serviceResponse.Data = report;
            }
            else
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.Search(x => x.CreatedAt.Date >= addOrdersDto.FromDate &&
                        x.CreatedAt.Date <= addOrdersDto.ToDate &&
                        x.StoreID == addOrdersDto.StoreID &&
                        x.IsDeleted == false,
                        x => x.OrdersStatus,
                        x => x.OrderDetails);
                var data = new GetSalesReportDto();
                data.TotalOrders = result.Count();

                foreach (var res in result)
                {
                    data.Status = res.OrdersStatusID;
                    data.Tax += res.Tax;
                    data.GrandTotal += res.GrandTotal;
                    data.TotalProducts += res.OrderDetails.Count();
                }
                report.Add(data);

                if (addOrdersDto.OrderStatus != null)
                {
                    report = report.Where(x => x.Status == addOrdersDto.OrderStatus).ToList();
                }
                serviceResponse.Data = report;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IList<GetBestSellingProductsDto>>> GetBestSellingProductsByStore(AddOrdersDto addOrdersDto)
        {
            var serviceResponse = new ServiceResponse<IList<GetBestSellingProductsDto>>();
            IList<GetOrderDetailsDto> details = new List<GetOrderDetailsDto>();
            IList<GetBestSellingProductsDto> report = new List<GetBestSellingProductsDto>();
            var _ordersRepo = _unitOfWork.GetRepository<Orders>();
            var result = await _ordersRepo.Search(x => x.StoreID == addOrdersDto.StoreID &&
                        x.IsDeleted == false,
                        x => x.OrderDetails);
            foreach (var res in result)
            {
                foreach (var r in res.OrderDetails)
                {
                    var list = _mapper.Map<GetOrderDetailsDto>(r);
                    details.Add(list);
                }
            }

            foreach (var r in details.GroupBy(x => x.ProductsID))
            {
                int Count = 0;
                var data = new GetBestSellingProductsDto();
                data.Path = r.FirstOrDefault().Products.Path;
                data.Name = r.FirstOrDefault().Products.Name;
                data.AdditionalCode = r.FirstOrDefault().Products.SKU;
                foreach (var d in r)
                {
                    Count += d.Quantity;
                }
                data.Count = Count;
                report.Add(data);
            }
            var reportData = report.OrderBy(x => x.Count);
            serviceResponse.Data = reportData.OrderByDescending(x => x.Count).Take(5).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetTotalOrdersByStore(int storeId)
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetOrdersDto>>();
            try
            {
                var _ordersRepo = _unitOfWork.GetRepository<Orders>();
                var result = await _ordersRepo.Search(
                    x => x.IsDeleted == false &&
                    x.StoreID == storeId);
                var data = _mapper.Map<IEnumerable<GetOrdersDto>>(result);
                serviceResponse.Data = data;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.GetType().ToString();
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetTotalOrdersByStore)}");
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IList<GetSoldProductsReportDto>>> GetSoldProductsByStore(int storeId)
        {
            var serviceResponse = new ServiceResponse<IList<GetSoldProductsReportDto>>();
            IList<GetOrderDetailsDto> details = new List<GetOrderDetailsDto>();
            IList<GetSoldProductsReportDto> report = new List<GetSoldProductsReportDto>();
            var _ordersRepo = _unitOfWork.GetRepository<Orders>();
            var result = await _ordersRepo.Search(x => x.StoreID == storeId &&
                       x.IsDeleted == false,
                       x => x.OrderDetails);
            foreach (var res in result)
            {
                foreach (var r in res.OrderDetails)
                {
                    var list = _mapper.Map<GetOrderDetailsDto>(r);
                    details.Add(list);
                }
            }

            foreach (var r in details.GroupBy(x => x.ProductsID))
            {
                int Qty = 0;
                decimal Total = 0;
                var data = new GetSoldProductsReportDto();
                data.Name = r.FirstOrDefault().Products.Name;
                data.AdditionalCode = r.FirstOrDefault().Products.SKU;
                foreach (var d in r)
                {
                    Qty += d.Quantity;
                    Total += d.GrandTotal;
                }
                data.Quantity = Qty;
                data.Total = Total;
                report.Add(data);
            }
            serviceResponse.Data = report;
            return serviceResponse;
        }

        //public async Task<ServiceResponse<IEnumerable<GetOrdersDto>>> GetOrdersByCustomerAndStore(int storeId, int customersId)
        //{
        //    var serviceResponse = new ServiceResponse<IEnumerable<GetOrdersDto>>();
        //    try
        //    {
        //        var _ordersRepo = _unitOfWork.GetRepository<Orders>();
        //        var result = await _ordersRepo.Search(x=>x.StoreID == storeId && x.CustomersId == customersId);
        //        if (result != null)
        //        {
        //            serviceResponse.Data = _mapper.Map<IEnumerable<GetOrdersDto>>(result);
        //        }
        //        else
        //        {
        //            serviceResponse.Success = false;
        //            serviceResponse.Message = "Order not found.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Success = false;
        //        serviceResponse.Message = ex.GetType().ToString();
        //        _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetOrder)}");
        //    }
        //    return serviceResponse;
        //}
    }
}
