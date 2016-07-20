using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using TasksWeb.Models;
using System.Data;
namespace TasksWeb.Hubs
{
    public class TasksHub : Hub
    {
        //[HubName("TasksHub")]
        //static List<CurrentUser> ConnectedUsers = new List<CurrentUser>();
        //static DataTable dt = new DataTable();
        //public void Hello()
        //{
        //    Clients.All.hello();
        //}


        //public void SendTaks(string userId)
        //{
        //    var id = Context.ConnectionId;
        //    if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
        //    {
        //        ConnectedUsers.Add(new CurrentUser
        //        {
        //            ConnectionId = id,
        //            UserID = userId,
        //        });
        //        Clients.Caller.onConnected(id, userId);
        //        //Clients.AllExcept(id).onNewUserConnected(id, userID);

        //        Clients.Client(id).onNewUserConnected(id, userID);
        //    }
        //    else
        //    {

        //        Clients.Caller.onConnected(id, userID);
        //        Clients.Client(id).onExistUserConnected(id, userID);
        //        // Clients.AllExcept(id).onExistUserConnected(id, userID);
        //    }
        //}

        ////public void SendChat(string id, string name);

        //public void Connect(string url, string userID)
        //{
        //    var id = Context.ConnectionId;
        //    if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
        //    {
        //        ConnectedUsers.Add(new CurrentUser
        //        {
        //            ConnectionId = id,
        //            UserID = userID,
        //        });
        //        Clients.Caller.onConnected(id, userID, url);
        //        //Clients.AllExcept(id).onNewUserConnected(id, userID);

        //        Clients.Client(id).onNewUserConnected(id, userID);
        //    }
        //    else
        //    {

        //        Clients.Caller.onConnected(id, userID, url);
        //        Clients.Client(id).onExistUserConnected(id, userID);
        //        // Clients.AllExcept(id).onExistUserConnected(id, userID);
        //    }
        //}

        ///// <summary>
        ///// 登出
        ///// </summary>
        //public void Exit(string userID, bool stop)
        //{
        //    var id = Context.ConnectionId;

        //    OnDisconnected(stop);
        //    Clients.Caller.onConnected(id, userID, "");
        //    Clients.Client(id).onExit(id, userID);
        //}

        ///// <summary>
        ///// 断开
        ///// </summary>
        ///// <returns></returns>
        //public override System.Threading.Tasks.Task OnDisconnected(bool stop)
        //{
        //    var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        //    if (item != null)
        //    {
        //        ConnectedUsers.Remove(item);

        //        var id = Context.ConnectionId;
        //        Clients.All.onUserDisconnected(id, item.UserID);

        //    }
        //    return base.OnDisconnected(stop);
        //}

    }
}