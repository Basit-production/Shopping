namespace Ahmed_mart.Dtos.v1.StudentDto
{
    public class GetStudentDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsGraduate { get; set; }
        public string[]? Courses { get; set; }
        public string? FileSize { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Content { get; set; }
        public string? Base64File { get; set; }
    }
}
