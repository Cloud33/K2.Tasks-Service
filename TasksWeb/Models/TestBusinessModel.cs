using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class TestBusinessModel
    {
        public int 流程实例ID { get; set; }
        public string 流程单号 { get; set; }
        public string 差旅类型 { get; set; }
        public string 出发地 { get; set; }
        public string 目的地 { get; set; }
        public string 项目 { get; set; }

        public string 开始时间 { get; set; }

        public string 结束时间 { get; set; }
        public string 总金额 { get; set; }

        public List<TestBusionessDetailsModel> 差旅详细 { get; set; }
    }
    public class TestBusionessDetailsModel
    {
        public string 费用类型 { get; set; }
        public string 成本中心 { get; set; }
        public string 说明 { get; set; }
        public string 金额 { get; set; }
        //public string 用述 { get; set; }
        public string 币种 { get; set; }
        public string 汇率 { get; set; }


    }
}