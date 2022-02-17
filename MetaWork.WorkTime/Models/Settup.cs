using MetaWork.Data.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaWork.WorkTime.Models
{
    public class Settup
    {
        public void SettupChannel()
        {
            //
            var ad1 = Guid.Parse("345515cc-5385-4654-ad4a-0d1279ee07b4");
            var ad2 = Guid.Parse("1ea0b760-36b2-4d2f-9e88-a16cc7f5ac68");
            DuAnProvider duAnM = new DuAnProvider();
            NguoiDungProvider ndM = new NguoiDungProvider();
            var projectActives = duAnM.GetsByTrangThaiDuAn( 2 );
            if (projectActives != null && projectActives.Count > 0)
            {
                PhongChatProvider pcM = new PhongChatProvider();
                foreach(var project in projectActives)
                {

                    if (!string.IsNullOrEmpty(project.TenVietTat) && !pcM.IsExistChannelName(project.TenVietTat, 0))
                    {
                        var newpcId= pcM.InsertPhongChat(project.TenDuAn, project.NguoiQuanLyId, null, 2, project.DuAnId.ToString(), 1, project.TenVietTat);
                        if (newpcId > 0)
                        {
                            var lstUserId = ndM.GetIdsBy(null, new List<int> { project.DuAnId });
                            if (lstUserId != null && lstUserId.Count > 0)
                            {
                                foreach (var userId in lstUserId)
                                {
                                    pcM.InsertLienKetPhongChat(newpcId, userId);
                                }
                                if (!lstUserId.Contains(project.NguoiQuanLyId)) pcM.InsertLienKetPhongChat(newpcId, project.NguoiQuanLyId);
                                if (!lstUserId.Contains(ad1)) pcM.InsertLienKetPhongChat(newpcId, ad1);
                                if (!lstUserId.Contains(ad2)) pcM.InsertLienKetPhongChat(newpcId, ad2);
                            }
                            else
                            {
                                pcM.InsertLienKetPhongChat(newpcId, project.NguoiQuanLyId);
                                pcM.InsertLienKetPhongChat(newpcId, ad1);
                                pcM.InsertLienKetPhongChat(newpcId, ad2);
                            }
                        }
                    }
                  
                }
            }
        }
    }
}