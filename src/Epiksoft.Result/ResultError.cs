using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epiksoft.Result
{
    public sealed class ResultError
    {
        public string Message { get; }
        public string Code { get; }

        /// <summary>
        /// constructor to create result error
        /// </summary>
        /// <param name="message">your message  e.g. "email already in use"</param>
        /// <param name="code">your message code e.g. "email_already_in_use"</param>
        public ResultError(string message, string code)
        {
            Message = message;
            Code = code;
        }
    }
}
