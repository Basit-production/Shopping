namespace Ahmed_mart.Dtos.v1.FileDtos
{
    public class UploadFileDto
    {
        public string Directory { get; set; }
        public IFormFile File { get; set; }
    }
}
