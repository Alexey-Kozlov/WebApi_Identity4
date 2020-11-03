using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Logger
{
    public class LogLessAttribute : Attribute
    {
        public List<string> Properties = new List<string>();

        public LogLessAttribute(params string[] args)
        {
            Properties.AddRange(args);
        }
    }
}
