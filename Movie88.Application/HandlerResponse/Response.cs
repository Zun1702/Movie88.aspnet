namespace Movie88.Application.HandlerResponse
{
    public class Response<T>
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public T? Data { get; set; }

        public Response(string message, int status)
        {
            Message = message;
            Status = status;
        }

        public Response(string message, int status, T? data)
        {
            Message = message;
            Status = status;
            Data = data;
        }
    }

    // Simple response without data
    public class Response
    {
        public string Message { get; set; }
        public int Status { get; set; }

        public Response(string message, int status)
        {
            Message = message;
            Status = status;
        }
    }
}
