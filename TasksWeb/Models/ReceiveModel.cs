using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    /// <summary>
    /// 审批参数
    /// </summary>
    public class ReceiveModel
    {
        public string ProcInstID { get; set; }
        /// <summary>
        /// SN
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 是否是SharedUser
        /// </summary>
        public string SharedUser { get; set; }
    }
}