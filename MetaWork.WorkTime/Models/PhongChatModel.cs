using HtmlAgilityPack;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;

namespace MetaWork.WorkTime.Models
{
    public class PhongChatModel
    {
        PhongChatProvider _manager;
        public PhongChatModel()
        {
            _manager = new PhongChatProvider();
        }
        #region Get       
       

        public PhongChatViewModel GetInfoOfChannel(int phongChatId)
        {
            var vm = _manager.GetById(phongChatId);
            if (vm != null)
            {
                vm.CountNguoiDung = _manager.CountNguoiDungBy(phongChatId);
                FileProvider fileM = new FileProvider();
                vm.CountFile = fileM.CountFileBy(phongChatId.ToString(), 2);
            }
            return vm;
        }
        public PhongChatViewModel GetShortInfoOfChannel(int phongChatId)
        {
            var vm = _manager.GetById(phongChatId);
            if (vm != null)
            {
                vm.PhongChatInfos = _manager.GetPhongChatInfosBy(phongChatId, "");
                vm.HoTenNguoiDungs = _manager.GetHoTenNguoiDungsBy(phongChatId);
            }
            return vm;
        }
        
        public PhongChatViewModel GetAllOfThread(int threadId, Guid nguoiDungId)
        {
            var phongChat = _manager.GetById(threadId);
            if (phongChat != null)
            {
                phongChat.Messages = GroupMessage(_manager.GetMessagesByDiaChiNhan(threadId.ToString(), 0, 0, nguoiDungId));
            }
            return phongChat;
        }
        public List<PhongChatViewModel> GetBy(int phongChatId, Guid userId)
        {
            List<PhongChatViewModel> lstToReturn = new List<PhongChatViewModel>();
            var pc = _manager.GetById(phongChatId);
            if (pc != null && pc.Type == 2)
            {
                var pcs = new List<PhongChatViewModel>();
                MessageProvider messmanager = new MessageProvider();
                if (pc.KhoaChaId == null)
                    pcs = _manager.GetByKhoaChaId(phongChatId);
                else pcs.Add(pc);
                foreach (var item in pcs)
                {
                    if (item.Count > 0)
                    {
                        if (item.Count < 5)
                        {
                            item.Messages = GroupMessage(messmanager.GetsBy(item.PhongChatId, 0, 0));
                        }
                        else
                        {
                            item.Messages = GroupMessage(messmanager.GetsBy(item.PhongChatId, 1, 1));
                            item.Messages.AddRange(GroupMessage(messmanager.GetsBy(item.PhongChatId, 2, item.Count - 2)));
                        }
                        item.NgayTao = item.Messages[item.Messages.Count - 1].NgayTao;
                        item.User = userId;
                        lstToReturn.Add(item);
                    }

                }
            }
            try
            {
                return lstToReturn.Where(t => t.Count > 0).OrderBy(t => t.NgayTao).ToList();
            }
            catch
            {
                return null;
            }


        }
        public List<FileViewModel> GetFileOfChannel(int phongChatId, byte? fileType, string filecha)
        {
            FileProvider fileM = new FileProvider();
            var fileCha = Guid.Empty;
            if (!string.IsNullOrEmpty(filecha)) Guid.TryParse(filecha, out fileCha);
            var files = fileM.GetFileBy(phongChatId.ToString(), 2, fileType, fileCha);
            return files;
        }
        private List<MessageViewModel> GroupMessage(List<MessageViewModel> mess)
        {
            List<MessageViewModel> lstToReturn = new List<MessageViewModel>();
            if (mess != null && mess.Count > 0)
            {
                Guid user = Guid.Empty;
                DateTime date = new DateTime(1990, 1, 1);
                MessageViewModel mes = new MessageViewModel();
                foreach (var m in mess)
                {
                    if (mes.MessageId == Guid.Empty)
                    {
                        mes = m;
                        mes.Messages = new List<MessageViewModel>() { m };
                        user = m.NguoiGuiId;
                        date = m.NgayTao;
                    }
                    else
                    {
                        if (user == m.NguoiGuiId)
                        {
                            if (date.AddMinutes(3) < m.NgayTao)
                            {
                                lstToReturn.Add(mes);
                                mes = m;
                                mes.Messages = new List<MessageViewModel>() { m };
                                date = m.NgayTao;
                            }
                            else
                            {
                                mes.Messages.Add(m);
                            }
                        }
                        else
                        {
                            lstToReturn.Add(mes);
                            mes = m;
                            mes.Messages = new List<MessageViewModel>() { m };
                            user = m.NguoiGuiId;
                            date = m.NgayTao;
                        }
                    }
                }
                lstToReturn.Add(mes);
            }
            return lstToReturn;
        }

