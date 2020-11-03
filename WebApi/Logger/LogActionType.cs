using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Logger
{
    public enum LogActionType
    {
        Request,
        Response,
        Warning,
        Error,
        Fatal,
        Debug
    }
}
