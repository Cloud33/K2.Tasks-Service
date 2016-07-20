using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public int role { get; set; }
        public int appId { get; set; }
    }
}