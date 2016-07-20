using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class TaskDto
    {
        public int ID { get; set; }
        public int ProcInstID { get; set; }
        public string Folio { get; set; }
        public string Originator { get; set; }
        public string SharedUser { get; set; }
        public string ActivityName { get; set; }
        public string ProcessName { get; set; }
        public string Destination { get; set; }
        public string SN { get; set; }
        public string BusinessData { get; set; }
        public string Data { get; set; }
        public string ParamsTable { get; set; }
        public string Actions { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime StartDate { get; set; }
    }
}