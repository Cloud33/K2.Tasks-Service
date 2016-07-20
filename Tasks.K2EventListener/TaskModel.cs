using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.K2EventListener
{
    public class TaskModel
    {
        public int ProcInstID { get; set; }
        public int ActInstDestID { get; set; }
        public string Folio { get; set; }
        public string Originator { get; set; }
        public string ActivityName { get; set; }
        public string ProcessName { get; set; }
        public string Destination { get; set; }


        /// <summary>
        /// url
        /// </summary>
        public string Data { get; set; }
        public string ParamsTable { get; set; }
        public string State { get; set; }

        public DateTime StartDate { get; set; }
    }

    public class TaskUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Manager { get; set; }
    }
}
