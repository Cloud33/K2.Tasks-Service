using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tasks.Service;
using TasksWeb.Models;
using TasksWeb.Help;
using Newtonsoft.Json;
using System.Configuration;

namespace TaskServiceTest
{
    [TestClass]
    public class TasksServiceTest
    {
        private static readonly string QuanShiOASendChangeAPIUrl = ConfigurationManager.AppSettings["QuanShiOASendChangeAPIUrl"];
        [TestMethod]
        public void TestMethod1()
        {
            TasksService task = new TasksService();
            var obj = task.GetTasks();
            //Assert.AreEqual();

        }
        [TestMethod]
        public void GetWorklistItemsByPrototypeAPI()
        {

            //var obj = WorkflowClientService.GetWorklistItems('K2:',);
        }

        [TestMethod]
        public void TaskPushTest()
        {
            //WorkItemArgs re = new WorkItemArgs();
            //re = JsonConvert.DeserializeObject<WorkItemArgs>(@"{'Task':{'ProcInstID':526,'ActInstDestID':0,'Folio':'Test20160127014','Originator':'DENALLIX\\administrator','ActivityName':'Queue','ProcessName':'Test','Destination':null,'Data':'http://demo.k2software.cn:82/Workspace/ClientEventPages/KStarWorkflow/Test_Page_9e231467.aspx?SN=526_14','ParamsTable':'{}','State':'k2'},'Users':[{'Name':'DENALLIX\\Administrator','Email':'administrator@denallix.com','Manager':'DENALLIX\\Jonno'},{'Name':'DENALLIX\\Anthony','Email':'Anthony@denallix.com','Manager':'DENALLIX\\Bob'},{'Name':'DENALLIX\\Jonno','Email':'Jonno@denallix.com','Manager':'DENALLIX\\Mark'},{'Name':'DENALLIX\\Bob','Email':'Bob@denallix.com','Manager':'DENALLIX\\Brandon'},{'Name':'DENALLIX\\Codi','Email':'Codi@denallix.com','Manager':'DENALLIX\\John'},{'Name':'DENALLIX\\Mike','Email':'Mike@denallix.com','Manager':'DENALLIX\\Anthony'}],'Actions':['OK']}");
            //foreach (var item in re.Users)
            //{
            //    var task = re.Task;
            //    task.Destination = item.Name;
            //    TaskPush.SaveTask(task);
            //}
        }
        [TestMethod]
        public void QuanShiPush()
        {
            TaskDto task = new TaskDto()
            {
                ID = 12,
                ActivityName = "主管领导审批",
                Data = @"/ExpenseClaim/Expense/Index?_FormId=568&SN=568_17",
                Destination = @"DENALLIX\jackf",
                Folio = "EC-201601280030",
                Originator = @"DENALLIX\Anthony",
                ParamsTable = "{'IsUsingCar':false,'FormId':568}",
                ProcessName = "ExpenseClaim",
                ProcInstID = 519,
                State = "K2",
                SN = "568_17",
                SendTime = DateTime.Now
            };
            try
            {
                TaskPush.QuanShiPush(task, new string[] { "同意", "拒绝" }, null);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}
