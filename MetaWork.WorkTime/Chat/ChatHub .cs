using HtmlAgilityPack;
using MetaWork.Data.Provider;
using MetaWork.WorkTime.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace MetaWork.WorkTime.Chat
{

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public void SendChatMessage(string to, string message)
        {
            if (string.IsNullOrEmpty(to)) return; 
            var userName = Context.User.Identity.Name;
            NguoiDungModel ndM = new NguoiDungModel();
            var user = ndM.GetNguoiDungByUserName(userName);
            ConnectionProvider manager = new ConnectionProvider();
            int id = 0;
            int.TryParse(to, out id);
            byte type = 1;
            if (id == 0) type = 2;
            // Save message:
            MessageModel model = new MessageModel();
            message = FormatUrls(message);
            var messageId= model.Insert(user.NguoiDungId, to, type, message);
            List<Guid> nguoiDungIds = new List<Guid>();
            if (id > 0)
            {
                PhongChatProvider pc = new PhongChatProvider();
                nguoiDungIds = pc.GetNguoiDungIdsBy(id);
            }
            else
            {
                nguoiDungIds.Add(Guid.Parse(to));
                if (!nguoiDungIds.Contains(user.NguoiDungId)) nguoiDungIds.Add(user.NguoiDungId);
            }          
            model.AddLienKet(messageId, nguoiDungIds);

            foreach(var nguoiDungId in nguoiDungIds)
            {
                var connects = manager.GetConnectionByUserId(nguoiDungId);
                foreach (var connection in connects)
                {
                    Clients.Client(connection.ConnectionId).broadcastMessage(1, messageId,to, user.HoTen,user.Avatar,DateTime.Now.ToString("t"),message);
                }
            }                    
        }
        public void sendChatMessageThread(string message,int threadId)
        {
            if (threadId == 0) return;
            var userName = Context.User.Identity.Name;
            NguoiDungModel ndM = new NguoiDungModel();
            var user = ndM.GetNguoiDungByUserName(userName);
            PhongChatProvider pc = new PhongChatProvider();
            var phongchat = pc.GetById2(threadId);
            ConnectionProvider manager = new ConnectionProvider();
            PhongChatModel pcm = new PhongChatModel();
            
            var path = System.Configuration.ConfigurationManager.AppSettings.Get("HostWeb") + "Uploads/";
            // save file
            var txt = message;
            message = FormatUrls(message);
            message = pcm.ProcessingMessage(message, phongchat.KhoaChaId.Value,user.NguoiDungId,path);
            // Save message:
            MessageModel model = new MessageModel();       
            var messageId = model.Insert(user.NguoiDungId, threadId.ToString(), 2, message);
            List<Guid> nguoiDungIds = new List<Guid>();                       
            nguoiDungIds = pc.GetNguoiDungIdsBy(phongchat.KhoaChaId.Value);        
            model.AddLienKet(messageId, nguoiDungIds);           
            foreach (var nguoiDungId in nguoiDungIds)
            {
                var connects = manager.GetConnectionByUserId(nguoiDungId);
                foreach (var connection in connects)
                {
                    Clients.Client(connection.ConnectionId).broadcastMessageThread(messageId, threadId,user.NguoiDungId, user.HoTen, user.Avatar, DateTime.Now.ToString("HH:mm"), message);
                }
            }
        }

        public void InsertThread(int threadId, int  phongChatId)
        {
            ConnectionProvider manager = new ConnectionProvider();
            PhongChatProvider pc = new PhongChatProvider();          
            List<Guid> nguoiDungIds = pc.GetNguoiDungIdsBy(phongChatId);          
            foreach (var nguoiDungId in nguoiDungIds)
            {
                var connects = manager.GetConnectionByUserId(nguoiDungId);
                foreach (var connection in connects)
                {                    
                    Clients.Client(connection.ConnectionId).insertThread(threadId, phongChatId);
                }
            }
        }

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            NguoiDungModel ndM = new NguoiDungModel();
            ConnectionProvider manager = new ConnectionProvider();
            var user = ndM.GetNguoiDungByUserName(userName);
            manager.InsertOrUpdate(Context.ConnectionId, user.NguoiDungId, true);
            _connections.Add(userName, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            NguoiDungModel ndM = new NguoiDungModel();
            ConnectionProvider manager = new ConnectionProvider();
            var user = ndM.GetNguoiDungByUserName(userName);
            manager.InsertOrUpdate(Context.ConnectionId, user.NguoiDungId, false);
            _connections.Remove(userName, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.Add(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.User.Identity.Name} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.Remove(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.User.Identity.Name} has left the group {groupName}.");
        }
        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
   //     private string ConvertTextUrlToLink(string url)
   //     {
   //         string regex = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[_.a-z0-9-]+\.
   //[a-z0-9\/_:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
   //         Regex r = new Regex(regex, RegexOptions.IgnoreCase);
   //         return r.Replace(url, "a href=\"$1\" title=\"Click here to open in a new window or tab\"target =\"_blank\">$1</a>").Replace("href=\"www", "href=\"http://www");
   //     }

        public  string FormatUrls(string input)
        {
            string result = "";
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            foreach (var element in doc.DocumentNode.ChildNodes)
            {
                if (element.HasClass("ImportNewFile")|| element.InnerHtml.Contains("src") || element.InnerHtml.Contains("href"))
                {
                    result += element.OuterHtml;
                }
                else if (element.HasClass("ImportNewDocumentLink") || element.InnerHtml.Contains("src") || element.InnerHtml.Contains("href"))
                {
                    result += element.OuterHtml;
                }else
                {
                    string output = element.OuterHtml;                    
                    Regex regx = new Regex("http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*([a-zA-Z0-9\\?\\#\\=\\/]){1})?", RegexOptions.IgnoreCase);

                    MatchCollection mactches = regx.Matches(output);

                    foreach (Match match in mactches)
                    {
                        output = output.Replace(match.Value, "<a href='" + match.Value + "' target='blank'>" + match.Value + "</a>");
                    }
                    result += output;
                }
            }
           
            return result;
        }
        public void DeleteMessage(Guid messageId,int phongChatId)
        {
            PhongChatModel model = new PhongChatModel();
            ConnectionProvider manager = new ConnectionProvider();
            var userName = Context.User.Identity.Name;
            NguoiDungModel ndM = new NguoiDungModel();
            var user = ndM.GetNguoiDungByUserName(userName);
            if (model.DeleteMessage(messageId, user.NguoiDungId))
            {
                PhongChatProvider pc = new PhongChatProvider();
                List<Guid> nguoiDungIds = pc.GetNguoiDungIdsBy(phongChatId);
                foreach (var nguoiDungId in nguoiDungIds)
                {                   
                    var connects = manager.GetConnectionByUserId(nguoiDungId);
                    foreach (var connection in connects)
                    {
                        Clients.Client(connection.ConnectionId).deleteMessage(messageId, true);
                    }
                }
            }
            else
            {
                var connects = manager.GetConnectionByUserId(user.NguoiDungId);
                foreach (var connection in connects)
                {
                    Clients.Client(connection.ConnectionId).deleteMessage(messageId, false);
                }
            }
        }
    }

}