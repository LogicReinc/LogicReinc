using LogicReinc.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Exceptions
{
    public class APIException : Exception
    {
        public ExceptionData ExData { get; set; }
        public APIException(ExceptionData data) : base($"{data.Type}: {data.Message}")
        {
            ExData = data;
        }
    }
}
