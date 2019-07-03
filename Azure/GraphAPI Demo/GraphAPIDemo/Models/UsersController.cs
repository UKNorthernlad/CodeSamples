using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication6.Models
{
    public class User
    {
        public string userPrincipalName;
        public string displayName;
        public string objectId;
    }
    public class Users
    {
        public IEnumerable<User> value;
    };

}