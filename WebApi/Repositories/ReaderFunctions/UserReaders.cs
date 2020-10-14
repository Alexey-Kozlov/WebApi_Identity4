using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using WebApi.DataBaseHelpers;
using WebApi.Models;

namespace WebApi.Repositories.ReaderFunctions
{
    public class UserReaders
    {
        public UserSession ReadUserSession(IDataReader reader)
        {
            var user = new UserSession
            {
                UserName = reader.GetValue<string>("UserName"),
                SessionId = reader.GetValue<int>("SessionId"),
                UserId = reader.GetValue<int>("UserId"),
                LoginName = reader.GetValue<string>("LoginName")
            };
            return user;
        }
    }
}
