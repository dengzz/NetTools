using System;

namespace NetTool
{
    public class InvokedResult
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public static InvokedResult Ok(string message = null)
        {
            return new InvokedResult { Succeeded = true, Message = message };
        }

        public static InvokedResult<T> Ok<T>(T data, string message)
        {
            return new InvokedResult<T> { Succeeded = true, Data = data, Message = message };
        }

        public static InvokedResult Fail(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            return new InvokedResult { Succeeded = false, Message = message };
        }

        public static InvokedResult<T> Fail<T>(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            return new InvokedResult<T> { Succeeded = false, Message = message };
        }
    }

    public class InvokedResult<T> : InvokedResult
    {
        public T Data { get; set; }
    }
}
