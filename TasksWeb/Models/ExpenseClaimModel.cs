using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class ExpenseClaimModel
    {
        public string 流程单号 { get; set; }
        public string 报销月份 { get; set; }
        public string 本位币 { get; set; }
        public string 项目名称 { get; set; }
        public string 说明 { get; set; }

        public List<ExpenseClaimDetailsModel> 详细 { get; set; }
    }

    public class ExpenseClaimDetailsModel
    {
        public string 日期 { get; set; }
        public string 类型 { get; set; }
        public string 描述 { get; set; }
        public string 金额 { get; set; }

    }
}