using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.K2EventListener
{
    public class WorkItemArgs
    {
        public TaskModel Task { get; set; }
        public List<TaskUser> Users { get; set; }
        public string[] Actions { get; set; }
    }
}
