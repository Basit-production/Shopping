using Ahmed_mart.Dtos.v1.StudentDto;
using Ahmed_mart.Services.v1.StudentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ahmed_mart.Controllers.v1
{
    //[ServiceFilter(typeof(AccessFilterAttribute))]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [EnableRateLimiting("fixed")]
    public class StudentController : BaseController
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET /GetAllAsync
        [MapToApiVersion("1.0")]
        [HttpGet("GetAllAsync")]
        public async Task<IActionResult> GetAllAsync()
        {
            var serviceResponse = await HandleServiceResponseAsync(_studentService.GetAllAsync());
            return serviceResponse;
        }

        // GET /GetByIdAsync/{id}
        [MapToApiVersion("1.0")]
        [HttpGet("GetByIdAsync/{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_studentService.GetByIdAsync(id));
            return serviceResponse;
        }

        // POST /AddAsync
        [MapToApiVersion("1.0")]
        [HttpPost("AddAsync")]
        public async Task<IActionResult> AddAsync([FromForm] AddStudentDto addStudentDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_studentService.AddAsync(addStudentDto));
            return serviceResponse;
        }

        // PUT /UpdateAsync
        [MapToApiVersion("2.0")]
        [HttpPut("UpdateAsync")]
        public async Task<IActionResult> UpdateAsync([FromForm] UpdateStudentDto updateStudentDto)
        {
            var serviceResponse = await HandleServiceResponseAsync(_studentService.UpdateAsync(updateStudentDto));
            return serviceResponse;
        }

        // DELETE /DeleteAsync/{id}
        [MapToApiVersion("2.0")]
        [HttpDelete("DeleteAsync/{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var serviceResponse = await HandleServiceResponseAsync(_studentService.DeleteAsync(id));
            return serviceResponse;
        }
    }
}
