using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class OANewsChangeModel
    {
        public string username { get; set; }
        public string token { get; set; }
        public OANewsChangeDataModel data { get; set; }
    }

    public class OANewsChangeDataModel
    {
        public string msgId { get; set; }
        public int newStatus { get; set; }
    }

    public class OANewsChangeRequestModel
    {
        public int status { get; set; }
    }
}