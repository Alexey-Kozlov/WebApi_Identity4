using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Filters
{
    public class MessageException : Exception
    {
        public MessageException()
        {

        }

        public MessageException(string message) : base(message)
        {

        }

        public MessageException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
