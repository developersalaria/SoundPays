using System;
using System.Collections.Generic;
using System.Text;

namespace SoundpaysAdd.Core.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data,string message = null, bool succeeded = true)
        {
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }
        public Response(string message, bool succeeded = false, List<string> errors = null)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = errors;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }
    }
}
