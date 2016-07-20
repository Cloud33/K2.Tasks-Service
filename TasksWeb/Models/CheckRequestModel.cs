using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class CheckRequestModel
    {
        public int errorCode { get; set; }
        public List<CheckRequestDataModel> data { get; set; }
        public string errorMessage { get; set; }
        public string requestId { get; set; }

    }

    public class CheckRequestDataModel
    {
        public int errorCode { get; set; }
        public string sessionId { get; set; }
        public string account { get; set; }
        public string displayName { get; set; }
        public string sex { get; set; }
        public string mobile { get; set; }
        public string fixTel { get; set; }
        public string position { get; set; }
        public string email { get; set; }
        public string department { get; set; }

    }
}