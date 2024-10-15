using Ahmed_mart.Dtos.v1.StudentDto;
using Ahmed_mart.Repository.v1;

namespace Ahmed_mart.Services.v1.StudentService
{
    public interface IStudentService
    {
        Task<ServiceResponse<IEnumerable<GetStudentDto>>> GetAllAsync();
        Task<ServiceResponse<GetStudentDto>> GetByIdAsync(string id);
        Task<ServiceResponse<GetStudentDto>> AddAsync(AddStudentDto addStudentDto);
        Task<ServiceResponse<GetStudentDto>> UpdateAsync(UpdateStudentDto updateStudentDto);
        Task<ServiceResponse<GetStudentDto>> DeleteAsync(string id);
    }
}