        public PhongChatInfoViewModel GetCalendarInfoBy(int phongChatId)
        {
            return _manager.GetInfoCalendarBy(phongChatId);
        }
        public PhongChatViewModel GetByThreadId(int threadId)
        {
            var result = _manager.GetById2(threadId);
            if (result != null)
            {
                MessageProvider messmanager = new MessageProvider();
                result.Messages = GroupMessage(messmanager.GetsBy(threadId, 0, 0));
                if (result.Messages != null && result.Messages.Count > 0)
                {
                    result.NgayTao = result.Messages[result.Messages.Count - 1].NgayTao;
                }
            }
            return result;
        }       
        #endregion
        #region Check
        public bool CheckIsExistChannel(string channelName, int channelId)
        {
            return _manager.IsExistChannelName(channelName, channelId);
        }
        #endregion
        #region insertOrUpdate
        public ResultViewModel AddChannel(string title, List<Guid> userIds, string calendarId, string embedCalendar, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            if (_manager.IsExistChannelName(title, 0))
            {
                result.Message = "Tên channel đã tồn tại.";
            }
            else
            {
                var phongChatId = _manager.InsertPhongChat(title, userId, null, 2, "", 0, title);
                if (phongChatId > 0)
                {
                    if (userIds != null && userIds.Count > 0)
                    {
                        foreach (var user in userIds)
                        {
                            if (user != Guid.Empty)
                                _manager.InsertLienKetPhongChat(phongChatId, user);
                        }
                    }
                    if (userIds == null || !userIds.Contains(userId)) _manager.InsertLienKetPhongChat(phongChatId, userId);
                    _manager.InsertPhongChatInfo(phongChatId, "LINKCALENDAR", embedCalendar, calendarId, 1, userId);
                    result.Status = true;
                }
                else
                {
                    result.Message = "Lỗi.";
                }
                result.ItemId = phongChatId.ToString();
            }

            return result;
        }

        public ResultViewModel UpdateChannel(int phongChatId, string title, List<Guid> userIds, string calendarId, string embedCalendar, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            if (_manager.IsExistChannelName(title, phongChatId))
            {
                result.Message = "Tên channel đã tồn tại.";
            }
            else
            {
                if (_manager.UpdatePhongChat(phongChatId, title, "", 0, title))
                {
                    _manager.InsertLienKetPhongChat(phongChatId, userIds);
                    if (userIds == null || !userIds.Contains(userId)) _manager.InsertLienKetPhongChat(phongChatId, userId);
                    _manager.UpdatePhongChatInfo(phongChatId, "LINKCALENDAR", embedCalendar, calendarId);
                    result.Status = true;
                }
                else
                {
                    result.Message = "Lỗi.";
                }
                result.ItemId = phongChatId.ToString();
            }

            return result;
        }
        public int InsertThreadByUpdateFile(Guid fileId, int phongChatId, Guid nguoiDungId)
        {
            var newThreadId = _manager.InsertPhongChat("", nguoiDungId, phongChatId, 3, null, 0, null);
            if (newThreadId > 0)
            {
                string title = "Update File name";
                FileProvider fileM = new FileProvider();
                var file = fileM.GetById(fileId);
                var filePath = file.FilePath;
                var fileName = filePath.Substring(filePath.LastIndexOf('/') + 1);
                var ext = fileName.Substring(fileName.LastIndexOf('.') + 1);
                List<string> fileAnh = new List<string>() { "BMP", "JPG", "PNG" };
                string result = "";
                if (fileAnh.Contains(ext.ToUpper()))
                {
                    result = "<p name=\"" + fileName + "\" class=\"UploadFileLocal\" id=\"" + fileId + "_" + ext + "\"><img src=\"" + filePath + "\"  width=\"300\" height=\"300\" </p>";
                }
                else
                {
                    result = "<p name=\"" + fileName + "\"   class=\" UploadFileLocal\" id=\"" + fileId + "_" + ext + "\"><a href=\"" + filePath + " \" title=\"" + file.FileName + "\"  >" + file.FileName + "</a></p>";
                }
                InsertMessage(nguoiDungId, phongChatId, 2, "", title);
            }
            return newThreadId;
        }


