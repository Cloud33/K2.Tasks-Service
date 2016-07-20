using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class WorklistTask
    {
        public string ProcInstID { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsReaded { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Folio { get; set; }

        /// <summary>
        /// 流程实例编号
        /// </summary>
        public string ProcInstNo { get; set; }

        /// <summary>
        /// 流程主题
        /// </summary>
        public string ProcSubject { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string Originator { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ProcStartDate { get; set; }
        /// <summary>
        /// 当前环节名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 任务送达时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public DateTime? ProcessTime { get; set; }

        /// <summary>
        /// 过程
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// 过程名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 过程全名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 最后修改的时间
        /// </summary>
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// 最后修改的用户
        /// </summary>
        //  public string LastActivityUser { get; set; }

        /// <summary>
        /// 工作流程步骤
        /// </summary>
        public string WorkflowStep { get; set; }

        //public string Requester { get; set; } //Business Data

        public string HyperLink { get; set; }

        public string ViewFlowUrl { get; set; }

        //public IList<BusinessDataItem> BusinessData { get; set; }
    }
}
