using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class CheckModel
    {
        public string username { get; set; }
        public string token { get; set; }

        public List<CheckDataModel> data { get; set; }
    }
    public class CheckDataModel
    {
        public string account { get; set; }
        public string sessionId { get; set; }
    }
}