        //type=1: text; type=2: Input File,type=3: Create task,type=4:Create Meetingnote
        public int InsertThread(Guid nguoiDungId, int phongChatId, byte type, string content)
        {
            var newThreadId = _manager.InsertPhongChat("", nguoiDungId, phongChatId, 3, null, 0, null);
            if (newThreadId > 0)
            {
                string title = "";
                switch (type)
                {
                    case 2:
                        title = "Input File";
                        break;
                    case 3:
                        title = "Create Task";
                        break;
                    case 4:
                        title = "Create MeetingNote";
                        break;
                }
                InsertMessage(nguoiDungId, phongChatId, type, content, title);

            }
            return newThreadId;
        }
        public Guid InsertMessage(Guid nguoiDungId, int phongChatId, byte type, string content, string title)
        {
            MessageProvider messangeP = new MessageProvider();
            var messageId = messangeP.InsertMessage(nguoiDungId, phongChatId.ToString(), type, content, title);
            if (messageId != null && messageId != Guid.Empty)
            {
                var userIds = _manager.GetNguoiDungIdsBy(phongChatId);
                if (userIds != null && userIds.Count > 0)
                {
                    if (userIds.Contains(nguoiDungId)) userIds.Remove(nguoiDungId);
                    messangeP.InsertLienKetMassage(messageId, userIds);
                }
            }
            return messageId;
        }
        public Guid InsertFile(Guid nguoiDungId, string fileName, string filePath, byte fileType, string itemId, byte itemType)
        {
            FileProvider manager = new FileProvider();
            var newFile = manager.Insert(fileName, filePath, fileType, nguoiDungId);
            if (newFile != Guid.Empty)
            {
                manager.InsertLienKetFile(newFile, itemId, itemType);
            }
            return newFile;
        }
        public List<PhongChatViewModel> GetsByNguoiDungId(Guid nguoiDungId)
        {
            MessageProvider mm = new MessageProvider();
            var lstToReturn = _manager.GetsByNguoiDungId(nguoiDungId);
            if (lstToReturn != null && lstToReturn.Count > 0)
            {
                foreach (var pc in lstToReturn)
                {
                    pc.CountUnRead = mm.CountUnReadPhongChatThread(nguoiDungId, pc.PhongChatId);
                }
            }
            return lstToReturn;
        }

        #endregion











        #region Delete
        public bool DeleteMessage(Guid messageId, Guid nguoiDungId)
        {
            MessageProvider messM = new MessageProvider();
            var mess = messM.GetById(messageId);
            if (mess.NguoiGuiId == nguoiDungId)
            {
                return messM.DeleteMessage(messageId);
            }
            return false;
        }
        public bool DeleteFile(Guid fileId, Guid nguoiDungId)
        {
            FileProvider fileM = new FileProvider();
            return fileM.DeleteFile(fileId);
        }
        public bool DeleteNguoiThamGia(int phongChatId, Guid nguoiDungId)
        {
           return _manager.DeleteLienKetNguoiDungPhongChat(phongChatId, nguoiDungId);
        }
        #endregion
        #region method
        public string ProcessingMessage(string text, int phongChatId, Guid nguoiDungId, string path)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            checkToInsertFile(doc.DocumentNode.ChildNodes, phongChatId, nguoiDungId, path);
            return text.Replace("ImportNewFile", "UploadFileLocal").Replace("ImportNewDocumentLink", "UploadDocumentLink");
        }
        private void checkToInsertFile(HtmlNodeCollection nodes, int phongChatId, Guid nguoiDungId, string path)
        {
            FileProvider filemanager = new FileProvider();
            PhongChatProvider pcmanager = new PhongChatProvider();
            foreach (var element in nodes)
            {
                var newFileId = Guid.Empty;
                if (element.HasClass("ImportNewFile"))
                {
                    var id = element.Id;
                    var name = element.Attributes["name"].Value;
                    newFileId = filemanager.Insert(name, path + id.Replace("_", "."), 1, nguoiDungId);
                }
                else if (element.HasClass("ImportNewDocumentLink"))
                {
                    var link = element.Id;
                    var title = element.Attributes["name"].Value;
                    newFileId = filemanager.Insert(title, link, 2, nguoiDungId);
                }
                else if (element.HasClass("ImportNewFileLink"))
                {
                    var link = element.Id;
                    var title = element.Attributes["name"].Value;
                    newFileId = filemanager.Insert(title, link, 3, nguoiDungId);
                }
                if (newFileId != Guid.Empty)
                {
                    List<int> lstPhongChat = new List<int>() { phongChatId };
                    var pc = pcmanager.GetById(phongChatId);
                    if (pc.KhoaChaId > 0) lstPhongChat.Add(pc.KhoaChaId.Value);
                    foreach (var item in lstPhongChat)
                    {
                        filemanager.InsertLienKetFile(newFileId, item.ToString(), 2);
                    }
                }
                if (element.ChildNodes != null && element.ChildNodes.Count > 0) checkToInsertFile(element.ChildNodes, phongChatId, nguoiDungId, path);
            }
        }

        #endregion
    }
}