using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class TravelRequestModel
    {
        public string 流程单号 { get; set; }
        public string 差旅类型 { get; set; }
        public string 本位币 { get; set; }
        public string 外出日期 { get; set; }
        public string 返回日期 { get; set; }
        public string 出发地 { get; set; }
        public string 目的地 { get; set; }
        public string 事由 { get; set; }
        public string 总计金额 { get; set; }
        public List<TravelRequestDetailsModel> 详细 { get; set; }
    }

    public class TravelRequestDetailsModel
    {
        public string 类型 { get; set; }
        public string 描述 { get; set; }
        public string 金额 { get; set; }
    }
}