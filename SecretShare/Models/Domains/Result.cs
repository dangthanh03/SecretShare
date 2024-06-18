namespace SecretShare.Models.Domains
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        // return a new instance of  Result{T}  with IsSuccess set to true.
        public static Result<T> Success(T data)
        {
            return new Result<T> { IsSuccess = true, Data = data };
        }

        // return a new instance of Result{T} with IsSuccess set to false.
        public static Result<T> Fail(string message)
        {
            return new Result<T> { IsSuccess = false, Message = message };
        }
    }
}
