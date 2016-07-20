using SourceCode.SmartObjects.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmObjConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string smoConnectionString = "Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=False;Host=development.k2software.cn;Port=5555;SecurityLabelName=K2;UserID=Administrator;Password=K2pass!;WindowsDomain=DENALLIX";
            //Integrated=True; IsPrimaryLogin=True; Authenticate=True; EncryptedPassword=False; CachePassword=True;Host=DLX; Port=5555
            //Create a SO Server Client Object
            SmartObjectClientServer soServer = new SmartObjectClientServer();
            try
            {
                //Open the connection to the K2 Server
                soServer.CreateConnection();
                soServer.Connection.Open(smoConnectionString);

                //Get a handle to the 'Employee' SO
                SmartObject soUMUser = soServer.GetSmartObject("ProjectData");

                //Call the GetList Method
                //soUMUser.MethodToExecute = "Get_Role_Users";
                //soUMUser.MethodToExecute = "Get_Users";
                soUMUser.MethodToExecute = "GetList";
                //Input Properties setting:
                //soUMUser.ListMethods["Get_Role_Users"].InputProperties["Role_Name"].Value = "tester";
                //soUMUser.ListMethods["Get_Group_Users"].InputProperties["Group_Name"].Value = "Domain Admins";
                //soUMUser.ListMethods["Get_Group_Users"].InputProperties["LabelName"].Value = "K2";
                //Execute GetList Method, and put the result to a SmartObjectList

                SmartObjectList smoList = soServer.ExecuteList(soUMUser);

                //Iterate the SmartObject List
                foreach (SmartObject soDetail in smoList.SmartObjectsList)
                {
                    Console.WriteLine("ID:" + soDetail.Properties["ID"].Value.ToString());
                }


                soServer.Connection.Close();

                Console.ReadKey();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
    }
}
