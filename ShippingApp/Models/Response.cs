namespace ShippingApp.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public bool IsSuccess { get; set; }
    }
}
