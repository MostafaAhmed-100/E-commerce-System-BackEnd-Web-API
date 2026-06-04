namespace WebApplication1.DTOS.Response_DTOs
{
    public class ApiResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public T? Data { get; set; }
    }
}