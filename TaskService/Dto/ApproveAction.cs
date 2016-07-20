using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class ApproveAction
    {
        public string Name { get; set; }

        public string MetaData { get; set; }

        //
        //public WorklistItem WorklistItem { get; set; }

        public void Execute(string comment)
        {
            this.Execute(comment, true);
        }

        public void Execute(string comment, bool sync)
        {

            //List<DataField> dataFields = new List<DataField>();
            //if (this.WorklistItem.mProcessInstance != null)
            //{
            //    dataFields = this.WorklistItem.mProcessInstance.DataFields.Values.Where(p => p.IsChanged == true).ToList<DataField>();
            //}
            //this.WorklistItem.Connection.ExecuteAction(this.WorklistItem.SN, dataFields, this.Name, comment, sync);
        }
    }
}
