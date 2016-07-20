using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class WorkItemArgs
    {
        public TaskModel Task { get; set; }
        public List<TaskUser> Users { get; set; }
        public string[] Actions { get; set; }
    }
}