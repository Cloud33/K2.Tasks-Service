using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class WorklistItem
    {
        public Int64 ID { get; set; }

        public int ProcInstID { get; set; }

        public int ActInstDestID { get; set; }

        public int ActInstID { get; set; }

        public int ProcID { get; set; }

        public int ActID { get; set; }

        public int EventID { get; set; }

        public string Destination { get; set; }

        public string AssignedDate { get; set; }

        public string StartDate { get; set; }

        public string FinishDate { get; set; }

        public WorklistStatus Status { get; set; }


        public string TenantID { get; set; }

        public string ActivityName { get; set; }

        public string ActivityDispName { get; set; }

        public string FullName { get; set; }

        public string ProcDispName { get; set; }

        public string Originator { get; set; }

        public string Folio { get; set; }

        public string FlowNumber { get; set; }


        public string ProfileID { get; set; }


        public ProcInstStatus ProcInstStatus { get; set; }


        public string ProcStartDate { get; set; }


        public string SN { get; set; }


        public string Data { get; set; }

        private bool mIsChecked = false;

        //public ICollection<BusinessDataItem> BusinessData { get; set; }

        public bool IsChecked
        {
            get
            {
                return this.mIsChecked;
            }
            set
            {
                this.mIsChecked = value;
            }
        }


        /*
        private Dictionary<string, ApproveAction> mDicActions = new Dictionary<string, ApproveAction>();
        
        public Dictionary<string, ApproveAction> Actions
        {
            get
            {
                return this.mDicActions;
            }
            internal set
            {
                this.mDicActions = value;
            }
        }

        */
        private Actions mActions = new Actions();

        public Actions Actions
        {
            get
            {
                return this.mActions;
            }
            internal set
            {
                this.mActions = value;
            }
        }


        internal ProcessInstance mProcessInstance = null;
        public ProcessInstance ProcessInstance
        {
            get
            {
                //if (this.mProcessInstance == null && this.Connection != null)
                //{
                //    this.mProcessInstance = this.Connection.GetProcessInstanceFromOrig(this.ProcInstID); //ProcessInstanceDA.GetProcessInstanceFromOrig(this.ProcInstID);
                //    this.mProcessInstance.Connection = this.Connection;
                //}
                return this.mProcessInstance;
            }
            set
            {
                this.mProcessInstance = value;
            }
        }


        public Dictionary<string, object> ActivityDataFields
        {
            get;
            set;
        }


    }

    [Serializable()]
    public enum WorklistStatus
    {
        Available = 0,
        Allocated = 1,
        Open = 2,
        Sleep = 3,
        Completed = 4
    }

    [Serializable()]
    public enum PlatformType
    {
        ASP = 0
    }
}
