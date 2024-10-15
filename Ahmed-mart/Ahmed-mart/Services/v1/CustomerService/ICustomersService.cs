using Ahmed_mart.Dtos.v1.CustomersDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.CustomerService
{
    public interface ICustomersService
    {
        Task<ServiceResponse<GetCustomersDto>> AddCustomer(AddCustomersDto addCustomersDto);
        Task<ServiceResponse<GetCustomersDto>> UpdateCustomer(UpdateCustomersDto updateCustomersDto);
        Task<ServiceResponse<IEnumerable<GetCustomersDto>>> GetCustomers();
    }
}
