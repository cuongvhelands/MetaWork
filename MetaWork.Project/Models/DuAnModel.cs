using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using System.Data;
using RestSharp;

namespace MetaWork.Project.Models
{
    public class DuAnModel
    {
        DuAnProvider _manager;
        GiaiDoanDuAnProvider _giaiDoanM;
        CongViecProvider _congViecM;
        NguoiDungProvider _nguoiDungM;
        TrangThaiDuAnProvider _ttDuAnM;
        public DuAnModel()
        {
            _manager = new DuAnProvider();
            _giaiDoanM = new GiaiDoanDuAnProvider();
            _congViecM = new CongViecProvider();
            _nguoiDungM = new NguoiDungProvider();
            _ttDuAnM = new TrangThaiDuAnProvider();
        }
 
        public ResultViewModel AddProjectFromCoda(List<GiaiDoanDuAnCodaViewModel> lst,Guid userId,List<string> channelNames)
        {
            ResultViewModel result = new ResultViewModel() { Status = true };
            List<DuAnViewModel> lstDAC = new List<DuAnViewModel>();
            List<TrangThaiDuAnViewModel> lsttt = new List<TrangThaiDuAnViewModel>();
            List<LoaiDuAnViewModel> lstloaiDA = new List<LoaiDuAnViewModel>();
            List<NguoiDungViewModel> lstND = new List<NguoiDungViewModel>();
            if (channelNames == null) channelNames = new List<string>();
            foreach (var item in lst)
            {
                if(!string.IsNullOrEmpty(item.Parent)&& !string.IsNullOrEmpty(item.Ma) && !string.IsNullOrEmpty(item.Type))
                {
                    var vm = _manager.GetByMa(item.Ma);
                    if (vm == null || vm.DuAnId == 0)
                    {
                        vm = new DuAnViewModel();
                    }
                    // KhoaChaId
                    if (lstDAC.Count(t => t.TenDuAn.Equals(item.Parent)) == 0)
                    {
                        var dac = _manager.GetDuAnByName(item.Parent);
                        if (dac != null && dac.DuAnId > 0)
                        {
                            lstDAC.Add(dac);
                            vm.KhoaChaId = dac.DuAnId;
                        }
                        else
                        {
                            vm.KhoaChaId = _manager.Insert(new DuAnViewModel() { TenDuAn = item.Parent }, userId);
                            var ducaa = _manager.GetDuAnByName(item.Parent);
                            if (ducaa != null)
                                lstDAC.Add(ducaa);
                        }
                    }
                    else
                    {

                        vm.KhoaChaId = lstDAC.Where(t => t.TenDuAn.Equals(item.Parent)).FirstOrDefault().DuAnId;
                    }
                    //TrangThaiDuAnId
                    if (lsttt.Count(x => x.TenTrangThaiDuAn.Equals(item.Status)) == 0)
                    {
                        var tt = _ttDuAnM.GetByName(item.Status);
                        if (tt != null && tt.TrangThaiDuAnId > 0)
                        {
                            vm.TrangThaiDuAnId = tt.TrangThaiDuAnId;
                            lsttt.Add(tt);
                        }


                        else
                        {
                            if (!string.IsNullOrEmpty(item.Status))
                            {
                                vm.TrangThaiDuAnId = _ttDuAnM.InsertTrangThaiDuAn(new TrangThaiDuAnViewModel() { TenTrangThaiDuAn = item.Status, MaMau = "Info" });
                                var tt2a = _ttDuAnM.GetByName(item.Status);
                                if (tt2a != null)
                                    lsttt.Add(tt2a);
                            }

                        }
                    }
                    else
                    {
                        vm.TrangThaiDuAnId = lsttt.Where(x => x.TenTrangThaiDuAn.Equals(item.Status)).FirstOrDefault().TrangThaiDuAnId;
                    }
                    vm.TenDuAn = item.ProjectName.Replace("&amp;", "&");
                    //LoaiDuAnId
                    if (lstloaiDA.Count(b => b.TenLoaiDuAn.Equals(item.Type)) == 0)
                    {
                        var loaiDA = _manager.GetLoaiDuAnByName(item.Type);
                        if (loaiDA != null && loaiDA.LoaiDuAnId > 0)
                        {
                            vm.LoaiDuAnId = loaiDA.LoaiDuAnId;
                            lstloaiDA.Add(loaiDA);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.Type))
                            {
                                vm.LoaiDuAnId = _manager.InsertLoaiDuAn(item.Type);
                                var loaiAdd = _manager.GetLoaiDuAnByName(item.Type);
                                if (loaiAdd != null)
                                    lstloaiDA.Add(loaiAdd);
                            }

                        }
                    }
                    else
                    {
                        vm.LoaiDuAnId = lstloaiDA.Where(b => b.TenLoaiDuAn.Equals(item.Type)).FirstOrDefault().LoaiDuAnId;
                    }
                    //NguoiQuanLyId
                    if (lstND.Count(a => a.HoTen.Equals(item.TeamLead)) == 0)
                    {
                        var nd = _nguoiDungM.GetByHoTen(item.TeamLead);
                        if (nd != null && nd.NguoiDungId != Guid.Empty)
                        {
                            vm.NguoiQuanLyId = nd.NguoiDungId;
                            lstND.Add(nd);
                        }
                        else
                        {
                            vm.NguoiQuanLyId = userId;
                        }
                    }
                    else
                    {
                        vm.NguoiQuanLyId = lstND.Where(a => a.HoTen.Equals(item.TeamLead)).FirstOrDefault().NguoiDungId;
                    }
                    vm.CostTien = item.CostTien;
                    vm.TongNganSach = item.Budget;
                    vm.TenHoatDong = item.Risk;                    
                    vm.MoTa = (item.Document + "").Replace("blank=\"\"", "blank").Replace("<br/>", "<br>");
                    vm.GhiChu = item.Note;
                    vm.MaDuAn = item.Ma;
                    vm.TenVietTat = item.Slack_Name;
                    if (vm.DuAnId == 0)
                    {
                        if (vm.MaDuAn != null || vm.KhoaChaId == null)
                            if (_manager.Insert(vm, userId) == 0)
                            {
                                result.Status = false;

                            }
                    }
                    else
                    {
                        if (!_manager.Update(vm, userId))
                        {
                            result.Status = false;
                        }
                    }
                    if (result.Status)
                    {
                        if (!string.IsNullOrEmpty(vm.TenVietTat) && channelNames.IndexOf( vm.TenVietTat.Trim().ToLower()) == -1&&(vm.TrangThaiDuAnId==(int)EnumTrangThaiDuAnType.Active||vm.TrangThaiDuAnId==(int)EnumTrangThaiDuAnType.Pending||vm.TrangThaiDuAnId==(int)EnumTrangThaiDuAnType.MacDinh))
                        {
                            CreateChannel(vm.TenVietTat.Trim().ToLower());
                        }
                    }
                }
            }
            return result;
        }
        public bool CreateChannel (string channelName)
        {
            var link = System.Configuration.ConfigurationManager.AppSettings.Get("apiConversationsCreate");
            var token = System.Configuration.ConfigurationManager.AppSettings.Get("TokenBotSlack");
            var cookie = System.Configuration.ConfigurationManager.AppSettings.Get("CokieTokenBotSlack");
            var client = new RestClient(link);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Content-Type", "application/json");
            string text = "";

            CreateChannelViewModel body = new CreateChannelViewModel();
                body.name = channelName;               
                text = JsonConvert.SerializeObject(body);
            

            request.AddParameter("application/json", text, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }
       
        public List<DuAnViewModel> GetDuAnThanhPhanByKhoaCha(int duAnId)
        {
            return _manager.GetDuAnThanhPhanByKhoaCha(duAnId);
        }

        public string GetMaDuAnById(int duAnId)
        {
            return _manager.GetMaDuAnById(duAnId);
        }
        public DuAnViewModel GetByMa(string maDuAn)
        {
            var vm = _manager.GetByMa(maDuAn);
                if (vm != null && vm.DuAnId > 0&&vm.KhoaChaId!=null) {
                var cha = _manager.GetTenDuAnById(vm.KhoaChaId.Value);
                if (cha != null) vm.TenKhoaCha = cha.TenDuAn;
            }
            return vm;
        }
        public List<DuAnViewModel> GetAll()
        {
            return _manager.GetAll();
        }
        public List<DuAnViewModel> GetsBy(string keyWord, int? trangThaiDuAnId)
        {
            var lst = _manager.GetsBy(trangThaiDuAnId, keyWord);
            if (lst != null && lst.Count > 0)
            {

                LoaiGiaiDoanDuAnProvider loaiGiaiDoanM = new LoaiGiaiDoanDuAnProvider();
                NguoiDungModel nguoiDungM = new NguoiDungModel();
                lst[0].Spent = 250;
                foreach (var item in lst)
                {
                    item.TotalShipable = _congViecM.TotalShipableInDuAn(item.DuAnId);
                    item.TotalShipableDone = _congViecM.TotalShipableDoneInDuAn(item.DuAnId);
                    var vm = _giaiDoanM.GetActiveBy(item.DuAnId);
                    if (vm != null)
                    {
                        var loaiGiaiDoan = loaiGiaiDoanM.GetById(vm.LoaiGiaiDoanId);
                        if (loaiGiaiDoan != null)
                        {
                            item.TenGiaiDoanActive = loaiGiaiDoan.TenLoaiGiaiDoan;
                            item.MaMau = loaiGiaiDoan.MaMau;
                        }
                    }
                    if (item.NgayBatDau != null)
                    {
                        var date = item.NgayBatDau.Value;
                        var dateN = DateTime.Now;
                        var day = (int)(dateN - date).TotalDays;
                        if (day < 0)
                        {
                            item.StrNgayBatDau2 = "0 ngày";
                        }
                        else if (day <= 31) item.StrNgayBatDau2 = day + " ngày";
                        else
                        {
                            if (day <= 365)
                            {
                                if (dateN.Day >= date.Day) item.StrNgayBatDau2 = (dateN.Month - date.Month) + " tháng " + (dateN.Day - date.Day) + " ngày trước";
                                else
                                {
                                    var day2 = DateTime.Now.AddMonths(-1);
                                    var dayInMonth = DateTime.DaysInMonth(day2.Year, day2.Month);
                                    item.StrNgayBatDau2 = (dateN.Month - (date.Month + 1)) + " tháng " + ((dateN.Day + dayInMonth) - date.Day) + " ngày";
                                }
                            }
                            else
                            {
                                if (dateN.Month >= date.Month)
                                {
                                    item.StrNgayBatDau += (dateN.Year - date.Year) + " năm ";
                                    if (dateN.Day >= date.Day) item.StrNgayBatDau2 += (dateN.Month - date.Month) + " tháng " + (dateN.Day - date.Day) + " ngày";
                                    else
                                    {
                                        var day2 = DateTime.Now.AddMonths(-1);
                                        var dayInMonth = DateTime.DaysInMonth(day2.Year, day2.Month);
                                        item.StrNgayBatDau2 += (dateN.Month - (date.Month + 1)) + " tháng " + ((dateN.Day + dayInMonth) - date.Day) + " ngày";
                                    }
                                }
                                else
                                {
                                    item.StrNgayBatDau += (dateN.Year - (date.Year + 1)) + " năm ";
                                    if (dateN.Day >= date.Day) item.StrNgayBatDau2 += ((dateN.Month + 12) - date.Month) + " tháng " + (dateN.Day - date.Day) + " ngày";
                                    else
                                    {
                                        var day2 = DateTime.Now.AddMonths(-1);
                                        var dayInMonth = DateTime.DaysInMonth(day2.Year, day2.Month);
                                        item.StrNgayBatDau2 += ((dateN.Month + 12) - (date.Month + 1)) + " tháng " + ((dateN.Day + dayInMonth) - date.Day) + " ngày";
                                    }
                                }
                            }
                        }
                    }
                    item.NguoiQuanLys = nguoiDungM.GetByDuAnId2(item.DuAnId);
                }
            }


            return lst;
        }

        public List<DuAnViewModel> GetsBy(int type,List<Guid> nguoiDungIds, List<int> status)
        {
            return _manager.GetsBy(type, nguoiDungIds, status);
          
        }

        public int CostHCuaDuAnThanhPhan(int duAnId, List<NguoiDungViewModel> lstLuong)
        {
            int cost = 0;
            ThoiGianLamViecProvider ttP = new ThoiGianLamViecProvider();
            var nguoiDungIds = ttP.GetUserSpentTimeInDuAnThanhPhan(duAnId);
          
            if (nguoiDungIds != null && nguoiDungIds.Count > 0 &&lstLuong!=null&&lstLuong.Count>0)
            {
                foreach(var nguoiDungId in nguoiDungIds)
                {
                    var nguoiDung = _nguoiDungM.GetById(nguoiDungId);
                        var time = ttP.GetTimeSpentOfNguoiDungInDuAnThanhPhan(duAnId, nguoiDung.NguoiDungId);
                        if (time > 0 &&lstLuong.Count(t=>t.HoTen.ToUpper()==nguoiDung.HoTen.ToUpper())>0)
                        {
                            var luong = lstLuong.Where(t => t.HoTen.ToUpper() == nguoiDung.HoTen.ToUpper()).FirstOrDefault().LuongThang;
                            cost +=(int)(luong * time / (192 * 3600));
                        }                      
                    
                } 
            }
            return (cost/1000)*1000;
        }
        public int CostHCuaDuAn(int duAnId, List<NguoiDungViewModel> lstLuong)
        {
            int cost = 0;
            ThoiGianLamViecProvider ttP = new ThoiGianLamViecProvider();
            var nguoiDungIds = ttP.GetUserSpentTimeInDuAn(duAnId);

            if (nguoiDungIds != null && nguoiDungIds.Count > 0 && lstLuong != null && lstLuong.Count > 0)
            {
                foreach (var nguoiDungId in nguoiDungIds)
                {
                    var nguoiDung = _nguoiDungM.GetById(nguoiDungId);
                    var time = ttP.GetTimeSpentOfNguoiDungInDuAn(duAnId, nguoiDung.NguoiDungId);
                    if (time > 0 && lstLuong.Count(t => t.HoTen.ToUpper() == nguoiDung.HoTen.ToUpper()) > 0)
                    {
                        var luong = lstLuong.Where(t => t.HoTen.ToUpper() == nguoiDung.HoTen.ToUpper()).FirstOrDefault().LuongThang;
                        cost += (int)(luong * time / (192 * 3600));
                    }

                }
            }
            return (cost / 1000) * 1000;
        }
        public int CostTienCuaDuAn(int duAnId)
        {
            ThoiGianLamViecProvider ttp = new ThoiGianLamViecProvider();
            return ttp.CostTienOfDuAn(duAnId);
        }
        public int TongNhanSach(int duAnId)
        {
            ThoiGianLamViecProvider ttp = new ThoiGianLamViecProvider();
            return ttp.TongNganSachOfDuAn(duAnId);
        }

        public List<DuAnViewModel> GetsByNguoiDungId(Guid userId)
        {
            List<DuAnViewModel> lstToReturn = new List<DuAnViewModel>();
            var lstId = _manager.GetsBy(userId);
            if (lstId != null && lstId.Count > 0)
            {
                lstToReturn = _manager.GetTenDuAnByIds(lstId);
            }
            return lstToReturn;
        }

        public int InsertDuAnViewModel(DuAnViewModel vm, Guid userId)
        {
            vm.NguoiQuanLyId = userId;
            if (!string.IsNullOrEmpty(vm.StrNgayBatDau))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrNgayBatDau, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.NgayBatDau = dt;
                }

            }
            if (!string.IsNullOrEmpty(vm.StrNgayKetThuc))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrNgayKetThuc, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.NgayKetThuc = dt;
                }

            }
            if (vm.GiaiDoanDuAns != null && vm.GiaiDoanDuAns.Count > 0)
            {
                foreach (var item in vm.GiaiDoanDuAns)
                {
                    if (!string.IsNullOrEmpty(item.StrThoiGianBatDau))
                    {
                        DateTime dt = DateTime.MinValue;
                        if (DateTime.TryParseExact(item.StrThoiGianBatDau, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            item.ThoiGianBatDau = dt;
                        }

                    }
                    if (!string.IsNullOrEmpty(item.StrThoiGianKetThuc))
                    {
                        DateTime dt = DateTime.MinValue;
                        if (DateTime.TryParseExact(item.StrThoiGianKetThuc, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            item.ThoiGianKetThuc = dt;
                        }
                    }
                }
            }
            return _manager.Insert(vm, userId);
        }

        public DuAnViewModel GetById(int duAnId, Guid userId)
        {
            var vm = _manager.GetById(duAnId, userId);
            if (vm != null)
            {
                LoaiCongViecProvider loaiCVM = new LoaiCongViecProvider();
                vm.LoaiCongViecIds = loaiCVM.GetIdsByDuAnId(duAnId);
                if (vm.LoaiCongViecIds != null && vm.LoaiCongViecIds.Count > 0) vm.strLoaiCongViecIds = JsonConvert.SerializeObject(vm.LoaiCongViecIds);
                vm.LienKetNguoiDungDuAn = _manager.GetlkNguoiDungDuAnByDuAnId(duAnId);
                GiaiDoanDuAnProvider giaiDoanDuAnM = new GiaiDoanDuAnProvider();
                vm.GiaiDoanDuAns = giaiDoanDuAnM.GetsByDuAnId(duAnId);
                if (vm.GiaiDoanDuAns != null && vm.GiaiDoanDuAns.Count > 0)
                {
                    HangMucCongViecProvider hangMucM = new HangMucCongViecProvider();
                    LoaiGiaiDoanDuAnProvider loaiGiaiDoanM = new LoaiGiaiDoanDuAnProvider();
                    foreach (var item in vm.GiaiDoanDuAns)
                    {
                        item.HangMucCongViecs = hangMucM.GetsByGiaiDoanDuAnId(item.GiaiDoanDuAnId);
                        if (item.HangMucCongViecs != null && item.HangMucCongViecs.Count > 0)
                        {
                            foreach (var item2 in item.HangMucCongViecs)
                            {
                                if (item2.TrangThai != null && item2.TrangThai.Value) item.CountHangMucChecked++;
                            }
                        }
                        var vm2 = loaiGiaiDoanM.GetById(item.LoaiGiaiDoanId);
                        if (vm2 != null)
                        {
                            item.TenLoaiGiaiDoan = vm2.TenLoaiGiaiDoan;
                            item.MaMau = vm2.MaMau;
                        }
                    }
                }
                var lk = _manager.GetPhongBanIdsByDuAnId(duAnId);
                if (lk != null && lk.Count > 0) vm.StrPhongBanIds = JsonConvert.SerializeObject(lk);
            }
            return vm;
        }
        public DuAnViewModel GetById2(int duAnId, Guid userId, DateTime? startTime, DateTime? endTime,int? giaiDoanDuAnId)
        {
            var vm = _manager.GetById(duAnId, userId); 
            if (vm != null)
            {
                var totalweek = 0;
                if (giaiDoanDuAnId == null) giaiDoanDuAnId = 0;
                GiaiDoanDuAnProvider giaiDoanDuAnM = new GiaiDoanDuAnProvider();
                if (giaiDoanDuAnId == 0)
                {
                    startTime = Helpers.GetMonDayBy(vm.NgayBatDau.Value);
                    vm.GiaiDoanDuAns = giaiDoanDuAnM.GetsByDuAnId(duAnId);
                    if (vm.NgayKetThuc == null)
                    {
                        endTime = Helpers.GetMonDayBy(DateTime.Now).AddDays(7).AddTicks(-1);
                    }
                    else
                    {
                        endTime = Helpers.GetMonDayBy(vm.NgayKetThuc.Value).AddDays(7).AddTicks(-1);
                    }
                }
                else
                {
                    vm.GiaiDoanDuAns = new List<GiaiDoanDuAnViewModel>();
                    var giaiDoan1= giaiDoanDuAnM.GetById(giaiDoanDuAnId.Value) ;
                    if (giaiDoan1.DuAnId == duAnId) vm.GiaiDoanDuAns.Add(giaiDoan1);
                    startTime = Helpers.GetMonDayBy(giaiDoan1.ThoiGianBatDau.Value);
                    endTime = Helpers.GetMonDayBy(giaiDoan1.ThoiGianKetThuc.Value);
                }              
                vm.GiaiDoanDuAnId = giaiDoanDuAnId.Value;               
                var startWeek = Helpers.GetNumerWeek(startTime.Value);
                var startYear = Helpers.GetMonDayBy(startTime.Value).Year;
                var endWeek = Helpers.GetNumerWeek(endTime.Value);
                var endYear = Helpers.GetMonDayBy(endTime.Value).Year;
                List<TuanViewModel> tuans = new List<TuanViewModel>();
                if (vm.GiaiDoanDuAns != null && vm.GiaiDoanDuAns.Count > 0)
                {
                    NoiDungProvider noiDungP = new NoiDungProvider();
                    for (int i = startYear; i <= endYear; i++)
                    {
                        if (i == endYear)
                        {
                            for (int j = startWeek; j <= endWeek; j++)
                            {
                                tuans.Add(new TuanViewModel() { week = j ,year=i});
                                totalweek++;
                            }
                        }
                        else
                        {
                            var maxWeekOfYear = Helpers.GetNumberWeekOfYears(i);
                            for (int j = startWeek; j <= maxWeekOfYear; j++)
                            {
                                tuans.Add(new TuanViewModel() { week = j, year = i });
                                totalweek++;
                            }
                            startWeek = 1;
                        }                      
                    }
                    var maxWeek = endWeek;
                    var maxYear = endYear;
                    LoaiGiaiDoanDuAnProvider loaiGiaiDoanM = new LoaiGiaiDoanDuAnProvider();
                    foreach (var giaiDoan in vm.GiaiDoanDuAns)
                    {
                        var vm2 = loaiGiaiDoanM.GetById(giaiDoan.LoaiGiaiDoanId);
                        if (vm2 != null)
                        {
                            giaiDoan.TenLoaiGiaiDoan = vm2.TenLoaiGiaiDoan;
                            giaiDoan.MaMau = vm2.MaMau;
                        }                                    
                        var ships = _congViecM.GetShipablesByGiaiDoan(duAnId, giaiDoan.GiaiDoanDuAnId);
                        
                        if (ships!=null&& ships.Count > 0)
                        {
                            foreach (var item in ships)
                            {
                                item.CountCongViecCon = _congViecM.CountTaskByShipId(item.CongViecId);
                                item.TuanDuKienHoanThanh = Helpers.GetNumberWeekOfDay(item.NgayDuKienHoanThanh.Value);                                
                            
                                var namofTuanDuKien = Helpers.GetMonDayBy(item.NgayDuKienHoanThanh.Value).Year;
                                if (item.TuanHoanThanh == null)
                                {
                                    item.TuanHoanThanh = item.Tuan;
                                }
                                if (item.Tuan > item.TuanDuKienHoanThanh)
                                {
                                    if (item.Nam <= namofTuanDuKien) break;
                                    var year1 = startTime.Value.Year;                                    
                                    var numberweek = Helpers.GetNumberWeekOfYears(year1);
                                    item.SoTuanDuKien = item.TuanDuKienHoanThanh + (numberweek - item.Tuan) + 1; 
                                    item.TuanDaChay = item.TuanHoanThanh.Value + (numberweek - item.Tuan) + 1;
                                    item.NamHoanThanh = item.Nam + 1;
                                }
                                else
                                {
                                    item.SoTuanDuKien = item.TuanDuKienHoanThanh - item.Tuan + 1;                                   
                                    item.TuanDaChay = item.TuanHoanThanh.Value - item.Tuan + 1;
                                    item.NamHoanThanh = item.Nam;
                                }
                               
                                    item.SoNgayDuKien = item.SoTuanDuKien * 7;
                                    item.SoNgayDaChay = item.TuanDaChay * 7;
                               
                                if(item.NgayTao.Value.DayOfWeek!=DayOfWeek.Monday)
                                {
                                    var ngay = item.NgayTao.Value;
                                    do
                                    {
                                        item.SoNgayDuKien -= 1;
                                        item.SoNgayDaChay -= 1;
                                        ngay= ngay.AddDays(-1);
                                    } while (ngay.DayOfWeek != DayOfWeek.Monday);
                                }                               
                                if ((item.Tuan > maxWeek && item.Nam == maxYear)||item.Nam>maxYear)
                                {
                                    maxYear = item.Nam;
                                    maxWeek = item.Tuan;
                                }
                                if (item.TuanDuKienHoanThanh>0&&(item.TuanDuKienHoanThanh > maxWeek && namofTuanDuKien == maxYear) || namofTuanDuKien > maxYear)
                                {                                    
                                    maxYear = namofTuanDuKien;
                                    maxWeek = item.TuanDuKienHoanThanh;
                                }
                                var ngayBatDau = Helpers.GetFirstMondayOfWeek(item.Nam, item.Tuan);
                                DateTime ngayKetThuc;
                                if(item.TuanHoanThanh > item.TuanDuKienHoanThanh && item.NamHoanThanh >= namofTuanDuKien)
                                {
                                    ngayKetThuc = Helpers.GetFirstMondayOfWeek(item.NamHoanThanh, item.TuanHoanThanh.Value).AddDays(6);
                                }
                                else
                                {
                                    ngayKetThuc = Helpers.GetFirstMondayOfWeek(namofTuanDuKien, item.TuanDuKienHoanThanh).AddDays(6);
                                }

                                item.Comments = noiDungP.GetsBy(item.CongViecId.ToString(), (byte)EnumLoaiNoiDungType.CommentDuAnAndShip, (byte)EnumItemTypeType.ShipAbleType);
                                if (item.Comments != null && item.Comments.Count > 0)
                                {
                                    var lst = item.Comments.OrderBy(t => t.NgayTao);
                                    var left = 0;
                                    List<string> strNgayTaos = new List<string>();
                                    item.MarginLeftComments = new List<string>();
                                    var check = true;
                                    foreach(var comment in lst)
                                    {
                                        if (check)
                                        {
                                            var strNgayTao = comment.NgayTao.ToString("dd/MM/yyyy");
                                            if (strNgayTaos.IndexOf(strNgayTao) == -1)
                                            {
                                                strNgayTaos.Add(strNgayTao);
                                                if (comment.NgayTao < ngayKetThuc)
                                                {    
                                                    int margin = (int)(comment.NgayTao - ngayBatDau).TotalDays*10;
                                                    if (margin >= 60) margin=margin-10;
                                                    var marginpx = (margin) +"px";
                                                    if (margin < 0) marginpx = "0px";
                                                    item.MarginLeftComments.Add(marginpx);
                                                    ngayBatDau = item.NgayTao.Value;
                                                }
                                                else
                                                {
                                                    check = false;
                                                    int ngay = (int)(ngayKetThuc.AddDays(1) - ngayBatDau).TotalDays-1;
                                                    if (ngay >= 5) ngay--;
                                                    var margin = (ngay * 10) + "px";
                                                    item.MarginLeftComments.Add(margin);
                                                }                                            
                                            }
                                        }
                                       
                                    }
                                }
                            }
                        }
                        giaiDoan.Shipables = ships;
                    }
                    if(maxYear>endYear||(maxYear==endYear && maxWeek > endWeek)){
                        for(int i = endYear; i <= maxYear; i++)
                        {
                            if (i == maxYear)
                            {
                                for (int j = endWeek + 1; j <= maxWeek; j++)
                                {
                                    tuans.Add(new TuanViewModel() { week = j ,year=i});
                                    totalweek++;
                                }
                            }
                            else
                            {
                                var maxWeekOfYear = Helpers.GetNumberWeekOfYears(i);
                                if(endWeek<maxWeekOfYear)
                                for (int j = endWeek+1; j <= maxWeekOfYear; j++)
                                {
                                    tuans.Add(new TuanViewModel() { week = j, year = i });
                                        totalweek++;
                                    }
                                endWeek = 0;
                            }
                        }
                    }                   
                }
                vm.Tuans = tuans;
                vm.TotalWeek = totalweek;
            }
            return vm;
        }
        public DuAnViewModel GetById3(int duAnId)
        {
            var vm = _manager.GetDuAnThanhPhanById(duAnId);
            if (vm != null && vm.DuAnId > 0)
            {
                vm.CongViecs = _congViecM.GetTenShipableByDuAnId2(duAnId);
            }
            // lấy link coda:
            CauHinhProvider ch = new CauHinhProvider();
            var ma = ch.GetValueByTen("Project/Edit/" + duAnId);
            if (string.IsNullOrEmpty(ma))
            {
                ma = System.Configuration.ConfigurationManager.AppSettings.Get("LinkProjectEdit");
            }
            vm.LinkCoda = ma;
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var bangluong = GetDataLuongThang(path + "LuongThang.xlsx", "LuongThang");
            vm.CostH = CostHCuaDuAnThanhPhan(duAnId, bangluong);
          
            vm.Cost = 0;
            if (vm.CostH > 0) vm.Cost += vm.CostH;
            if (vm.CostTien > 0) vm.Cost += vm.CostTien;
            vm.StrCost= vm.Cost.Value.ToString("C0").Substring(1);
            vm.StrCostH= vm.CostH.Value.ToString("C0").Substring(1);
            vm.StrCostTien = vm.CostTien.Value.ToString("C0").Substring(1);
            if (vm.TongNganSach == null) vm.TongNganSach = 0;
            vm.StrTongNganSach = vm.TongNganSach.Value.ToString("C0").Substring(1);
            return vm;
        }
        public DuAnViewModel GetShortInfoById(int duAnId)
        {
            return _manager.GetById(duAnId);
        }
        public bool UpdateDuAn(DuAnViewModel vm, Guid userId)
        {
            if (!string.IsNullOrEmpty(vm.StrNgayBatDau))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrNgayBatDau, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.NgayBatDau = dt;
                }

            }
            if (!string.IsNullOrEmpty(vm.StrNgayKetThuc))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrNgayKetThuc, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.NgayKetThuc = dt;
                }

            }
            return _manager.Update(vm, userId);
        }

        public bool DeleteDuAn(int duAnId, Guid userId)
        {
            return _manager.Delete(duAnId, userId);
        }
        #region LienKetNguoiDungDuAn
        public bool InsertLienKetNguoiDungDuAn(LienKetNguoiDungDuAnViewModel vm)
        {
            return _manager.InsertLienKetNguoiDungDuAn(vm.DuAnId, vm.NguoiDungId, vm.LaQuanLy.Value);
        }
        public bool UpdateLienKetNguoiDungDuAn(LienKetNguoiDungDuAnViewModel vm)
        {
            return _manager.UpdateLienKetNguoiDungDuAn(vm.DuAnId, vm.NguoiDungId, vm.LaQuanLy.Value);
        }
        public bool DeleteLienKetNguoiDungDuAn(LienKetNguoiDungDuAnViewModel vm)
        {
            return _manager.DeleteLienKetNguoiDungDuAn(vm.DuAnId, vm.NguoiDungId);
        }
        #endregion
        public List<GiaiDoanDuAnViewModel> GetGiaiDoanDuAnsByDuAnId(int duAnId)
        {
            return _giaiDoanM.GetsByDuAnId(duAnId);
        }
        public List<TrangThaiDuAnViewModel> GetTrangThaiDuAns()
        {
            TrangThaiDuAnProvider manager = new TrangThaiDuAnProvider();
            return manager.Gets();
        }
        public List<LoaiNganSachViewModel> GetLoaiNganSachs()
        {
            LoaiNganSachProvider manager = new LoaiNganSachProvider();
            return manager.Gets();
        }
        public int InsertGiaiDoanDuAn(GiaiDoanDuAnViewModel vm, Guid userId)
        {
            if (!string.IsNullOrEmpty(vm.StrThoiGianBatDau))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrThoiGianBatDau, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.ThoiGianBatDau = dt;
                }

            }
            if (!string.IsNullOrEmpty(vm.StrThoiGianKetThuc))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrThoiGianKetThuc, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.ThoiGianKetThuc = dt;
                }
            }
            return _giaiDoanM.InsetGiaiDoanDuAn(vm, userId);
        }
        public GiaiDoanDuAnViewModel GetGiaiDoanDuAnById(int id)
        {
            var vm = _giaiDoanM.GetById(id);
            HangMucCongViecProvider hangMucM = new HangMucCongViecProvider();
            LoaiGiaiDoanDuAnProvider loaiGiaiDoanM = new LoaiGiaiDoanDuAnProvider();
            if (vm != null)
            {
                vm.HangMucCongViecs = hangMucM.GetsByGiaiDoanDuAnId(vm.GiaiDoanDuAnId);
                var vm2 = loaiGiaiDoanM.GetById(vm.LoaiGiaiDoanId);
                if (vm2 != null) vm.TenLoaiGiaiDoan = vm2.TenLoaiGiaiDoan;
                if (vm.ThoiGianBatDau != null) vm.StrThoiGianBatDau = String.Format("{0:dd/MM/yyyy}", vm.ThoiGianBatDau);
                if (vm.ThoiGianKetThuc != null) vm.StrThoiGianKetThuc = String.Format("{0:dd/MM/yyyy}", vm.ThoiGianKetThuc);
            }
            return vm;
        }
        public bool UpdateGiaiDoanDuAn(GiaiDoanDuAnViewModel vm, Guid userId)
        {
            if (!string.IsNullOrEmpty(vm.StrThoiGianBatDau))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrThoiGianBatDau, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.ThoiGianBatDau = dt;
                }

            }
            if (!string.IsNullOrEmpty(vm.StrThoiGianKetThuc))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrThoiGianKetThuc, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.ThoiGianKetThuc = dt;
                }
            }
            return _giaiDoanM.UpdateGiaiDoanDuAn(vm, userId);
        }
        public bool DeleteGiaiDoanDuAn(int giaiDoanDuAnId)
        {
            return _giaiDoanM.Delete(giaiDoanDuAnId);
        }
        public bool Archive(int duAnId, Guid userId)
        {
            return _manager.Archive(duAnId, userId);
        }
        public int Count(int type)
        {
            return _manager.Count(type);
        }
        public List<LoaiGiaiDoanDuAnViewModel> GetLoaiGiaiDoans()
        {
            LoaiGiaiDoanDuAnProvider manager = new LoaiGiaiDoanDuAnProvider();
            return manager.Gets();
        }
        public List<LoaiDuAnViewModel> GetLoaiDuAns()
        {
            
            return _manager.GetAllLoaiDuAn();
        }

        public List<DuAnViewModel> GetReportProjectBy(int duAnId, Guid nguoiDungId, DateTime startTime, DateTime endTime)
        {
            List<DuAnViewModel> lstToReturn = new List<DuAnViewModel>();
            lstToReturn = GetsByp(duAnId, nguoiDungId);
            if (lstToReturn != null && lstToReturn.Count > 0)
            {
                ThoiGianLamViecProvider thoiGianM = new ThoiGianLamViecProvider();
                foreach(var duAn in lstToReturn)
                {
                    List<ThoiGianSuDungViewModel> thoiGianSuDungs = new List<ThoiGianSuDungViewModel>();
                    List<NguoiDungViewModel> nguoiDungs = new List<NguoiDungViewModel>();
                    if (nguoiDungId != Guid.Empty)
                    {
                        var nguoiDungVm = _nguoiDungM.GetById(nguoiDungId);
                        if (nguoiDungVm != null) nguoiDungs.Add(nguoiDungVm);
                    }else 
                     nguoiDungs = _nguoiDungM.GetAll();
                    var total = 0;
                    if (nguoiDungs != null && nguoiDungs.Count > 0)
                    {
                        foreach(var nguoiDung in nguoiDungs)
                        {
                            ThoiGianSuDungViewModel thoiGianSuDung = new ThoiGianSuDungViewModel();
                            thoiGianSuDung.NguoiDungId = nguoiDung.NguoiDungId;
                            thoiGianSuDung.HoTen = nguoiDung.HoTen;
                            thoiGianSuDung.ThoiGianSuDung = thoiGianM.GetTimeInProjectBy(nguoiDung.NguoiDungId, duAn.DuAnId, startTime, endTime);
                            thoiGianSuDung.StrThoiGianSuDung = GetStrTime(thoiGianSuDung.ThoiGianSuDung);
                            thoiGianSuDungs.Add(thoiGianSuDung);
                            total += thoiGianSuDung.ThoiGianSuDung;
                        }
                    }
                    duAn.ThoiGianSuDungs = thoiGianSuDungs;
                    duAn.Spent = total;
                    duAn.StrTimeSpent = GetStrTime(total);
                }
            }
            return lstToReturn;
        }

        public string GetDataReportProjectDetail(int duAnId, DateTime startTime, DateTime endTime)
        {
            ReportProjectDetailsViewModel vm = new ReportProjectDetailsViewModel();
            LoaiCongViecProvider loaiCongViecP = new LoaiCongViecProvider();
            var duAn = _manager.GetById(duAnId);
            if (duAn != null) vm.TenDuAn = duAn.TenDuAn;
            var lstLoaiCongViec = loaiCongViecP.GetLoaiCongViecConByDuAnId(duAnId);
            ThoiGianLamViecProvider thoiGianLamViecP = new ThoiGianLamViecProvider();
            List<double> ThoiGianSuDungs = new List<double>();
            List<string> LoaiCongViecs = new List<string>();
            var total = thoiGianLamViecP.GetAllTimeInProjectBy(duAnId, startTime, endTime);
            int totaluse = 0;
            if(lstLoaiCongViec!=null&& lstLoaiCongViec.Count > 0)
            {

                foreach(var loaiCongViec in lstLoaiCongViec)
                {
                    var time = thoiGianLamViecP.GetTimeOfLoaiCongViecInProjectBy(duAnId, startTime, endTime, loaiCongViec.LoaiCongViecId);
                    if (time > 0)
                    {
                        totaluse += time;
                        double a =(double)time / 3600;
                        double timed = Math.Round(a,1);
                        if (timed > 0)
                        {
                            LoaiCongViecs.Add(loaiCongViec.TenLoaiCongViec);
                            ThoiGianSuDungs.Add(timed);
                        }                       
                    }
                }
               
                if (totaluse <total)
                {
                    int outTime = total - totaluse;
                    double a = (double)outTime / 3600;
                    double timed = Math.Round(a, 1);
                    if (timed > 0)
                    {
                        LoaiCongViecs.Add("Add Off Time");
                        ThoiGianSuDungs.Add(timed);
                    }
                }
            }
            vm.LoaiCongViecs = LoaiCongViecs;
            vm.HourSpents = ThoiGianSuDungs;
            vm.TotalHour = GetStrTime(total);
            vm.KhoangThoiGian = getKhoangTime(startTime, endTime);

            return JsonConvert.SerializeObject(vm);
        }

        public string GetDataReportUserDetail(Guid nguoiDungId, DateTime startTime, DateTime endTime)
        {
            ReportProjectDetailsViewModel vm = new ReportProjectDetailsViewModel();
            LoaiCongViecProvider loaiCongViecP = new LoaiCongViecProvider();            
            var nguoiDung = _nguoiDungM.GetById(nguoiDungId);
            if (nguoiDung != null) vm.HoTen = nguoiDung.HoTen;
            var lstLoaiCongViec = loaiCongViecP.GetAllLoaiCongViecCon();
            ThoiGianLamViecProvider thoiGianLamViecP = new ThoiGianLamViecProvider();
            List<double> ThoiGianSuDungs = new List<double>();
            List<string> LoaiCongViecs = new List<string>();
            int total = 0;
            var real = thoiGianLamViecP.GetAllTimeOfUserBy(nguoiDungId, startTime, endTime);
            if (lstLoaiCongViec != null && lstLoaiCongViec.Count > 0)
            {

                foreach (var loaiCongViec in lstLoaiCongViec)
                {
                    var time = thoiGianLamViecP.GetTimeOfLoaiCongViecWithUserBy(nguoiDungId, startTime, endTime, loaiCongViec.LoaiCongViecId);
                    if (time > 0)
                    {
                        total += time;
                        double a = (double)time / 3600;
                        double timed = Math.Round(a, 1);
                        if (timed > 0)
                        {
                            LoaiCongViecs.Add(loaiCongViec.TenLoaiCongViec);
                            ThoiGianSuDungs.Add(timed);
                            
                        }
                    }
                }
                
                if (total < real)
                {
                    var outTime = real - total;
                    double a = (double)outTime / 3600;
                    double timed = Math.Round(a, 1);
                    if (timed > 0)
                    {
                        LoaiCongViecs.Add("Add Off Time");
                        ThoiGianSuDungs.Add(timed);
                        total += outTime;
                    }
                }
            }
            vm.LoaiCongViecs = LoaiCongViecs;
            vm.HourSpents = ThoiGianSuDungs;
            vm.TotalHour = GetStrTime(real);
            vm.KhoangThoiGian = getKhoangTime(startTime, endTime);
            return JsonConvert.SerializeObject(vm);
        }

        public string GetDataReportUserProjectDetail2(Guid nguoiDungId, DateTime startTime, DateTime endTime)
        {
            ReportProjectDetailsViewModel vm = new ReportProjectDetailsViewModel();        
            var nguoiDung = _nguoiDungM.GetById(nguoiDungId);
            if (nguoiDung != null) vm.HoTen = nguoiDung.HoTen;
            var duAnIDs = _manager.GetsBy(nguoiDungId);
            ThoiGianLamViecProvider thoiGianLamViecP = new ThoiGianLamViecProvider();
            List<double> ThoiGianSuDungs = new List<double>();
            List<string> TenDuAns = new List<string>();
            int total = 0;
            var real = thoiGianLamViecP.GetAllTimeOfUserBy(nguoiDungId, startTime, endTime);
            if (duAnIDs != null && duAnIDs.Count > 0)
            {

                foreach (var duAnId in duAnIDs)
                {
                    var time = thoiGianLamViecP.GetTimeInProjectBy(nguoiDungId,duAnId,startTime,endTime);
                    if (time > 0)
                    {
                        total += time;
                        double a = (double)time / 3600;
                        double timed = Math.Round(a, 1);
                        if (timed > 0)
                        {
                            var duAn = _manager.GetById(duAnId);
                            if (duAn != null)
                            {
                                TenDuAns.Add(duAn.TenDuAn);
                                ThoiGianSuDungs.Add(timed);
                            }
                          

                        }
                    }
                }
            }
            vm.TenDuAns = TenDuAns;
            vm.HourSpents = ThoiGianSuDungs;
            vm.TotalHour = GetStrTime(real);
            vm.KhoangThoiGian = getKhoangTime(startTime, endTime);
            return JsonConvert.SerializeObject(vm);
        }




        public string GetDataReportUserProjectDetail(Guid nguoiDungId,int duAnId, DateTime startTime, DateTime endTime)
        {
            ReportProjectDetailsViewModel vm = new ReportProjectDetailsViewModel();
            LoaiCongViecProvider loaiCongViecP = new LoaiCongViecProvider();
            var nguoiDung = _nguoiDungM.GetById(nguoiDungId);
            if (nguoiDung != null) vm.HoTen = nguoiDung.HoTen;
            var duAn = _manager.GetById(duAnId);
            if (duAn != null) vm.TenDuAn = duAn.TenDuAn;
            var lstLoaiCongViec = loaiCongViecP.GetLoaiCongViecConByDuAnId(duAnId);
            ThoiGianLamViecProvider thoiGianLamViecP = new ThoiGianLamViecProvider();
            List<PieTimeViewModel> ThoiGianSuDungs = new List<PieTimeViewModel>();         
            int total = thoiGianLamViecP.GetTimeUserInProjectBy(nguoiDungId, duAnId, startTime, endTime);
            double totalPer = 0;
            if (lstLoaiCongViec != null && lstLoaiCongViec.Count > 0)
            {
                foreach (var loaiCongViec in lstLoaiCongViec)
                {
                    var time = thoiGianLamViecP.GetTimeOfLoaiCongViecWithUserProjectBy(nguoiDungId,duAnId, startTime, endTime, loaiCongViec.LoaiCongViecId);
                    if (time > 0)
                    {
                        
                        PieTimeViewModel vm1 = new PieTimeViewModel();
                        vm1.category = loaiCongViec.TenLoaiCongViec;
                        var value = Math.Round(((double)time / (double)total) * 100, 2);
                        totalPer += value;
                        if (totalPer > 100) value = 100 - (totalPer - value);
                        vm1.value = value;
                        ThoiGianSuDungs.Add(vm1);
                    }
                }
                if (totalPer < 100)
                {
                    PieTimeViewModel vm1 = new PieTimeViewModel();
                    vm1.category = "Add Off Time";
                    vm1.value = 100 - totalPer;
                    ThoiGianSuDungs.Add(vm1);
                }
                    
            }
            vm.PieTime = ThoiGianSuDungs;         
            vm.TotalHour = GetStrTime(total);
            vm.KhoangThoiGian = getKhoangTime(startTime, endTime);

            return JsonConvert.SerializeObject(vm);
        }

        private string getKhoangTime( DateTime startTime, DateTime endTime)
        {
            string result = "";
            if (startTime.Year != endTime.Year)
            {
                result = " từ ngày " + startTime.ToString("dd/MM/yyyy") + " đến ngày " + endTime.ToString("dd/MM/yyyy");
            }
            else
            {
                if (startTime.Month != endTime.Month)
                {
                    result = " từ ngày " + startTime.ToString("dd/MM") + " đến ngày " + endTime.ToString("dd/MM/yyyy");
                }
                else
                {
                    if (startTime.ToString("dd/MM/yyyy") == endTime.ToString("dd/MM/yyyy"))
                    {
                        result = " trong ngày " + startTime.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        result = " từ ngày " + startTime.Day + " đến ngày " + endTime.ToString("dd/MM/yyyy");
                    }
                }
            }
            return result;
        }
        private List<DuAnViewModel> GetsByp(int duAnId, Guid nguoiDungId)
        {
            List<DuAnViewModel> lstToReturn = new List<DuAnViewModel>();
            if (duAnId > 0)
            {
                var vm = _manager.GetById(duAnId);
                if (vm != null) lstToReturn.Add(vm);
            }
            else if (nguoiDungId != Guid.Empty)
            {
                var lstId = _manager.GetsBy(nguoiDungId);
                if (lstId != null && lstId.Count > 0)
                {
                    foreach(var id in lstId)
                    {
                        var duAn = _manager.GetById(id);
                        if (duAn != null) lstToReturn.Add(duAn);
                    }
                }
            }
            else
            {
                lstToReturn = _manager.GetAll2();
            }
            return lstToReturn;
        }
        private string GetStrTime(int timeSpent)
        {
            var result = "";
            if (timeSpent > 0)
            {
                var hour = timeSpent / 3600;
                var minutes = (timeSpent % 3600) / 60;
                var second = (timeSpent % 3600) % 60;
                if (second > 0) minutes++;
                result += hour + "h";
                if (minutes < 10)
                    result += "0" + minutes + "p";
                else result += minutes + "p";
            }
            else
            {
                result = "-";
            }
            return result;
        }

        public bool InsertLienKetDuAnPhongBan(int duAnId, int phongBanId)
        {
            return _manager.InsertLienKetDuAnPhongBan(duAnId, phongBanId);
        }
        public bool DeleteLienKetDuAnPhongBan(int duAnId, int phongBanId)
        {
            return _manager.DeleteLienKetDuAnPhongBan(duAnId, phongBanId);
        }
        public List<NguoiDungViewModel> GetDataLuongThang(string fileName, string sheetName)
        {
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            ExcelOpenXml excel = new ExcelOpenXml(fileName, sheetName);
            var table = excel.ReadDataTable(2, false);
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    NguoiDungViewModel nd = new NguoiDungViewModel();
                    nd.HoTen = row[0].ToString();
                    nd.LuongThang = double.Parse(row[1].ToString());
                    lstToReturn.Add(nd);
                }
            }
            return lstToReturn;
        }
    }
}