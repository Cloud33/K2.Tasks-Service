using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class TasksItem
    {

        /// <summary>
        /// 流程实例ID
        /// </summary>
        public int ProcInstID { get; set; }
        /// <summary>
        /// 流程单号
        /// </summary>
        public string Folio { get; set; }
        /// <summary>
        /// 发起时间
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 当前环节
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string Originator { get; set; }
        //public string OriginatorDisName { get; set; }
        /// <summary>
        /// SN
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// 是否是ShareUser
        /// </summary>
        public string SharedUser { get; set; }
        /// <summary>
        /// 是否已读
        /// </summary>
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

    }
}
