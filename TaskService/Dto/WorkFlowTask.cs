using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class WorkFlowTask
    {
        public int ID { get; set; }
        public int ProcInstID { get; set; }
        public string SN { get; set; }
        public string Destination { get; set; }
        public string ActivityName { get; set; }
        public DateTime AssignedDate { get; set; }
        public string DisplayName { get; set; }
        public string Folio { get; set; }
        public DateTime StartDate { get; set; }
    }
}
