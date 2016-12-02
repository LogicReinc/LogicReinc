using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.API
{
    public class APIWrap
    {
        public bool Success { get; set; }
        public object Result { get; set; }

        public ExceptionData Exception { get; set; }

        public APIWrap() { }

        public APIWrap(object val)
        {
            Success = true;
            Result = val;
            Exception = null;
        }
        public APIWrap(Exception ex)
        {
            Success = false;
            Result = null;
            Exception = new ExceptionData(ex);
        }

        public APIWrap<T> As<T>()
        {
            object result = Result;
            if (Result.GetType().IsSubclassOf(typeof(JToken)))
                result = ((JToken)result).ToObject<T>();
            return new APIWrap<T>()
            {
                Exception = Exception,
                Result = (T)result,
                Success = Success
            };
        }
    }
    public class APIWrap<T>
    {
        public bool Success { get; set; }
        public T Result { get; set; }
        public ExceptionData Exception { get; set; }

        public APIWrap() { }
        public APIWrap(T val)
        {
            Success = true;
            Result = val;
            Exception = null;
        }
        public APIWrap(Exception ex)
        {
            Success = false;
            Result = default(T);
            Exception = new ExceptionData(ex);
        }
    }

    public class ExceptionData
    {
        public string Type { get; set; }
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public ExceptionData() {}
        public ExceptionData(Exception ex)
        {
            Type = ex.GetType().Name;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
        }

        public ExceptionData Safe()
        {
            StackTrace = "";
            return this;
        }
    }
}
