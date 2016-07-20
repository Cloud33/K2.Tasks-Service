using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TasksWeb.Controllers
{
    public class QuanShiController : ApiController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string QuanShiOASendChangeAPIUrl = ConfigurationManager.AppSettings["QuanShiOASendChangeAPIUrl"];

        // POST api/<controller>
        [HttpPost]
        [ActionName("PostTask")]
        public void PostTask()
        {

        }
    }
}