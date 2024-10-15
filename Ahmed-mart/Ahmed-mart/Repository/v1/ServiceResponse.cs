using System.Net;

namespace Ahmed_mart.Repository.v1
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string Message { get; set; } = string.Empty;
        public virtual IList<string> Errors { get; set; } = new List<string>();
    }
}
