using SourceCode.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class ProcessInstance
    {
        public int ID { get; set; }

        public int ProcID { get; set; }

        public int ProcSetID { get; set; }

        public string FullName { get; set; }

        public string DisplayName { get; set; }

        public string ProcessName { get; set; }

        public string FlowNumber { get; set; }

        public string Folio { get; set; }

        public string Originator { get; set; }

        public string ActivityName { get; set; }

        public string ViewUrl { get; set; }


        public int Priority { get; set; }

        public int ExpectedDuration { get; set; }

        public ProcInstStatus Status { get; set; }

        public byte[] State { get; set; }

        public int Version { get; set; }

        public string StartDate { get; set; }

        public string FinishDate { get; set; }

        public string BOID { get; set; }


        public bool IsSub { get; set; }


        public string OrigProfileID { get; set; }

        public string BOOwner { get; set; }

        public string BOOwnerProfileID { get; set; }


        public string PrevApprovers { get; set; }


        public string TenantID { get; set; }

        private bool mIsChecked = false;

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

        private Dictionary<string, DataField> mDataFields = null;

        public Dictionary<string, DataField> DataFields
        {
            get
            {
                //if (this.mDataFields == null && this.Connection != null)
                //{
                //    this.mDataFields = this.Connection.GetProcInstDataFields(this.ID);
                //}
                return this.mDataFields;
            }
            set
            {
                this.mDataFields = value;
            }
        }

        private Dictionary<string, XmlField> mXmlFields = null;

        public Dictionary<string, XmlField> XmlFields
        {
            get
            {
                //if (this.mDataFields == null && this.Connection != null)
                //{
                //    this.mDataFields = this.Connection.GetProcInstDataFields(this.ID);
                //}
                return this.mXmlFields;
            }
            set
            {
                this.mXmlFields = value;
            }
        }

        //internal Connection Connection { get; set; }
    }

    [Serializable()]
    public enum ProcInstStatus
    {
        Error = 0,
        Running = 1,
        Active = 2,
        Completed = 3,
        Stoped = 4,
        Deleted = 5
    }
}
