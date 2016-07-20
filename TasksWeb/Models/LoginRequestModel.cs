using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class LoginRequestModel
    {
        public int crrorCode { get; set; }
        public LoginRequestDataModel data { get; set; }
        public string errorMessage { get; set; }
        public string requestId { get; set; }
    }

    public class LoginRequestDataModel
    {
        public string token { get; set; }
        public string username { get; set; }
        public int role { get; set; }
        public int appId { get; set; }
    }
}