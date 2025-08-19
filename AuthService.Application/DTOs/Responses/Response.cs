using System.Net;

namespace AuthService.Application.DTOs.Responses
{
    public class Response
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
    public class Response<T>:Response
    {
        public T Data { get; set; }
        
    }
}
