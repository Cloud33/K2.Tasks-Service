using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Tasks.Service
{
    public class TasksService
    {
        private const string strConn = "server=.;database=K2Task;uid=sa;pwd=123456";
        public DataTable GetTasks()
        {
            string mysql = "select * from [dbo].[Tasks]";
            using (SqlConnection myconnection = new SqlConnection(strConn))
            {
                SqlDataAdapter sqlda = new SqlDataAdapter(mysql, strConn);
                DataSet ds = new DataSet();
                sqlda.Fill(ds, "Tasks");
                return ds.Tables[0];
            }
        }
    }
}
