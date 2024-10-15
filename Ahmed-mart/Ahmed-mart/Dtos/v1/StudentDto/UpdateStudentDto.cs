using System.ComponentModel.DataAnnotations;

namespace Ahmed_mart.Dtos.v1.StudentDto
{

    public class UpdateStudentDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsGraduate { get; set; }
        public string[]? Courses { get; set; }
        public IFormFile? File { get; set; }
    }
}
