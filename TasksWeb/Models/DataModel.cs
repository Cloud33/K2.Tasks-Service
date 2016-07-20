using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class DataModel
    {
        public RequestStatus status { get; set; }
        public string message { get; set; }

        public object data { get; set; }
    }
    public enum RequestStatus
    {
        Success = 1,
        Error = 2,
        Unknown = 3
    }
}