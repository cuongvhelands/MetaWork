using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.ExcelUtilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;

namespace MetaWork.Project.Models
{
    public class CongViecModel
    {
        TrangThaiCongViecProvider _trangThaiCCM = new TrangThaiCongViecProvider();
        CongViecProvider _manager = new CongViecProvider();
        NguoiDungProvider _nguoiDungM = new NguoiDungProvider();
        DuAnProvider _duanProvider = new DuAnProvider();
        LoaiCongViecProvider _loaiCongViecP = new LoaiCongViecProvider();
        ThoiGianLamViecProvider _thoiGianLamViecP = new ThoiGianLamViecProvider();
        #region ToDoWork
     



        public List<DuAnViewModel> GetToDoWorksBy(Guid nguoiDungId, List<int> duAnIds, List<int> phongBanIds, List<Guid> nguoiDungIds, int trangThaiCongViecId, bool quanTam, string tenShipable, DateTime startTime, DateTime endTime)
        {
            List<DuAnViewModel> lstToReturn = new List<DuAnViewModel>();
            if (nguoiDungIds == null || nguoiDungIds.Count == 0)
            {
                nguoiDungIds = _nguoiDungM.GetIdsBy(phongBanIds, duAnIds);
            }
            var ships = GetAllShip(nguoiDungId, duAnIds, nguoiDungIds, trangThaiCongViecId, quanTam, tenShipable, startTime, endTime);
            if (ships != null && ships.Count > 0)
            {
                ships = ships.OrderBy(t => t.DuAnId).ThenByDescending(t=>t.NgayCapNhat).ToList();
                DuAnViewModel duan = new DuAnViewModel();
                foreach (var ship in ships)
                {                
                    if (duan.DuAnId == 0)
                    {
                        duan = _duanProvider.GetById(ship.DuAnId);
                        duan.CongViecs = new List<CongViecViewModel>();
                    }

                    if (duan.DuAnId>0&& duan.DuAnId != ship.DuAnId)
                    {
                        if (duan.DuAnId > 0) lstToReturn.Add(duan);
                        duan = _duanProvider.GetById(ship.DuAnId);
                        duan.CongViecs = new List<CongViecViewModel>() { ship };
                        duan.NgayCapNhat = ship.NgayCapNhat;
                    }
                    else if (duan.DuAnId == ship.DuAnId && duan.DuAnId > 0 && duan.CongViecs.Count(t => t.CongViecId == ship.CongViecId) == 0)
                    {
                        duan.CongViecs.Add(ship);
                        if (ship.NgayCapNhat > duan.NgayCapNhat) duan.NgayCapNhat = ship.NgayCapNhat;
                    }
                }
                lstToReturn.Add(duan);               
            }
            if (lstToReturn != null && lstToReturn.Count > 0) lstToReturn = lstToReturn.OrderByDescending(t => t.NgayCapNhat).ToList();
            return lstToReturn;
        }
        public List<CongViecViewModel> GetAllShip(Guid nguoiDungId, List<int> duAnIds, List<Guid> nguoiDungIds, int trangThaiCongViecId, bool quanTam, string tenShipable, DateTime startTime, DateTime endTime)
        {
            List<CongViecViewModel> lstToreturn = new List<CongViecViewModel>();
            if (nguoiDungIds == null || nguoiDungIds.Count == 0)
            {
                var nguoidung = _nguoiDungM.GetById(nguoiDungId);
                nguoiDungIds = _nguoiDungM.GetUserIdsBy(nguoidung.PhongBanId);
            }

            if (nguoiDungIds != null && nguoiDungIds.Count > 0)
            {
                var shipIds = _manager.GetShipIdsBy(nguoiDungIds, nguoiDungId, duAnIds,null, trangThaiCongViecId, quanTam, tenShipable, startTime, endTime);
                var tasks = _manager.GetTaskInTimeBy(duAnIds,null, nguoiDungIds,nguoiDungId,quanTam, startTime, endTime);
                if (tasks != null && tasks.Count > 0)
                {
                    tasks = tasks.OrderByDescending(t => t.KhoaChaId).ThenByDescending(t => t.NgayDuKienHoanThanh).ThenByDescending(t=>t.NgayTao).ToList();
                    CongViecViewModel ship = new CongViecViewModel();
                    foreach(var task in tasks)
                    {
                        task.QuanTam = _manager.getQuanTam(task.CongViecId, nguoiDungId);
                        var vm = _thoiGianLamViecP.GetInfoOfTakForTimeBy(nguoiDungId, task.CongViecId);
                        if (vm != null)
                        {
                            task.TongThoiGian = vm.TongThoiGian;                            
                            
                            task.ThoiGianBatDau = vm.ThoiGianBatDau;
                            task.ThoiGianKetThuc = vm.ThoiGianKetThuc;
                            if (task.TongThoiGian > 0)
                            {
                                task.StrTotalTime = GetStrTime2(task.TongThoiGian);
                                if (task.ThoiGianBatDau != null)
                                {
                                    task.StrTotalTime += " <br/>  <span class=\"text-mute font-11 font-w-300\">(" + task.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm");
                                    if (task.ThoiGianKetThuc != null) task.StrTotalTime += " - " + task.ThoiGianKetThuc.Value.ToString("dd/MM/yyyy HH:mm");
                                    task.StrTotalTime += ")</span>";
                                }

                            }
                            else if (task.ThoiGianBatDau != null)
                            {
                                task.StrTotalTime = "Thời gian bắt đâu: " + task.ThoiGianBatDau.Value.ToString("dd/MM/yyyy hh:mm");
                            }


                        }
                        var nguoiIds = _thoiGianLamViecP.GetNguoiDungIdsStartTimeInTask(task.CongViecId);
                        if(nguoiIds!=null&& nguoiIds.Count > 0)
                        {
                            task.NguoiDungs = new List<NguoiDungViewModel>();
                            foreach(var item in nguoiIds)
                            {
                                var nguoiDung = _nguoiDungM.GetAvatarNguoiDungById(item);
                                if(nguoiDung!=null&&task.NguoiDungs.Count(t=>t.NguoiDungId==item)==0)
                                task.NguoiDungs.Add(nguoiDung);
                            }
                        }
                        if (ship.CongViecId == 0)
                        {
                            ship = _manager.GetInfoById(task.KhoaChaId.Value);
                            if (ship.NgayCapNhat==null|| ship.NgayCapNhat < ship.NgayTao) ship.NgayCapNhat = ship.NgayTao;
                            ship.QuanTam = _manager.getQuanTam(ship.CongViecId, nguoiDungId);
                            if (ship.NgayCapNhat < task.NgayDuKienHoanThanh) ship.NgayCapNhat = task.NgayDuKienHoanThanh;
                            if (ship.NgayCapNhat < task.NgayTao) ship.NgayCapNhat = task.NgayTao;
                            //ship.TongThoiGian = _thoiGianLamViecP.GetToTalTimeSpendInShip(ship.CongViecId);
                            //ship.StrTotalTime = GetStrTime(ship.TongThoiGian);
                        }
                        
                        if (ship.CongViecId>0&& ship.CongViecId == task.KhoaChaId.Value)
                        {
                            if (ship.CongViecs == null) ship.CongViecs = new List<CongViecViewModel>();
                            ship.CongViecs.Add(task);
                            if (ship.NgayCapNhat < task.NgayDuKienHoanThanh) ship.NgayCapNhat = task.NgayDuKienHoanThanh;
                            if (ship.NgayCapNhat < task.NgayTao) ship.NgayCapNhat = task.NgayTao;
                        }
                        else if(ship.CongViecId > 0 && ship.CongViecId != task.KhoaChaId.Value)
                        {                            
                            lstToreturn.Add(ship);
                            ship = _manager.GetInfoById(task.KhoaChaId.Value);
                            if (ship.NgayCapNhat==null|| ship.NgayCapNhat < ship.NgayTao) ship.NgayCapNhat = ship.NgayTao;
                            if (ship.NgayCapNhat < task.NgayDuKienHoanThanh) ship.NgayCapNhat = task.NgayDuKienHoanThanh;
                            if (ship.NgayCapNhat < task.NgayTao) ship.NgayCapNhat = task.NgayTao;
                            ship.CongViecs= new List<CongViecViewModel>(){ task };
                        }
                    }
                    if ((lstToreturn.Count(t => t.CongViecId == ship.CongViecId) == 0) && ship.CongViecId > 0) lstToreturn.Add(ship);
                }
                var taskruneTimeIds = _manager.GetTaskIdRunTimeBy(duAnIds,null, nguoiDungIds, quanTam, startTime, endTime);
                if (taskruneTimeIds != null && taskruneTimeIds.Count > 0)
                {
                    foreach(var taskId in taskruneTimeIds)
                    {
                        if (tasks == null || tasks.Count == 0 || tasks.Count(t => t.CongViecId == taskId) == 0)
                        {
                            var task = _manager.GetInfoById(taskId);
                            var vm = _thoiGianLamViecP.GetInfoOfTakForTimeBy(nguoiDungId, task.CongViecId);
                            if (vm != null)
                            {
                                task.TongThoiGian = vm.TongThoiGian;
                                task.QuanTam = _manager.getQuanTam(task.CongViecId, nguoiDungId);
                                task.ThoiGianBatDau = vm.ThoiGianBatDau;
                                task.ThoiGianKetThuc = vm.ThoiGianKetThuc;
                                if (task.TongThoiGian > 0)
                                {
                                    task.StrTotalTime = GetStrTime2(task.TongThoiGian);
                                    if (task.ThoiGianBatDau != null)
                                    {
                                        task.StrTotalTime += " <br/>  <span class=\"text-mute font-11 font-w-300\">(" + task.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm");
                                        if (task.ThoiGianKetThuc != null) task.StrTotalTime += " - " + task.ThoiGianKetThuc.Value.ToString("dd/MM/yyyy HH:mm");
                                        task.StrTotalTime += ")</span>";
                                    }

                                }
                                else if (task.ThoiGianBatDau != null)
                                {
                                    task.StrTotalTime = "Thời gian bắt đâu: " + task.ThoiGianBatDau.Value.ToString("dd/MM/yyyy hh:mm");
                                }


                            }
                            var nguoiIds = _thoiGianLamViecP.GetNguoiDungIdsStartTimeInTask(task.CongViecId);
                            if (nguoiIds != null && nguoiIds.Count > 0)
                            {
                                task.NguoiDungs = new List<NguoiDungViewModel>();
                                foreach (var item in nguoiIds)
                                {
                                    var nguoiDung = _nguoiDungM.GetAvatarNguoiDungById(item);
                                    if (nguoiDung != null && task.NguoiDungs.Count(t => t.NguoiDungId == item) == 0)
                                        task.NguoiDungs.Add(nguoiDung);
                                }
                            }
                            if (lstToreturn.Count(t => t.CongViecId == task.KhoaChaId)==0){
                              var  ship = _manager.GetInfoById(task.KhoaChaId.Value);
                                ship.QuanTam = _manager.getQuanTam(ship.CongViecId, nguoiDungId);
                                ship.CongViecs = new List<CongViecViewModel>() { task };
                                if (ship.NgayCapNhat < task.NgayDuKienHoanThanh) ship.NgayCapNhat = task.NgayDuKienHoanThanh;
                                if (ship.NgayCapNhat < task.NgayTao) ship.NgayCapNhat = task.NgayTao;
                                ship.CongViecs = new List<CongViecViewModel>() { task };

                                lstToreturn.Add(ship);
                            }
                            else
                            {
                                var obj = lstToreturn.FirstOrDefault(x => x.CongViecId == task.KhoaChaId);
                                if (obj.CongViecs == null) obj.CongViecs = new List<CongViecViewModel>() { task };
                                else obj.CongViecs.Add(task);
                                if (obj.NgayCapNhat < task.NgayDuKienHoanThanh) obj.NgayCapNhat = task.NgayDuKienHoanThanh;
                                if (obj.NgayCapNhat < task.NgayTao) obj.NgayCapNhat = task.NgayTao;
                                obj.CongViecs = new List<CongViecViewModel>() { task };
                            }
                        }
                     
                    }
                }





                if (shipIds != null && shipIds.Count > 0)
                {
                    foreach(var id in shipIds)
                    {
                        if (lstToreturn.Count(t => t.CongViecId == id) == 0)
                        {
                           var ship = _manager.GetInfoById(id);
                            if (ship.NgayCapNhat == null || ship.NgayCapNhat < ship.NgayTao) ship.NgayCapNhat = ship.NgayTao;
                            ship.QuanTam = _manager.getQuanTam(ship.CongViecId, nguoiDungId);

                            ship.TongThoiGian = _thoiGianLamViecP.GetToTalTimeSpendInShip(ship.CongViecId);
                            ship.StrTotalTime = GetStrTime(ship.TongThoiGian);
                            lstToreturn.Add(ship);
                        }
                    }
                }
                
            }
            return lstToreturn;
        }

        public CongViecViewModel GetcurrentTask(Guid nguoiDungId)
        {
            var result = _thoiGianLamViecP.GetTaskStartTimeOfUser(nguoiDungId);
            if (result != null)
            {
                int newLog = 0;
                var result2 = new ThoiGianLamViecModel().GetCurrentTodoCountTime(nguoiDungId);
                if (result2 != null)
                {
                    newLog = (int)(DateTime.Now - result2.ThoiGianBatDau).Value.TotalSeconds;

                }

                result.TongThoiGianLog = newLog + _thoiGianLamViecP.GetTotalTimeUsedByUserInCurrentTask(result.CongViecId,nguoiDungId);
            }
            return result;
        }
        public int GetCurrentToDoId(Guid nguoiDungId)
        {
            return _thoiGianLamViecP.CurrentToDoId(nguoiDungId);
        }

        public int InsertShipableToDoWork(CongViecViewModel vm, Guid userId)
        {
            vm.LaShipAble = true;
            vm.NguoiTaoId = userId;
            vm.NguoiXuLyId = userId;
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var week = Helpers.GetNumberWeekOfDay(monday);
            var year = monday.Year;
            vm.Tuan = week;
            vm.Nam = year;
            var check = false;
            if (!string.IsNullOrEmpty(vm.StrNgayDuKienHoanThanh))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrNgayDuKienHoanThanh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.NgayDuKienHoanThanh = dt;
                    check = true;
                }

            }


            if (!check)
            {
                var now = DateTime.Now;
                var month = DateTime.Now.Month;
                while (month % 3 != 0)
                {
                    month += 1;
                }
                vm.NgayDuKienHoanThanh = new DateTime(now.Year, month, 1).AddMonths(1).AddDays(-1);
            }
            vm.TuanHoanThanh = vm.Tuan;
            CauHinhProvider cauHinhM = new CauHinhProvider();
            vm.MaCongViec = cauHinhM.GetMaShipAble().ToString();
            vm.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.shipablePlan;
            var id = _manager.Insert(vm);
            if (id > 0)
            {
                cauHinhM.InsertMaShipable();
               
                //_manager.InsertOrUpdateQuanTam(id, vm.NguoiXuLyId.Value, true);
                //_manager.InsertOrUpdateQuanTam(id, vm.NguoiTaoId, true);
                if (vm.CongViecs != null && vm.CongViecs.Count > 0)
                {
                    foreach (var task in vm.CongViecs)
                    {
                        task.LaShipAble = false;
                        task.LaTask = true;
                        task.DuAnId = vm.DuAnId;

                        task.IsToDoAdd = false;
                        task.KhoaChaId = id;
                        task.Tuan = vm.Tuan;
                        task.NguoiTaoId = userId;
                        task.Nam = vm.Nam;
                        task.LoaiTimer = 1;
                        task.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.congviecNew;
                        var idtask = _manager.Insert(task);
                        if (idtask > 0)
                        {
                            //_manager.InsertOrUpdateQuanTam(idtask, userId, true);
                            //_manager.InsertOrUpdateQuanTam(idtask, task.NguoiXuLyId.Value, true);
                        }
                    }
                }
            }
            return id;
        }

        public bool UpdateNguoiXuLyOfShip(int shipId, Guid nguoiXuLyId)
        {
            return _manager.UpdateNguoiXuLyId(shipId, nguoiXuLyId);
        }

        public int UpdateShipable(CongViecViewModel vm)
        {
            if (vm.StrNgayDuKienHoanThanh != null)
            {
                try
                {
                    vm.NgayDuKienHoanThanh = DateTime.ParseExact(vm.StrNgayDuKienHoanThanh, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                }
                catch
                {
                    vm.NgayDuKienHoanThanh = null;
                }
            }
            if (vm.GiaiDoanDuAnId == 0) vm.GiaiDoanDuAnId = null;
            var result = _manager.UpdateShipAble(vm.CongViecId,vm.TenCongViec,vm.NgayDuKienHoanThanh,vm.DuAnId,vm.GiaiDoanDuAnId,vm.TrangThaiCongViecId,vm.MoTa,vm.NguoiXuLyId);
            if (result) return 1;
            return 0;

        }

        public bool InsertTaskToDoWork(CongViecViewModel vm, Guid userId)
        {
            var tuan = Helpers.GetNumerWeek(DateTime.Now);
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var year = monday.Year;
            if (vm.CongViecId > 0)
            {
                var ship = _manager.GetById(vm.CongViecId);
                if (vm.CongViecs != null && vm.CongViecs.Count > 0)
                {
                    foreach (var task in vm.CongViecs)
                    {
                        task.LaShipAble = false;
                        task.LaTask = true;
                        task.DuAnId = ship.DuAnId;

                        task.IsToDoAdd = false;
                        task.KhoaChaId = ship.CongViecId;
                        task.Tuan = tuan;
                        task.NguoiTaoId = userId;
                        task.Nam = year;
                        task.LoaiTimer = 1;
                        task.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.congviecNew;
                        var idtask = _manager.Insert(task);
                        if (idtask > 0)
                        {
                            //_manager.InsertOrUpdateQuanTam(idtask, userId, true);
                            //_manager.InsertOrUpdateQuanTam(idtask, task.NguoiXuLyId.Value, true);
                        }
                    }
                }
            }
            return true;
        }

        public CongViecViewModel GetInfoShip(int shipId)
        {
            if (shipId > 0)
            {
                var ship = _manager.GetById(shipId);
                ship.CongViecs = _manager.GetTenCongViecsByKhoaChaId(shipId);
                return ship;
            }
            return null;

        }

        public CongViecViewModel GetTaskInfo(int taskId)
        {
            if (taskId > 0)
            {
                var task = _manager.GetDetailTaskById(taskId);
               
                return task;
            }
            return null;
        }

        public CongViecViewModel GetDetailShip(int shipId)
        {
            if (shipId > 0)
            {
                var ship = _manager.GetDetailShipById(shipId);
                ship.CongViecs = _manager.GetTenCongViecsByKhoaChaId(shipId);
                if (ship.CongViecs != null && ship.CongViecs.Count > 0)
                {
                    foreach(var item in ship.CongViecs)
                    {
                        item.TongThoiGian = _thoiGianLamViecP.GetToTalTimeSpendInTask(item.CongViecId);

                        if (item.TongThoiGian > 0)
                        {
                            item.StrTotalTime = GetStrTime2(item.TongThoiGian);
                            var userIds = _thoiGianLamViecP.GetNguoiDungIdsSpendedTimeInTask(item.CongViecId);
                            if (userIds != null && userIds.Count > 0) item.NguoiDungs = _nguoiDungM.GetsByIds(userIds);
                            ship.TongThoiGian += item.TongThoiGian;
                        }
                        else item.StrTotalTime = "";
                    }
                }

                if (ship.TongThoiGian > 0) ship.StrTotalTime = GetStrTime2(ship.TongThoiGian);
                if (ship.NgayDuKienHoanThanh != null) ship.StrNgayDuKienHoanThanh = ship.NgayDuKienHoanThanh.Value.ToString("dd/MM/yyyy");
                return ship;
            }
            return null;

        }

        #endregion




        #region Get
        #region thao tác về shipable
        /// <summary>
        /// Lấy danh sách shipable theo công việc đã được giao trong tuần
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <param name="week"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetShipablesBy(string phoneNumber, int week, int year)
        {
            var nguoiDungId = _nguoiDungM.GetNguoiDungIdByPhoneNumber(phoneNumber);
            var startTime = Helpers.GetFirstMondayOfWeek(year, week);
            var endTime = startTime.AddDays(7).AddTicks(-1);
            return _manager.GetShipables(nguoiDungId, week, year, startTime, endTime);
        }
        #endregion
        #region ToDO
        /// <summary>
        /// Lấy danh sách todo công việc theo khoảng time ( dùng cho lấy danh sách công việc trong tuần để tạo báo cáo cho report.tecotec.vn)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public List<ThoiGianLamViecTrongNgayViewModel> GetToDosInTimeBy(DateTime startDate, DateTime endDate, string userName, string password)
        {
            List<ThoiGianLamViecTrongNgayViewModel> lstToReturn = new List<ThoiGianLamViecTrongNgayViewModel>();
            var nguoiDung = _nguoiDungM.GetUserByUsernameAndPassword(userName, password);
            if (nguoiDung != null)
            {
                while (startDate <= endDate)
                {
                    ThoiGianLamViecTrongNgayViewModel vm = new ThoiGianLamViecTrongNgayViewModel() { NgayLamViec = startDate, TenCongViecs = new List<TenCongViecViewModel>() };
                    var ids = _manager.GetCongViecIdsInTime(nguoiDung.NguoiDungId, startDate, startDate.AddDays(1).AddTicks(-1));
                    if (ids != null && ids.Count > 0)
                    {
                        List<int> congViecIds = new List<int>();
                        foreach (var id in ids)
                        {
                            if (congViecIds.IndexOf(id) == -1)
                            {
                                congViecIds.Add(id);
                                var tenCongViec = _manager.GetTenCongViecById(id);
                                if (vm.TenCongViecs.Count(t => t.TenCongViec.ToUpper().Equals(tenCongViec.TenCongViec.ToUpper())) == 0)
                                {
                                    vm.TenCongViecs.Add(tenCongViec);
                                }
                            }
                        }
                    }
                    lstToReturn.Add(vm);
                    startDate = startDate.AddDays(1);
                }
            }
            return lstToReturn;
        }


        public void test(string file, List<AddThoiGianLamViecWithXMLViewModel> lst)
        {
            //var test = System.Xml.Serialization.XmlSerializer(lst);
            //File.WriteAllText(file, lst, Encoding.Unicode);

            File.WriteAllText(file, JsonConvert.SerializeObject(lst));
        }
        public void testAdd(string file)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            file = path + file + ".xml";
            var str = File.ReadAllText(file);
            var lst = JsonConvert.DeserializeObject<List<AddThoiGianLamViecWithXMLViewModel>>(str);
            if (lst != null && lst.Count > 0)
            {

                foreach (var item in lst)
                {
                    var nguoiDung = _nguoiDungM.GetUserByUsername(item.TenDangNhap);
                    if (nguoiDung != null)
                    {
                        int congViecId = item.CongViecId;
                        if (!_manager.CheckIsExist(item.CongViecId, item.TenCongViec, item.DuAnId))
                        {
                            //Lấy shipable Unknowledge
                            var shipId = _manager.GetShipableIdByName("unknowledged", item.DuAnId);
                            if (shipId == 0) shipId = _manager.Insert(new CongViecViewModel { TenCongViec = "unknowledged", DuAnId = item.DuAnId, Tuan = item.Tuan, Nam = item.Nam, LaShipAble = true, LoaiTimer = 1, NguoiTaoId = nguoiDung.NguoiDungId, NguoiXuLyId = nguoiDung.NguoiDungId, TrangThaiCongViecId = 2, ThuTuUuTien = 2 });
                            // Lấy task Unknowledge
                            var taskId = _manager.GetTaskIdByName("unknowledged", item.DuAnId);
                            if (taskId == 0) taskId = _manager.Insert(new CongViecViewModel { TenCongViec = "unknowledged", DuAnId = item.DuAnId, Tuan = item.Tuan, Nam = item.Nam, LaShipAble = false, LaTask = true, LoaiTimer = 1, KhoaChaId = shipId, NguoiTaoId = nguoiDung.NguoiDungId, NguoiXuLyId = nguoiDung.NguoiDungId, TrangThaiCongViecId = 9 });
                            CongViecViewModel vm = new CongViecViewModel()
                            {
                                TenCongViec = item.TenCongViec,
                                DuAnId = item.DuAnId,
                                KhoaChaId = taskId,
                                LaShipAble = false,
                                LaTask = false,
                                NguoiTaoId = nguoiDung.NguoiDungId,
                                NgayTao = item.NgayTao,
                                NgayDuKienHoanThanh = item.NgayDuKienHoanThanh,
                                NgayLamViec = item.NgayLamViec,
                                NguoiXuLyId = nguoiDung.NguoiDungId,
                                NguoiHoTroId = nguoiDung.NguoiDungId,
                                Tuan = item.Tuan,
                                Nam = item.Nam,
                                LoaiTimer = item.LoaiTimer,
                                TrangThaiCongViecId = item.TrangThaiCongViecId,
                                IsToDoAdd = item.IsToDoAdd
                            };
                            congViecId = _manager.Insert(vm);
                        }
                        if (congViecId > 0)
                        {
                            _thoiGianLamViecP.Insert(congViecId, nguoiDung.NguoiDungId, item.ThoiGianBatDau, item.ThoiGianKetThuc, item.LoaiThoiGian.Value, item.TongThoiGian, item.PheDuyet, null, null, item.TokenId, item.NgayLamViec);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Lấy danh sách Todo theo công việc đã được giao trong tuần theo shipable
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <param name="week"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<CongViecViewModel> GetToDoBy(int shipId, string phoneNumber, int week, int year)
        {
            var nguoiDungId = _nguoiDungM.GetNguoiDungIdByPhoneNumber(phoneNumber);
            var startTime = Helpers.GetFirstMondayOfWeek(year, week);
            var endTime = startTime.AddDays(7).AddTicks(-1);
            return _manager.GetTodosBy(nguoiDungId, shipId, week, year, startTime, endTime);
        }

        #endregion
        public List<TrangThaiCongViecViewModel> GetTrangThaiShipables(int khoaChaId)
        {
            return _trangThaiCCM.GetByKhoaChaId(khoaChaId);
        }
        public List<TrangThaiCongViecViewModel> GetTrangThaiShipablesAll(int? khoaChaId)
        {
            List<TrangThaiCongViecViewModel> lstToReturn = new List<TrangThaiCongViecViewModel>();
            List<TrangThaiCongViecViewModel> lst;
            if (khoaChaId == null)
            {
                lst = _trangThaiCCM.GetByKhoaChaId((int)EnumTrangThaiCongViecType.shipable);

            }
            else
            {
                lst = _trangThaiCCM.GetByKhoaChaId(khoaChaId.Value);
            }
            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    lstToReturn.Add(item);
                    var lst2 = _trangThaiCCM.GetByKhoaChaId(item.TrangThaiCongViecId);
                    if (lst2 != null && lst2.Count > 0)
                    {
                        lstToReturn.AddRange(GetTrangThaiShipablesAll(item.TrangThaiCongViecId));
                    }
                }
            }
            return lstToReturn;
        }

        public List<CongViecViewModel> GetTenShipAbleByDuAnId(int duAnId)
        {
            return _manager.GetTenShipableByDuAnId(duAnId);
        }
        public List<CongViecViewModel> GetTenCongViecByKhoaChaId(int khoaChaId)
        {
            return _manager.GetTenCongViecsByKhoaChaId(khoaChaId);
        }
        public ReportPointViewModel GetReportPointBy(bool phongBan, int year, int month, int week)
        {

            ReportPointViewModel result = new ReportPointViewModel() { HoTenNguoiDungs = new List<string>(), Value = new List<decimal>() };
            NguoiDungModel model = new NguoiDungModel();
            DateTime startDate = Helpers.GetFirstMondayOfMonth(year, month);
            DateTime endDate = startDate.AddDays(7).AddTicks(-1);
            int numberWeek = 1;
            if (week == 0)
            {
                numberWeek = Helpers.GetNumberWeekOfMonth(year, month);
            }
            else
            {
                startDate = startDate.AddDays((week - 1) * 7);
                endDate = endDate.AddDays((week - 1) * 7);
            }

            decimal max = 10;
            if (phongBan)
            {
                result.Title = "Thống kê điểm của các phòng ban TecoTec Công nghệ.";
                PhongBanProvider pbM = new PhongBanProvider();
                var lstPb = pbM.GetByKhoaChaId(1);

                if (lstPb != null && lstPb.Count > 0)
                {
                    foreach (var item in lstPb)
                    {
                        decimal point = 0;
                        var lstUser = _nguoiDungM.GetNguoiDungsByPhongBanId(item.PhongBanId);
                        if (lstUser != null && lstUser.Count > 0)
                        {
                            foreach (var user in lstUser)
                            {
                                var startTime = startDate.AddDays(-7);
                                var endTime = endDate.AddDays(-7);
                                for (int i = 1; i <= numberWeek; i++)
                                {
                                    decimal value = 0;
                                    startTime = startTime.AddDays(7);
                                    endTime = endTime.AddDays(7);
                                    decimal numberTaskInWeek = (decimal)_manager.CountCongViecOfUserBy(user.NguoiDungId, startTime, endTime, 0);
                                    if (numberTaskInWeek > 0)
                                    {
                                        decimal numberTaskDone = (decimal)_manager.CountCongViecOfUserBy(user.NguoiDungId, startTime, endTime, (int)EnumTrangThaiCongViecType.congViecDone);
                                        value = 6 * numberTaskDone / numberTaskInWeek;
                                    }
                                    value = Math.Round(value, 2);
                                    point += value;
                                    if (point > 10 && point > max) max = point + 5;

                                }
                            }
                        }
                        item.Point = point;
                    }
                    List<PhongBanViewModel> SortedList = lstPb.OrderByDescending(o => o.Point).ToList();
                    foreach (var item in SortedList)
                    {
                        result.HoTenNguoiDungs.Add(item.TenPhongBan);
                        result.Value.Add(item.Point);
                    }

                }
            }
            else
            {
                result.Title = "Thống kê điểm của nhân viên TecoTec Công nghệ.";
                List<NguoiDungViewModel> users = new List<NguoiDungViewModel>();
                var users1 = model.GetNguoiDungByLoai(3);
                if (users1 != null && users1.Count > 0) users.AddRange(users1);
                var users2 = model.GetNguoiDungByLoai(4);
                if (users2 != null && users2.Count > 0) users.AddRange(users2);
                DiemPointProvider manager = new DiemPointProvider();
                var lstHoten = new List<string>();
                var lstValue = new List<int>();
                //var users = model.GetNguoiDungByLoai(3);              
                if (users != null && users.Count > 0)
                {
                    foreach (var item in users)
                    {
                        var startTime = startDate.AddDays(-7);
                        var endTime = endDate.AddDays(-7);
                        for (int i = 1; i <= numberWeek; i++)
                        {
                            decimal value = 0;
                            startTime = startTime.AddDays(7);
                            endTime = endTime.AddDays(7);
                            decimal numberTaskInWeek = (decimal)_manager.CountCongViecOfUserBy(item.NguoiDungId, startTime, endTime, 0);
                            if (numberTaskInWeek > 0)
                            {
                                decimal numberTaskDone = (decimal)_manager.CountCongViecOfUserBy(item.NguoiDungId, startTime, endTime, (int)EnumTrangThaiCongViecType.congViecDone);
                                value = 6 * numberTaskDone / numberTaskInWeek;
                            }
                            value = Math.Round(value, 2);
                            if (value > 10 && value > max) max = value + 5;
                            item.Point += value;
                        }

                    }
                    List<NguoiDungViewModel> SortedList = users.OrderByDescending(o => o.Point).ToList();
                    foreach (var item in SortedList)
                    {
                        result.HoTenNguoiDungs.Add(item.HoTen);
                        result.Value.Add(item.Point);
                    }
                }

            }
            result.Max = max;
            return result;
        }
        public List<TrangThaiCongViecViewModel> GetTrangThaiCongViecs()
        {
            return _trangThaiCCM.GetByKhoaChaId((int)EnumTrangThaiCongViecType.congviec);
        }
        public TrangThaiCongViecViewModel GetTTCVById(int id)
        {
            return _trangThaiCCM.GetById(id);
        }
        public CongViecViewModel GetById(int congViecId)
        {
            return _manager.GetById(congViecId);

        }
        public CongViecViewModel GetById2(int congViecId)
        {
            var item = _manager.GetById2(congViecId);
            if (item != null)
            {
                LoaiCongViecProvider loaiCVM = new LoaiCongViecProvider();
                item = GetDoPhucTapAndThuTuUuTien(item);
                item.CongViecs = _manager.GetToDoBy(congViecId);
                item.LoaiCongViecIds = loaiCVM.GetIdsByCongViecId(congViecId);
                if (item.NgayDuKienHoanThanh != null)
                {
                    item.StrNgayDuKienHoanThanhFull = String.Format("{0:dd/MM/yyyy}", item.NgayDuKienHoanThanh);
                    item.StrNgayDuKienHoanThanh = item.StrNgayDuKienHoanThanhFull.Substring(0, 5);
                }
                if (item.CongViecs != null && item.CongViecs.Count > 0)
                {
                    foreach (var item2 in item.CongViecs)
                    {
                        if (item2.NgayDuKienHoanThanh != null)
                        {
                            item2.StrNgayDuKienHoanThanh = String.Format("{0:dd/MM/yyyy}", item2.NgayDuKienHoanThanh);
                        }
                        item2.LoaiCongViecIds = loaiCVM.GetIdsByCongViecId(item2.CongViecId);
                    }
                }
                item.TrangThaiCongViec = _manager.GetTrangThaiById(item.TrangThaiCongViecId);
                var lst = _manager.GetsByKhoaChaId(item.CongViecId);
                if (lst != null && lst.Count > 0) item.XuLyVaoTuanTiepTheo = true;
                else item.XuLyVaoTuanTiepTheo = false;
            }
            return item;
        }

        public CongViecViewModel GetById3(int congViecId, Guid nguoiDungId)
        {
            var vm = _manager.GetById2(congViecId);

            if (vm != null)
            {
                NoiDungProvider noiDungP = new NoiDungProvider();
                vm.Comments = noiDungP.GetsBy(congViecId.ToString(), (byte)EnumLoaiNoiDungType.CommentDuAnAndShip, (byte)EnumItemTypeType.ShipAbleType);
                if (vm.Comments != null && vm.Comments.Count > 0)
                {
                    foreach (var comment in vm.Comments)
                    {
                        if (comment.NguoiDungId == nguoiDungId) comment.Edit = true;
                    }
                }
            }
            return vm;
        }
        public CongViecViewModel GetShipableById(int congViecId)
        {
            var item = _manager.GetById2(congViecId);
            if (item != null)
            {
                LoaiCongViecProvider loaiCVM = new LoaiCongViecProvider();
                item = GetDoPhucTapAndThuTuUuTien(item);
                item.CongViecs = _manager.GetToDoBy(congViecId);
                item.LoaiCongViecIds = loaiCVM.GetIdsByCongViecId(congViecId);
                if (item.NgayDuKienHoanThanh != null)
                {
                    item.StrNgayDuKienHoanThanhFull = String.Format("{0:dd/MM/yyyy}", item.NgayDuKienHoanThanh);
                    item.StrNgayDuKienHoanThanh = item.StrNgayDuKienHoanThanhFull.Substring(0, 5);
                }
                if (item.CongViecs != null && item.CongViecs.Count > 0)
                {
                    foreach (var item2 in item.CongViecs)
                    {
                        if (item2.NgayDuKienHoanThanh != null)
                        {
                            item2.StrNgayDuKienHoanThanh = String.Format("{0:dd/MM/yyyy}", item2.NgayDuKienHoanThanh);
                        }
                        item2.LoaiCongViecIds = loaiCVM.GetIdsByCongViecId(item2.CongViecId);
                    }
                }
                item.TrangThaiCongViec = _manager.GetTrangThaiById(item.TrangThaiCongViecId);
                var lst = _manager.GetsByKhoaChaId(item.CongViecId);
                if (lst != null && lst.Count > 0) item.XuLyVaoTuanTiepTheo = true;
                else item.XuLyVaoTuanTiepTheo = false;
            }
            return item;
        }
        private CongViecViewModel GetShipableCha(int khoaChaId)
        {
            var congViecVm = _manager.GetById(khoaChaId);
            if (congViecVm.KhoaChaId != null) return GetShipableById(congViecVm.KhoaChaId.Value);
            return congViecVm;
        }
        private CongViecViewModel GetDoPhucTapAndThuTuUuTien(CongViecViewModel item)
        {
            if (item != null)
            {
                if (item.ThuTuUuTien != null)
                {
                    switch (item.ThuTuUuTien)
                    {
                        case 1:
                            item.TenThuTuUuTien = "Thấp";
                            item.MaMauThuTuUuTien = "<i class=\"fa fa-arrow-up text-secondary\"></i>";
                            break;
                        case 2:
                            item.TenThuTuUuTien = "Trung bình";
                            item.MaMauThuTuUuTien = "<i class=\"fa fa-arrow-up text-info\"></i>";
                            break;
                        case 3:
                            item.TenThuTuUuTien = "Cao";
                            item.MaMauThuTuUuTien = "<i class=\"fa fa-arrow-up text-danger\"></i>";
                            break;
                        default:
                            break;
                    }
                }
                if (item.DoPhucTap != null)
                {
                    switch (item.DoPhucTap)
                    {
                        case 1:
                            item.TenDoPhucTap = "Dễ";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#ECECEC\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        case 2:
                            item.TenDoPhucTap = "Trung bình";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#67BF7F\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        case 3:
                            item.TenDoPhucTap = "Khó";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#F86B6B\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        default:
                            break;
                    }
                }
            }

            return item;
        }
        public List<CongViecViewModel> GetsBy(int duAnId, int tuan)
        {

            return _manager.GetsBy(duAnId, tuan);
        }
        public ShipAbleViewModel GetBy(int? duAnId, int tuan, int nam, List<int> trangThaiCongViecId, Guid nguoiDungId, int? teamId)
        {
            ShipAbleViewModel result = new ShipAbleViewModel() { Tuan = tuan, Nam = nam, ShipAble2Indexs = new List<ShipAble2IndexViewModel>() };
            DiemPointProvider diemP = new DiemPointProvider();
            LoaiGiaiDoanDuAnProvider loaiGiaiDoanM = new LoaiGiaiDoanDuAnProvider();
            List<Guid> nguoiDungIds = new List<Guid>();
            if (nguoiDungId != Guid.Empty)
            {
                nguoiDungIds.Add(nguoiDungId);
            }
            else
            {
                if (teamId == null)
                {
                    var lstUser = _nguoiDungM.GetAll();
                    if (lstUser != null && lstUser.Count > 0) foreach (var user in lstUser) nguoiDungIds.Add(user.NguoiDungId);
                }
                else
                {
                    var lstUser = _nguoiDungM.GetNguoiDungsByPhongBanId(teamId.Value);
                    if (lstUser != null && lstUser.Count > 0) foreach (var user in lstUser) nguoiDungIds.Add(user.NguoiDungId);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                ShipAble2IndexViewModel vm = new ShipAble2IndexViewModel();
                var nextNam = false;
                if (tuan <= 0)
                {
                    vm.Nam = nam - 1;
                    nextNam = true;
                    vm.Tuan = Helpers.GetNumberWeekOfYears(vm.Nam);
                    tuan = vm.Tuan;
                }
                else
                {
                    vm.Tuan = tuan;
                    vm.Nam = nam;
                }
                vm.Shipables = _manager.GetsBy(duAnId, vm.Tuan, vm.Nam, trangThaiCongViecId, nguoiDungIds);
                if (vm.Shipables != null && vm.Shipables.Count > 0)
                {

                    foreach (var item in vm.Shipables)
                    {
                        switch (item.ThuTuUuTien)
                        {
                            case 1:
                                item.TenThuTuUuTien = "Thấp";
                                item.MaMauThuTuUuTien = "low.svg";
                                break;
                            case 2:
                                item.TenThuTuUuTien = "Trung bình";
                                item.MaMauThuTuUuTien = "medium.svg";
                                break;
                            case 3:
                                item.TenThuTuUuTien = "Cao";
                                item.MaMauThuTuUuTien = "high.svg";
                                break;
                            default:
                                break;
                        }
                        if (item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDone)
                        {
                            result.ShipAbleHoanThanhTuanHienTai++;
                        }
                        if (item.NgayDuKienHoanThanh != null) item.StrNgayDuKienHoanThanhFull = String.Format("{0:dd/MM/yyyy}", item.NgayDuKienHoanThanh);

                        if (item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDone)
                        {
                            if (item.KhoaChaId != null)
                            {
                                var vm2 = GetShipableCha(item.KhoaChaId.Value);
                                item.TenCongViecMute += " Nợ từ tuần " + vm2.Tuan + " Đến tuần " + item.Tuan;
                            }
                            else if (item.TuanHoanThanh != item.Tuan && item.TuanHoanThanh != null)
                            {
                                item.TenCongViecMute += " Làm từ tuần " + item.Tuan + " đến tuần " + item.TuanHoanThanh;
                            }
                        }
                        else
                        {
                            if (item.KhoaChaId != null)
                            {
                                var vm2 = GetShipableCha(item.KhoaChaId.Value);
                                item.TenCongViecMute += " Nợ từ tuần " + vm2.Tuan;
                            }
                            if (item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableContinue)
                            {
                                item.TenCongViecMute += " Làm từ tuần " + item.Tuan + " đến tuần " + item.TuanHoanThanh;
                            }
                        }
                        if (item.LoaiGiaiDoanId != null)
                        {
                            var loaiGiaiDoan = loaiGiaiDoanM.GetById(item.LoaiGiaiDoanId.Value);
                            if (loaiGiaiDoan != null)
                            {
                                item.TenGiaiDoanActive = loaiGiaiDoan.TenLoaiGiaiDoan;
                                item.MaMauLoaiGiaiDoan = loaiGiaiDoan.MaMau;
                            }
                        }

                        item.DiemPoint = diemP.GetPointOfShipable(item.CongViecId);
                    }
                }
                result.ShipAble2Indexs.Add(vm);
                tuan--;
                if (nextNam) nam--;
            }
            return result;
        }
        public List<ShipAbleInWeekViewModel> GetShipablesInWeekBy(int? duAnId, List<int> trangThaiCongViecIds, List<Guid> nguoiDungIds, DateTime startTime, DateTime endTime)
        {
            List<ShipAbleInWeekViewModel> lstToRerturn = new List<ShipAbleInWeekViewModel>();
            DiemPointProvider diemP = new DiemPointProvider();
            LoaiGiaiDoanDuAnProvider loaiGiaiDoanM = new LoaiGiaiDoanDuAnProvider();
            var weekF = Helpers.GetNumerWeek(startTime);
            var mondayF = Helpers.GetMonDayBy(startTime);
            var yearF = mondayF.Year;
            var weekT = Helpers.GetNumerWeek(endTime);
            var mondayT = Helpers.GetMonDayBy(startTime);
            var yearT = mondayF.Year;
            List<TuanViewModel> lstTuan = new List<TuanViewModel>();
            for (int i = yearF; i <= yearT; i++)
            {
                if (i < yearT)
                {
                    var maxWeek = Helpers.GetNumberWeekOfYears(i);
                    for (int j = weekT; j <= maxWeek; j++)
                    {
                        lstTuan.Insert(0, new TuanViewModel() { week = j, year = i });
                    }
                    weekT = 1;
                }

                if (i == yearT)
                {
                    for (int j = weekF; j <= weekT; j++)
                    {
                        lstTuan.Insert(0, new TuanViewModel() { week = j, year = i });
                    }
                }
            }
            if (lstTuan != null && lstTuan.Count > 0)
            {
                foreach (var tuan in lstTuan)
                {
                    ShipAbleInWeekViewModel vm = new ShipAbleInWeekViewModel() { Nam = tuan.year, Tuan = tuan.week, DisplayTuan = getDisplayKhoangThoiGianTrongTuan(tuan.week, tuan.year) };
                    vm.Shipables = _manager.GetsBy(duAnId, tuan.week, tuan.year, trangThaiCongViecIds, nguoiDungIds);
                    if (vm.Shipables != null && vm.Shipables.Count > 0)
                    {
                        foreach (var item in vm.Shipables)
                        {
                            switch (item.ThuTuUuTien)
                            {
                                case 1:
                                    item.TenThuTuUuTien = "Thấp";
                                    item.MaMauThuTuUuTien = "low.svg";
                                    break;
                                case 2:
                                    item.TenThuTuUuTien = "Trung bình";
                                    item.MaMauThuTuUuTien = "medium.svg";
                                    break;
                                case 3:
                                    item.TenThuTuUuTien = "Cao";
                                    item.MaMauThuTuUuTien = "high.svg";
                                    break;
                                default:
                                    break;
                            }

                            if (item.NgayDuKienHoanThanh != null) item.StrNgayDuKienHoanThanhFull = String.Format("{0:dd/MM/yyyy}", item.NgayDuKienHoanThanh);

                            if (item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDone)
                            {
                                if (item.KhoaChaId != null)
                                {
                                    var vm2 = GetShipableCha(item.KhoaChaId.Value);
                                    item.TenCongViecMute += " Nợ từ tuần " + vm2.Tuan + " Đến tuần " + item.Tuan;
                                }
                                else if (item.TuanHoanThanh != item.Tuan && item.TuanHoanThanh != null)
                                {
                                    item.TenCongViecMute += " Làm từ tuần " + item.Tuan + " đến tuần " + item.TuanHoanThanh;
                                }
                            }
                            else
                            {
                                if (item.KhoaChaId != null)
                                {
                                    var vm2 = GetShipableCha(item.KhoaChaId.Value);
                                    item.TenCongViecMute += " Nợ từ tuần " + vm2.Tuan;
                                }
                                if (item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableContinue)
                                {
                                    item.TenCongViecMute += " Làm từ tuần " + item.Tuan + " đến tuần " + item.TuanHoanThanh;
                                }
                            }
                            if (item.LoaiGiaiDoanId != null)
                            {
                                var loaiGiaiDoan = loaiGiaiDoanM.GetById(item.LoaiGiaiDoanId.Value);
                                if (loaiGiaiDoan != null)
                                {
                                    item.TenGiaiDoanActive = loaiGiaiDoan.TenLoaiGiaiDoan;
                                    item.MaMauLoaiGiaiDoan = loaiGiaiDoan.MaMau;
                                }
                            }

                            item.DiemPoint = diemP.GetPointOfShipable(item.CongViecId);
                        }
                    }
                    lstToRerturn.Add(vm);
                }
            }

            return lstToRerturn;
        }
        private string getDisplayKhoangThoiGianTrongTuan(int tuan, int nam)
        {
            var result = "";
            var monday = Helpers.GetFirstMondayOfWeek(nam, tuan);
            var last = monday.AddDays(6);
            if (monday.Year != last.Year)
            {
                result = monday.ToString("dd/MM/yyyy") + " - " + last.ToString("dd/MM/yyyy");
            }
            else
            {
                if (monday.Month != last.Month)
                {
                    result = monday.ToString("dd/MM") + " - " + last.ToString("dd/MM/yyyy");
                }
                else
                {
                    result = monday.Day + " - " + last.ToString("dd/MM/yyyy");
                }
            }
            return result;
        }
        public ShipAbleViewModel GetBy(int duAnId, int year, int tuan, int trangThaiCongViecId)
        {
            var fromDate = Helpers.GetFirstMondayOfWeek(year, tuan);
            var toDate = fromDate.AddDays(7).AddTicks(-1);
            var fromDate2 = fromDate.AddDays(-7);
            var toDate2 = toDate.AddDays(-7);
            ShipAbleViewModel result = new ShipAbleViewModel() { Tuan = tuan, DuAnId = duAnId, TrangThaiCongViecId = trangThaiCongViecId };
            List<int> lstId = new List<int>();
            if (trangThaiCongViecId > 0) lstId.Add(trangThaiCongViecId);
            result.ShipableTuanHienTai = _manager.GetShipablesNewOrDebitBy(duAnId, tuan, year, lstId);
            List<int> lstShipIdsHT = new List<int>();
            List<int> lstShipIdsTT = new List<int>();
            if (result.ShipableTuanHienTai != null && result.ShipableTuanHienTai.Count > 0)
            {
                foreach (var item in result.ShipableTuanHienTai)
                {
                    lstShipIdsHT.Add(item.CongViecId);
                    switch (item.ThuTuUuTien)
                    {
                        case 1:
                            item.TenThuTuUuTien = "Thấp";
                            item.MaMauThuTuUuTien = "low.svg";
                            item.MauChuThuTuUuTien = "success";
                            break;
                        case 2:
                            item.TenThuTuUuTien = "Trung bình";
                            item.MaMauThuTuUuTien = "medium.svg";
                            item.MauChuThuTuUuTien = "medium";
                            break;
                        case 3:
                            item.TenThuTuUuTien = "Cao";
                            item.MaMauThuTuUuTien = "high.svg";
                            item.MauChuThuTuUuTien = "danger";
                            break;
                        default:
                            break;
                    }
                    NguoiDungProvider nguoiDungM = new NguoiDungProvider();
                    if (item.NguoiXuLyId != null)
                    {
                        var nguoiDungVm = nguoiDungM.GetById(item.NguoiXuLyId.Value);
                        if (nguoiDungVm != null) item.HoTen = nguoiDungVm.HoTen;
                    }
                    var lst = _manager.GetsByKhoaChaId(item.CongViecId);
                    if (lst != null && lst.Count > 0) item.XuLyVaoTuanTiepTheo = true;
                    else item.XuLyVaoTuanTiepTheo = false;
                }
                result.ToDoTuanHienTai = _manager.GetToDoBy(fromDate, toDate, lstShipIdsHT);
            }
            var tuanTruoc = tuan - 1;
            var namOfTuanTruoc = year;
            if (tuan == 1)
            {
                namOfTuanTruoc = year - 1;
                tuanTruoc = Helpers.GetNumberWeekOfYears(namOfTuanTruoc);
            }
            result.TuanTruoc = tuanTruoc;
            result.ShipableTuanTruoc = _manager.GetShipablesNewOrDebitBy(duAnId, tuanTruoc, namOfTuanTruoc, lstId);
            if (result.ShipableTuanTruoc != null && result.ShipableTuanTruoc.Count > 0 && result.ShipableTuanTruoc != null && result.ShipableTuanTruoc.Count > 0)
            {
                foreach (var item in result.ShipableTuanHienTai)
                {
                    lstShipIdsTT.Add(item.CongViecId);
                    switch (item.ThuTuUuTien)
                    {
                        case 1:
                            item.TenThuTuUuTien = "Thấp";
                            item.MaMauThuTuUuTien = "low.svg";
                            item.MauChuThuTuUuTien = "success";
                            break;
                        case 2:
                            item.TenThuTuUuTien = "Trung bình";
                            item.MaMauThuTuUuTien = "medium.svg";
                            item.MauChuThuTuUuTien = "medium";
                            break;
                        case 3:
                            item.TenThuTuUuTien = "Cao";
                            item.MaMauThuTuUuTien = "high.svg";
                            item.MauChuThuTuUuTien = "danger";
                            break;
                        default:
                            break;
                    }
                    NguoiDungProvider nguoiDungM = new NguoiDungProvider();
                    if (item.NguoiXuLyId != null)
                    {
                        var nguoiDungVm = nguoiDungM.GetById(item.NguoiXuLyId.Value);
                        if (nguoiDungVm != null) item.HoTen = nguoiDungVm.HoTen;
                    }
                    var lst = _manager.GetsByKhoaChaId(item.CongViecId);
                    if (lst != null && lst.Count > 0) item.XuLyVaoTuanTiepTheo = true;
                    else item.XuLyVaoTuanTiepTheo = false;
                }
                result.ToDoTuanTruoc = _manager.GetToDoBy(fromDate2, toDate2, lstShipIdsTT);
            }

            if (result.ToDoTuanHienTai != null && result.ToDoTuanHienTai.Count > 0)
            {
                foreach (var item in result.ToDoTuanHienTai)
                {
                    switch (item.ThuTuUuTien)
                    {
                        case 1:
                            item.TenThuTuUuTien = "Thấp";
                            item.MaMauThuTuUuTien = "low.svg";
                            break;
                        case 2:
                            item.TenThuTuUuTien = "Trung bình";
                            item.MaMauThuTuUuTien = "medium.svg";
                            break;
                        case 3:
                            item.TenThuTuUuTien = "Cao";
                            item.MaMauThuTuUuTien = "high.svg";
                            break;
                        default:
                            break;
                    }
                    switch (item.DoPhucTap)
                    {
                        case 1:
                            item.TenDoPhucTap = "Dễ";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#ECECEC\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        case 2:
                            item.TenDoPhucTap = "Trung bình";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#67BF7F\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        case 3:
                            item.TenDoPhucTap = "Khó";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#F86B6B\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        default:
                            break;
                    }
                    if (item.NgayDuKienHoanThanh != null)
                    {
                        item.StrNgayDuKienHoanThanhFull = String.Format("{0:dd/MM/yyyy}", item.NgayDuKienHoanThanh);
                        item.StrNgayDuKienHoanThanh = item.StrNgayDuKienHoanThanhFull.Substring(0, 5);
                    }
                }
            }


            if (result.ToDoTuanTruoc != null && result.ToDoTuanTruoc.Count > 0)
            {
                foreach (var item in result.ToDoTuanTruoc)
                {
                    switch (item.ThuTuUuTien)
                    {
                        case 1:
                            item.TenThuTuUuTien = "Thấp";
                            item.MaMauThuTuUuTien = "low.svg";
                            item.MauChuThuTuUuTien = "success";
                            break;
                        case 2:
                            item.TenThuTuUuTien = "Trung bình";
                            item.MaMauThuTuUuTien = "medium.svg";
                            item.MauChuThuTuUuTien = "medium";
                            break;
                        case 3:
                            item.TenThuTuUuTien = "Cao";
                            item.MaMauThuTuUuTien = "high.svg";
                            item.MauChuThuTuUuTien = "danger";
                            break;
                        default:
                            break;
                    }
                    switch (item.DoPhucTap)
                    {
                        case 1:
                            item.TenDoPhucTap = "Dễ";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#ECECEC\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        case 2:
                            item.TenDoPhucTap = "Trung bình";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#67BF7F\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        case 3:
                            item.TenDoPhucTap = "Khó";
                            item.MaMauDoPhucTap = "<span style=\"font-size:12px;color:#F86B6B\"><i class=\"fas fa-flag\"></i></span>";
                            break;
                        default:
                            break;
                    }
                    if (item.NgayDuKienHoanThanh != null)
                    {
                        item.StrNgayDuKienHoanThanhFull = String.Format("{0:dd/MM/yyyy}", item.NgayDuKienHoanThanh);
                        item.StrNgayDuKienHoanThanh = item.StrNgayDuKienHoanThanhFull.Substring(0, 5);
                    }
                }
            }
            result.Tuan = tuan;
            result.Nam = year;
            result.DayInWeeks = new List<DayInWeekViewModel>();
            for (int i = 0; i < 6; i++)
            {
                var day = fromDate.AddDays(i);
                result.DayInWeeks.Add(new DayInWeekViewModel() { TenDayInWeek = "Thứ " + ((int)day.DayOfWeek + 1), NgayThang = String.Format("{0:dd/MM/yyyy}", day).Substring(0, 5), DayOfWeek = (int)day.DayOfWeek });
            }
            result.StrWeek = "Từ ngày " + String.Format("{0:dd/MM/yyyy}", fromDate).Substring(0, 5) + " đến ngày " + String.Format("{0:dd/MM/yyyy}", toDate).Substring(0, 5);

            result.DayInWeeks2 = new List<DayInWeekViewModel>();
            for (int i = 0; i < 6; i++)
            {
                var day = fromDate2.AddDays(i);
                result.DayInWeeks2.Add(new DayInWeekViewModel() { TenDayInWeek = "Thứ " + ((int)day.DayOfWeek + 1), NgayThang = String.Format("{0:dd/MM/yyyy}", day).Substring(0, 5), DayOfWeek = (int)day.DayOfWeek });
            }
            result.StrWeek2 = "Từ ngày " + String.Format("{0:dd/MM/yyyy}", fromDate2).Substring(0, 5) + " đến ngày " + String.Format("{0:dd/MM/yyyy}", toDate2).Substring(0, 5);
            result.DuAns = _duanProvider.GetAll();
            result.TrangThaiCongViecs = GetTrangThaiShipablesAll(null);
            return result;
        }
        public ToDoInWeekViewModel GetToDoInWeekBy(int? year, int? tuan, int? phongBanId, Guid? userId, List<int> loaiCongViecIds)
        {
            if (year == null || tuan == null)
            {
                tuan = Helpers.GetNumerWeek(DateTime.Now);
                year = Helpers.GetMonDayBy(DateTime.Now).Year;
            }
            DateTime fromDate = Helpers.GetFirstMondayOfWeek(year.Value, tuan.Value);
            DateTime toDate = fromDate.AddDays(7).AddTicks(-1);
            ToDoInWeekViewModel result = new ToDoInWeekViewModel() { PhongBanId = phongBanId, Tuan = tuan.Value, ToDoOfUserInWeek = new List<ToDoOfUserInWeekViewModel>() };
            NguoiDungProvider nguoiDungM = new NguoiDungProvider();
            if (userId != null && userId != Guid.Empty)
            {
                ToDoOfUserInWeekViewModel vm = new ToDoOfUserInWeekViewModel();
                var nguoiDungVm = nguoiDungM.GetById(userId.Value);
                if (nguoiDungVm != null)
                {
                    vm.HoTen = nguoiDungVm.HoTen;
                    vm.Avatar = nguoiDungVm.Avatar;
                }
                vm.week = tuan.Value;
                vm.NguoiDungId = userId.Value;
                vm.CongViecs = _manager.GetToDoBy(fromDate, toDate, userId.Value, loaiCongViecIds);
                foreach (var item in vm.CongViecs)
                {
                    switch (item.ThuTuUuTien)
                    {
                        case 1:
                            item.TenThuTuUuTien = "Thấp";
                            item.MaMauThuTuUuTien = "low.svg";
                            break;
                        case 2:
                            item.TenThuTuUuTien = "Trung bình";
                            item.MaMauThuTuUuTien = "medium.svg";
                            break;
                        case 3:
                            item.TenThuTuUuTien = "Cao";
                            item.MaMauThuTuUuTien = "high.svg";
                            break;
                        default:
                            break;
                    }
                    if (item.NgayDuKienHoanThanh != null)
                    {
                        item.StrNgayDuKienHoanThanhFull = String.Format("{0:dd/MM/yyyy}", item.NgayDuKienHoanThanh);
                        item.StrNgayDuKienHoanThanh = item.StrNgayDuKienHoanThanhFull.Substring(0, 5);
                    }
                }
                result.ToDoOfUserInWeek.Add(vm);
            }
            else
            {
                var userids = nguoiDungM.GetUserIdsBy(phongBanId);
                foreach (var item in userids)
                {
                    ToDoOfUserInWeekViewModel vm = new ToDoOfUserInWeekViewModel();
                    var nguoiDungVm = nguoiDungM.GetById(item);
                    if (nguoiDungVm != null)
                    {
                        vm.HoTen = nguoiDungVm.HoTen;
                        vm.Avatar = nguoiDungVm.Avatar;
                    }
                    vm.week = tuan.Value;
                    vm.NguoiDungId = item;
                    vm.CongViecs = _manager.GetToDoBy(fromDate, toDate, item, loaiCongViecIds);
                    foreach (var item1 in vm.CongViecs)
                    {
                        switch (item1.ThuTuUuTien)
                        {
                            case 1:
                                item1.TenThuTuUuTien = "Thấp";
                                item1.MaMauThuTuUuTien = "low.svg";
                                break;
                            case 2:
                                item1.TenThuTuUuTien = "Trung bình";
                                item1.MaMauThuTuUuTien = "medium.svg";
                                break;
                            case 3:
                                item1.TenThuTuUuTien = "Cao";
                                item1.MaMauThuTuUuTien = "high.svg";
                                break;
                            default:
                                break;
                        }
                        if (item1.NgayDuKienHoanThanh != null)
                        {
                            item1.StrNgayDuKienHoanThanhFull = String.Format("{0:dd/MM/yyyy}", item1.NgayDuKienHoanThanh);
                            item1.StrNgayDuKienHoanThanh = item1.StrNgayDuKienHoanThanhFull.Substring(0, 5);
                        }
                    }
                    result.ToDoOfUserInWeek.Add(vm);
                }
            }
            result.NguoiDungId = userId;
            result.PhongBanId = phongBanId;
            result.DayInWeeks = new List<DayInWeekViewModel>();
            for (int i = 0; i < 6; i++)
            {
                var day = fromDate.AddDays(i);
                result.DayInWeeks.Add(new DayInWeekViewModel() { TenDayInWeek = "Thứ " + ((int)day.DayOfWeek + 1), NgayThang = String.Format("{0:dd/MM/yyyy}", day).Substring(0, 5), DayOfWeek = (int)day.DayOfWeek });
            }
            result.StrWeek = "Từ ngày " + String.Format("{0:dd/MM/yyyy}", fromDate).Substring(0, 5) + " đến ngày " + String.Format("{0:dd/MM/yyyy}", toDate).Substring(0, 5);
            return result;
        }
        public int GetNumerWeek(DateTime dateTime)
        {
            CultureInfo myCI = new CultureInfo("en-US");
            System.Globalization.Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            return myCal.GetWeekOfYear(dateTime, myCWR, myFirstDOW);
        }
        public decimal GetPointUsedInWeek(int week, Guid userId)
        {
            return _manager.GetPointUsedInWeek(week, userId);
        }
        public List<CongViecViewModel> GetShipablesBy(int? duAnId, int tuan)
        {
            return _manager.GetsBy(duAnId, tuan);
        }
        public int GetMaShipable()
        {
            CauHinhProvider manager = new CauHinhProvider();
            return manager.GetMaShipAble();
        }
        public List<CongViecViewModel> GetListCongViecToLogTime(int week, Guid nguoiDungId, string keyword)
        {
            var curentTodo = _thoiGianLamViecP.GetToDoStartTimeOfUser(nguoiDungId);
            var curentTodoId = 0;
            var checkCurrent = true;
            if (curentTodo == null)
            {
                curentTodoId = 0;
                checkCurrent = false;
            }
            else
            {
                curentTodoId = curentTodo.CongViecId;
            }
            var result = GetToDoListBy3(0, nguoiDungId, false, week, keyword, curentTodoId);


            if (result != null && result.Count > 0)
            {
                foreach (var task in result)
                {
                    var duan = _duanProvider.GetById(task.DuAnId);

                    if (task.CongViecs != null && task.CongViecs.Count > 0)
                    {
                        switch (task.ThuTuUuTien)
                        {
                            case 1:
                                task.TenThuTuUuTien = "Thấp";
                                task.MaMauThuTuUuTien = "low.svg";
                                task.MauChuThuTuUuTien = "success";
                                break;
                            case 2:
                                task.TenThuTuUuTien = "Trung bình";
                                task.MaMauThuTuUuTien = "medium.svg";
                                task.MauChuThuTuUuTien = "medium";
                                break;
                            case 3:
                                task.TenThuTuUuTien = "Cao";
                                task.MaMauThuTuUuTien = "high.svg";
                                task.MauChuThuTuUuTien = "danger";
                                break;
                            default:
                                task.TenThuTuUuTien = "Trung bình";
                                task.MaMauThuTuUuTien = "medium.svg";
                                task.MauChuThuTuUuTien = "medium";
                                break;
                        }
                        task.TenDuAn = duan.TenDuAn;

                        var lstTodo = _manager.GetToDosBy(task.CongViecId, null);
                        var total = 0;
                        if (lstTodo != null && lstTodo.Count > 0)
                        {
                            foreach (var todo in lstTodo)
                            {
                                todo.TongThoiGianLog = _thoiGianLamViecP.GetAllTimeSpentOfToDo(todo.CongViecId, null);
                                total += todo.TongThoiGianLog.Value;
                            }
                        }

                        //task.CongViecs = lstTodo;
                        task.TongThoiGianLog = total;

                    }

                }
            }
            return result;
        }
        public TodoListViewModel GetToDoListBy(int duAnId, Guid nguoiDungId, bool? xacNhanHoanThanh, int week, string keyWord, int pageNum, int pageSize, int year)
        {
            TodoListViewModel result = new TodoListViewModel() { DuAnId = duAnId, NguoiDungId = nguoiDungId, Week = week, KeyWord = keyWord };
            var shipables = _manager.GetShipablesBy(week, duAnId, Guid.Empty);
            List<CongViecViewModel> lstToReturn = new List<CongViecViewModel>();
            ThoiGianLamViecProvider tglvM = new ThoiGianLamViecProvider();
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (week > 0)
            {
                startDate = Helpers.GetFirstMondayOfWeek(year, week);
                endDate = startDate.AddDays(7).AddTicks(-1);
            }
            if (shipables != null && shipables.Count > 0)
            {
                if (shipables.Count > 0)
                {
                    foreach (var item in shipables)
                    {
                        var duAn = _duanProvider.GetById(item.DuAnId);
                        item.CongViecs = _manager.GetTasksBy(item.CongViecId, week, duAnId, nguoiDungId, keyWord, year, startDate, endDate);

                        item.TenDuAn = duAn.TenDuAn;
                        var ten = item.TenCongViec;
                        if (item.CongViecs != null && item.CongViecs.Count > 0)
                        {

                            foreach (var item2 in item.CongViecs)
                            {
                                item2.CongViecs = _manager.GetToDosBy(item2.CongViecId, xacNhanHoanThanh);
                                var taskTimeSpent = 0;
                                if (item2.CongViecs != null && item2.CongViecs.Count > 0)
                                {
                                    foreach (var item3 in item2.CongViecs)
                                    {
                                        var timespent = tglvM.GetAllTimeSpentOfToDo(item3.CongViecId, true);

                                        if (timespent > 0)
                                        {
                                            taskTimeSpent += timespent;
                                            var hour = timespent / 3600;
                                            var minutes = (timespent % 3600) / 60;
                                            if (taskTimeSpent - (hour * 3600 + minutes * 60) > 0) minutes = minutes + 1;
                                            if (hour > 0) item3.StrThoiGianLamViec += hour + "h";
                                            else item3.StrThoiGianLamViec += "0h";
                                            if (minutes > 0) item3.StrThoiGianLamViec += minutes;
                                        }
                                        if (!string.IsNullOrEmpty(item3.HoTen))
                                        {
                                            var col = item3.HoTen.Split(' ');
                                            if (col.Length > 0)
                                            {
                                                var str = col[col.Length - 1].ToString();
                                                if (str.Length > 0) item3.HoTen = str.Substring(0, 1);
                                            }
                                        }
                                    }
                                }
                                int taskEstimase = 0;
                                if (item2.ThoiGianUocLuong != null) taskEstimase = (int)item2.ThoiGianUocLuong * 3600;
                                var taskTime = taskEstimase - taskTimeSpent;
                                if (taskTime < 0)
                                {
                                    item2.StrThoiGianLamViec += "-";
                                    taskTime = -taskTime;
                                }
                                if (taskTime > 0)
                                {

                                    var hour = taskTime / 3600;
                                    var minutes = (taskTime % 3600) / 60;
                                    if (taskTime - (hour * 3600 + minutes * 60) > 0) minutes = minutes + 1;
                                    if (hour > 0) item2.StrThoiGianLamViec += hour + "h";
                                    else item2.StrThoiGianLamViec += "0h";
                                    if (minutes > 0) item2.StrThoiGianLamViec += minutes;
                                    else item2.StrThoiGianLamViec += minutes;
                                }
                            }
                            lstToReturn.Add(item);
                        }
                    }
                }
                result.Count = lstToReturn.Count;
                result.PageList = lstToReturn.ToPagedList(pageNum, pageSize);
                var skip = (pageNum - 1) * pageSize;
                lstToReturn = lstToReturn.Skip(skip).Take(pageSize).ToList();
            }
            result.PageNum = pageNum;
            result.PageSize = pageSize;
            result.ShipAbles = lstToReturn;
            if (xacNhanHoanThanh == null) result.XacNhanHoanThanh = 0;
            else if (!xacNhanHoanThanh.Value) result.XacNhanHoanThanh = 1;
            else result.XacNhanHoanThanh = 2;
            return result;
        }
        public TodoListViewModel GetToDoListBy2(int duAnId, Guid nguoiDungId, bool? xacNhanHoanThanh, int week, string keyWord, int currentToDo)
        {
            TodoListViewModel result = new TodoListViewModel() { DuAnId = duAnId, NguoiDungId = nguoiDungId, Week = week, KeyWord = keyWord };
            var shipables = _manager.GetShipablesBy(week, duAnId, Guid.Empty);
            List<CongViecViewModel> lstHienThi = new List<CongViecViewModel>();
            ThoiGianLamViecProvider tglvM = new ThoiGianLamViecProvider();
            if (shipables != null && shipables.Count > 0)
            {
                foreach (var item in shipables)
                {
                    if (item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDone || item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDebit || item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDebit) item.XacNhanHoanThanh = true;
                    var lstCongViecs = _manager.GetTasksBy2(item.CongViecId, week, duAnId, nguoiDungId, keyWord);
                    item.CongViecs = new List<CongViecViewModel>();
                    if (lstCongViecs != null && lstCongViecs.Count > 0)
                    {
                        foreach (var item2 in lstCongViecs)
                        {
                            if (item2.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.congViecDone || item2.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.congViecBlock) item2.XacNhanHoanThanh = true;
                            item2.CongViecs = _manager.GetToDosBy2(item2.CongViecId, xacNhanHoanThanh, nguoiDungId);

                            var taskTimeSpent = 0;
                            if (item2.CongViecs != null && item2.CongViecs.Count > 0)
                            {
                                foreach (var item3 in item2.CongViecs)
                                {
                                    var timespent = tglvM.GetAllTimeSpentOfToDo(item3.CongViecId, true);

                                    if (timespent > 0)
                                    {
                                        taskTimeSpent += timespent;
                                        var hour = timespent / 3600;
                                        var minutes = (timespent % 3600) / 60;
                                        if (hour > 0) item3.StrThoiGianLamViec += hour + "h";
                                        if (minutes > 0) item3.StrThoiGianLamViec += minutes;
                                    }
                                    if (!string.IsNullOrEmpty(item3.HoTen))
                                    {
                                        var col = item3.HoTen.Split(' ');
                                        if (col.Length > 0)
                                        {
                                            var str = col[col.Length - 1].ToString();
                                            if (str.Length > 0) item3.HoTen = str.Substring(0, 1);
                                        }
                                    }
                                    if (item3.CongViecId == currentToDo)
                                    {
                                        item.XacNhanHoanThanh = false;
                                        item2.XacNhanHoanThanh = false;
                                    }
                                }
                            }
                            int taskEstimase = 0;
                            if (item2.ThoiGianUocLuong != null) taskEstimase = (int)item2.ThoiGianUocLuong * 3600;
                            var taskTime = taskEstimase - taskTimeSpent;
                            if (taskTime < 0)
                            {
                                item2.StrThoiGianLamViec += "-";
                                taskTime = -taskTime;
                            }
                            if (taskTime > 0)
                            {

                                var hour = taskTime / 3600;
                                var minutes = (taskTime % 3600) / 60;
                                if (hour > 0) item2.StrThoiGianLamViec += hour + "h";
                                if (minutes > 0) item2.StrThoiGianLamViec += minutes;
                            }
                            if (item2.CongViecs != null && item2.CongViecs.Count > 0)
                            {
                                item.CongViecs.Add(item2);
                            }
                        }
                    }
                    if (item.XacNhanHoanThanh != true && item.CongViecs.Count > 0)
                    {
                        lstHienThi.Add(item);
                    }
                }
            }
            result.ShipAbles = lstHienThi;
            if (xacNhanHoanThanh == null) result.XacNhanHoanThanh = 0;
            else if (!xacNhanHoanThanh.Value) result.XacNhanHoanThanh = 1;
            else result.XacNhanHoanThanh = 2;
            return result;
        }
        public List<CongViecViewModel> GetToDoListBy3(int duAnId, Guid nguoiDungId, bool? xacNhanHoanThanh, int week, string keyWord, int currentToDo)
        {


            List<CongViecViewModel> lstTask = new List<CongViecViewModel>();
            ThoiGianLamViecProvider tglvM = new ThoiGianLamViecProvider();
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            var startTime = Helpers.GetFirstMondayOfWeek(monday.Year, week);
            var endTime = startTime.AddDays(7).AddTicks(-1);
            var lstToDo = _manager.GetToDosBy(nguoiDungId, startTime, endTime, currentToDo);
            if (lstToDo != null && lstToDo.Count > 0)
            {
                List<int> ids = new List<int>();
                foreach (var todo in lstToDo)
                {
                    var timespent = tglvM.GetAllTimeSpentOfToDo(todo.CongViecId, true);
                    if (timespent > 0)
                    {
                        //taskTimeSpent += timespent;
                        var hour = timespent / 3600;
                        var minutes = (timespent % 3600) / 60;
                        if (hour > 0) todo.StrThoiGianLamViec += hour + "h";
                        if (minutes > 0) todo.StrThoiGianLamViec += minutes;
                    }
                    todo.TongThoiGianLog = _thoiGianLamViecP.GetAllTimeSpentOfToDo(todo.CongViecId, null);
                    if (!string.IsNullOrEmpty(todo.HoTen))
                    {
                        var col = todo.HoTen.Split(' ');
                        if (col.Length > 0)
                        {
                            var str = col[col.Length - 1].ToString();
                            if (str.Length > 0) todo.HoTen = str.Substring(0, 1);
                        }
                    }

                    if (todo.KhoaChaId != null && ids.IndexOf(todo.KhoaChaId.Value) == -1 && todo.KhoaChaId.Value > 0)
                    {
                        var task = _manager.GetById(todo.KhoaChaId.Value);
                        if (todo.CongViecId == currentToDo)
                        {
                            task.XacNhanHoanThanh = false;
                            task.IsStart = true;
                        }
                        else
                        {
                            if (todo.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.congViecDone)
                            {
                                task.XacNhanHoanThanh = true;
                            }
                            else
                            {
                                task.XacNhanHoanThanh = false;
                            }
                        }
                        ids.Add(task.CongViecId);
                        lstTask.Add(task);
                    }
                }
            }
            if (lstTask != null && lstTask.Count > 0)
            {

                foreach (var task in lstTask)
                {
                    foreach (var todo in lstToDo)
                    {
                        if (todo.KhoaChaId == task.CongViecId)
                        {
                            if (task.CongViecs == null) task.CongViecs = new List<CongViecViewModel>() { todo };
                            else task.CongViecs.Add(todo);
                        }
                    }
                }

            }
            return lstTask;
        }
        public List<CongViecViewModel> GetShipablesBy2(int duAnId, Guid nguoiDungId, int week)
        {
            var shipables = _manager.GetShipablesBy(week, duAnId, Guid.Empty);
            List<CongViecViewModel> lstHienThi = new List<CongViecViewModel>();
            if (shipables != null && shipables.Count > 0)
            {
                foreach (var item in shipables)
                {
                    if (item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDone || item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDebit || item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDebit) item.XacNhanHoanThanh = true;
                    var lstCongViecs = _manager.GetTasksBy3(item.CongViecId, duAnId, nguoiDungId, null, (int)EnumTrangThaiCongViecType.congViecDone);
                    item.CongViecs = new List<CongViecViewModel>();
                    if (lstCongViecs != null && lstCongViecs.Count > 0)
                    {
                        foreach (var item2 in lstCongViecs)
                        {
                            if (item2.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.congViecDone) item2.XacNhanHoanThanh = true;
                            item.CongViecs.Add(item2);
                        }
                    }
                    if (item.XacNhanHoanThanh != true && item.CongViecs.Count > 0)
                    {
                        lstHienThi.Add(item);
                    }
                }
            }
            return lstHienThi;
        }
        public List<CongViecViewModel> GetShipablesBy3(int duAnId, Guid nguoiDungId, int week, int year)
        {
            int fromWeek = 0;
            int fromYear = year;
            if (week < 9)
            {
                fromYear = year - 1;
                var maxweek = Helpers.GetNumberWeekOfYears(fromYear);
                fromWeek = maxweek + week - 8;
            }
            else fromWeek = week - 8;

            var shipables = _manager.GetShipablesBy2(fromWeek, fromYear, week, year, duAnId, Guid.Empty);
            List<CongViecViewModel> lstHienThi = new List<CongViecViewModel>();
            if (shipables != null && shipables.Count > 0)
            {
                foreach (var item in shipables)
                {
                    //if (item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDone || item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDebit || item.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDebit) item.XacNhanHoanThanh = true;
                    //var lstCongViecs = _manager.GetTasksBy3(item.CongViecId, duAnId, nguoiDungId, null, (int)EnumTrangThaiCongViecType.congViecDone);
                    var lstCongViecs = _manager.GetTasksBy3(item.CongViecId, duAnId, nguoiDungId, null, 0);
                    item.CongViecs = new List<CongViecViewModel>();
                    if (lstCongViecs != null && lstCongViecs.Count > 0)
                    {
                        foreach (var item2 in lstCongViecs)
                        {
                            //if (item2.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.congViecDone) item2.XacNhanHoanThanh = true;
                            item.CongViecs.Add(item2);
                        }
                    }
                    //if (item.XacNhanHoanThanh != true && item.CongViecs.Count > 0)
                    if (item.CongViecs.Count > 0)
                    {
                        lstHienThi.Add(item);
                    }
                }
            }
            return lstHienThi;
        }
        public List<CongViecViewModel> GetTasksBy(int duAnId, int congViecId, Guid nguoiDungId, int week)
        {
            return _manager.GetTasksBy3(congViecId, duAnId, nguoiDungId, null, 0);
        }
        public CongViecViewModel GetTaskDetailById(int taskId)
        {
            var result = _manager.GetById2(taskId);
            if (result != null)
            {   // lay danh sach ngay da log time cua task
                var lstDays = _thoiGianLamViecP.GetDatesStartTimeOfTaskId(taskId);
                List<ToDoInDayViewModel> lst = new List<ToDoInDayViewModel>();
                List<int> lstTodos = new List<int>();
                var timeDone = 0;
                var timeTotal = 0;
                foreach (var item in lstDays)
                {
                    ToDoInDayViewModel vm = new ToDoInDayViewModel();
                    vm.Day = item;
                    // Lay danh sach congviecId da chay time trong ngay
                    var lstIds = _manager.GetIdsBy2(taskId, item);
                    vm.CongViecs = new List<CongViecViewModel>();

                    foreach (var item2 in lstIds)
                    {
                        if (lstTodos.IndexOf(item2) == -1) lstTodos.Add(item2);
                        var cv = _manager.GetById4(item2);
                        cv.LoaiCongViecs = _loaiCongViecP.GetsByCongViecId(item2);
                        if (cv.LoaiCongViecs != null && cv.LoaiCongViecs.Count > 0) cv.TenLoaiCongViec = cv.LoaiCongViecs[0].TenLoaiCongViec;
                        var timeInWork = _thoiGianLamViecP.GetTotalTimeOfToDoInDay(item2, item, true, 1);
                        if (timeInWork > 0)
                        {
                            cv.TongThoiGian = timeInWork;
                            cv.LoaiThoiGian = 1;
                            timeDone += timeInWork;
                            timeTotal += timeInWork;
                            cv.StrThoiGianLamViec = GetStrTime(timeInWork);
                            cv.PheDuyet = true;
                            vm.CongViecs.Add(cv);
                            var timeOutWork1 = _thoiGianLamViecP.GetTotalTimeOfToDoInDay(item2, item, true, 2);
                            var timeOutWork2 = _thoiGianLamViecP.GetTotalTimeOfToDoInDay(item2, item, false, 2);
                            if (timeOutWork1 > 0 || timeOutWork2 > 0)
                            {
                                var cv0 = _manager.GetById4(item2);
                                cv0.LoaiCongViecs = _loaiCongViecP.GetsByCongViecId(item2);
                                if (cv.LoaiCongViecs != null && cv0.LoaiCongViecs.Count > 0) cv0.TenLoaiCongViec = cv0.LoaiCongViecs[0].TenLoaiCongViec;
                                cv0.LoaiThoiGian = 2;
                                if (timeOutWork1 > 0)
                                {
                                    cv0.TongThoiGian = timeOutWork1;
                                    cv0.StrThoiGianLamViec = GetStrTime(timeOutWork1);
                                    cv0.PheDuyet = true;
                                    timeDone += timeOutWork1;
                                    timeTotal += timeOutWork1;
                                }
                                else
                                {
                                    cv0.TongThoiGian = timeOutWork2;
                                    cv0.StrThoiGianLamViec = GetStrTime(timeOutWork2);
                                    cv0.PheDuyet = false;
                                    timeTotal += timeOutWork2;
                                }
                                vm.CongViecs.Add(cv0);
                            }
                        }
                        else
                        {
                            var timeOutWork1 = _thoiGianLamViecP.GetTotalTimeOfToDoInDay(item2, item, true, 2);
                            var timeOutWork2 = _thoiGianLamViecP.GetTotalTimeOfToDoInDay(item2, item, false, 2);
                            if (timeOutWork1 > 0 || timeOutWork2 > 0)
                            {
                                cv.LoaiThoiGian = 2;
                                if (timeOutWork1 > 0)
                                {
                                    cv.TongThoiGian = timeOutWork1;
                                    cv.StrThoiGianLamViec = GetStrTime(timeOutWork1);
                                    cv.PheDuyet = true;
                                    timeDone += timeOutWork1;
                                    timeTotal += timeOutWork1;
                                }

                                else
                                {
                                    cv.TongThoiGian = timeOutWork2;
                                    cv.StrThoiGianLamViec = GetStrTime(timeOutWork2);
                                    cv.PheDuyet = false;
                                    timeTotal += timeOutWork2;
                                }
                                vm.CongViecs.Add(cv);
                            }
                            else
                            {

                                var timeEntry1 = _thoiGianLamViecP.GetTotalTimeOfToDoInDay(item2, item, true, 3);
                                var timeEntry2 = _thoiGianLamViecP.GetTotalTimeOfToDoInDay(item2, item, false, 3);
                                if (timeEntry1 > 0 || timeEntry2 > 0)
                                {
                                    cv.LoaiThoiGian = 3;
                                    if (timeEntry1 > 0)
                                    {

                                        cv.TongThoiGian = timeEntry1;
                                        cv.StrThoiGianLamViec = GetStrTime(timeEntry1);
                                        cv.PheDuyet = true;
                                        timeDone += timeEntry1;
                                        timeTotal += timeEntry1;
                                    }
                                    else
                                    {
                                        cv.TongThoiGian = timeEntry2;
                                        cv.StrThoiGianLamViec = GetStrTime(timeEntry2);
                                        cv.PheDuyet = false;
                                        timeTotal += timeEntry2;
                                    }
                                    vm.CongViecs.Add(cv);
                                }
                                else
                                {
                                    cv.LoaiThoiGian = 1;
                                    vm.CongViecs.Add(cv);
                                }
                            }
                        }
                    }
                    lst.Add(vm);
                }
                if (result.ThoiGianUocLuong == null) result.ThoiGianUocLuong = 0;
                result.StrThoiGianLamViec = GetStrTime(timeTotal) + "(" + result.ThoiGianUocLuong + "h)";
                result.TongThoiGian = timeTotal;
                result.StrTotalTime = GetStrTime(timeTotal);
                result.StrTimeDone = GetStrTime(timeDone);
                // Lay danh sach todoId con cua task
                var ids = _manager.GetCongViecIdsByKhoaCha(taskId);
                foreach (var item in ids)
                {
                    if (lstTodos.IndexOf(item) == -1)
                    {
                        lstTodos.Add(item);
                        var cv = _manager.GetById4(item);
                        cv.LoaiCongViecs = _loaiCongViecP.GetsByCongViecId(item);
                        if (cv.LoaiCongViecs != null && cv.LoaiCongViecs.Count > 0) cv.TenLoaiCongViec = cv.LoaiCongViecs[0].TenLoaiCongViec;
                        if (lst.Count == 0 || String.Format("{0:dd/MM/yyyy}", lst[0].Day) != String.Format("{0:dd/MM/yyyy}", DateTime.Now))
                        {
                            lst.Insert(0, new ToDoInDayViewModel() { Day = DateTime.Now, CongViecs = new List<CongViecViewModel>() { cv } });
                        }
                        else
                        {
                            lst[0].CongViecs.Add(cv);
                        }
                    }
                }
                result.ToDoInDays = lst;
                result.TrangThaiCongViecs = _trangThaiCCM.GetByKhoaChaId((int)EnumTrangThaiCongViecType.congviec);
                var loaiCongViecIds = _loaiCongViecP.GetIdsByCongViecId(result.CongViecId);
                if (loaiCongViecIds != null && loaiCongViecIds.Count > 0)
                {
                    List<LoaiCongViecViewModel> lstToUse = new List<LoaiCongViecViewModel>();
                    foreach (var item in loaiCongViecIds)
                    {
                        var lst2 = _loaiCongViecP.GetsByKhoaChaId(item);
                        if (lst2 != null && lst2.Count > 0) lstToUse.AddRange(lst2);
                    }
                    result.LoaiCongViecs = lstToUse;
                }
                result.NguoiDungs = new List<NguoiDungViewModel>();
                if (result.NguoiXuLyId != null)
                {
                    var nguoiDungvm = _nguoiDungM.GetById(result.NguoiXuLyId.Value);
                    if (nguoiDungvm != null)
                    {
                        result.NguoiDungs.Add(nguoiDungvm);
                    }
                }
                if (result.NguoiHoTroId != null)
                {
                    var nguoiDungvm = _nguoiDungM.GetById(result.NguoiHoTroId.Value);
                    if (nguoiDungvm != null)
                    {
                        result.NguoiDungs.Add(nguoiDungvm);
                    }
                }
                result.Times = new List<TimeViewModel>();
                for (int i = 0; i < 34; i++)
                {
                    if (i == 0)
                    {
                        result.Times.Add(new TimeViewModel() { Value = i, Summary = "0h00min" });
                    }
                    else if (i == 1)
                    {
                        result.Times.Add(new TimeViewModel() { Value = i, Summary = "0h05min" });
                    }
                    else
                    {
                        result.Times.Add(new TimeViewModel() { Value = i, Summary = GetStrTime((i - 1) * 900) });
                    }
                }
            }

            return result;
        }

        public CongViecViewModel GetTaskDetailById2(int taskId)
        {
            var result = _manager.GetById2(taskId);
            if (result != null)
            {   // lay danh sach ngay da log time cua task
                var lstDays = _thoiGianLamViecP.GetDatesStartTimeOfTaskId(taskId);
                List<ToDoInDayViewModel> lst = new List<ToDoInDayViewModel>();
                List<int> lstTodos = new List<int>();
                var timeDone = 0;
                var timeTotal = 0;
                foreach (var item in lstDays)
                {
                    ToDoInDayViewModel vm = new ToDoInDayViewModel();
                    vm.Day = item;
                    // Lay danh sach congviecId da chay time trong ngay
                    var lstIds = _manager.GetIdsBy2(taskId, item);
                    vm.CongViecs = _manager.GetsBy2(taskId, item);

                    foreach (var item2 in vm.CongViecs)
                    {
                        item2.LoaiCongViecs = _loaiCongViecP.GetsByCongViecId(item2.CongViecId);
                        if (item2.LoaiCongViecs != null && item2.LoaiCongViecs.Count > 0) item2.TenLoaiCongViec = item2.LoaiCongViecs[0].TenLoaiCongViec;
                        item2.StrThoiGianLamViec = GetStrTime(item2.TongThoiGian);
                        timeTotal += item2.TongThoiGian;
                        if (item2.PheDuyet != null && item2.PheDuyet.Value) timeDone += item2.TongThoiGian;
                        if (lstTodos.IndexOf(item2.CongViecId) == -1) lstTodos.Add(item2.CongViecId);
                    }
                    lst.Add(vm);
                }
                if (result.ThoiGianUocLuong == null) result.ThoiGianUocLuong = 0;
                result.StrThoiGianLamViec = GetStrTime(timeTotal) + "(" + result.ThoiGianUocLuong + "h)";
                result.TongThoiGian = timeTotal;
                result.StrTotalTime = GetStrTime(timeTotal);
                result.StrTimeDone = GetStrTime(timeDone);
                // Lay danh sach todoId con cua task
                var ids = _manager.GetCongViecIdsByKhoaCha(taskId);
                foreach (var item in ids)
                {
                    if (lstTodos.IndexOf(item) == -1)
                    {
                        lstTodos.Add(item);
                        var cv = _manager.GetById4(item);
                        cv.LoaiCongViecs = _loaiCongViecP.GetsByCongViecId(item);
                        if (cv.LoaiCongViecs != null && cv.LoaiCongViecs.Count > 0) cv.TenLoaiCongViec = cv.LoaiCongViecs[0].TenLoaiCongViec;
                        if (lst.Count == 0 || String.Format("{0:dd/MM/yyyy}", lst[0].Day) != String.Format("{0:dd/MM/yyyy}", DateTime.Now))
                        {
                            lst.Insert(0, new ToDoInDayViewModel() { Day = DateTime.Now, CongViecs = new List<CongViecViewModel>() { cv } });
                        }
                        else
                        {
                            lst[0].CongViecs.Add(cv);
                        }
                    }
                }
                result.ToDoInDays = lst;
                result.TrangThaiCongViecs = _trangThaiCCM.GetByKhoaChaId((int)EnumTrangThaiCongViecType.congviec);
                var loaiCongViecs = _loaiCongViecP.GetsByCongViecId(result.CongViecId);
                if (loaiCongViecs != null && loaiCongViecs.Count > 0)
                {
                    foreach (var item in loaiCongViecs)
                    {
                        item.LoaiCongViecs = _loaiCongViecP.GetsByKhoaChaId(item.LoaiCongViecId);
                    }
                    result.LoaiCongViecs = loaiCongViecs;
                }
                result.NguoiDungs = new List<NguoiDungViewModel>();
                if (result.NguoiXuLyId != null)
                {
                    var nguoiDungvm = _nguoiDungM.GetById(result.NguoiXuLyId.Value);
                    if (nguoiDungvm != null)
                    {
                        result.NguoiDungs.Add(nguoiDungvm);
                    }
                }
                if (result.NguoiHoTroId != null)
                {
                    var nguoiDungvm = _nguoiDungM.GetById(result.NguoiHoTroId.Value);
                    if (nguoiDungvm != null)
                    {
                        result.NguoiDungs.Add(nguoiDungvm);
                    }
                }
                result.Times = new List<TimeViewModel>();
                for (int i = 0; i < 34; i++)
                {
                    if (i == 0)
                    {
                        result.Times.Add(new TimeViewModel() { Value = i, Summary = "0h00min" });
                    }
                    else if (i == 1)
                    {
                        result.Times.Add(new TimeViewModel() { Value = i, Summary = "0h05min" });
                    }
                    else
                    {
                        result.Times.Add(new TimeViewModel() { Value = i, Summary = GetStrTime((i - 1) * 900) });
                    }
                }
            }

            return result;
        }
        public List<LoaiCongViecViewModel> GetLoaiCongViecsByTaskId(int taskId)
        {
            var lstToreturn = _loaiCongViecP.GetsByCongViecId(taskId);
            if (lstToreturn != null && lstToreturn.Count > 0)
            {
                foreach (var item in lstToreturn)
                {
                    item.LoaiCongViecs = _loaiCongViecP.GetsByKhoaChaId(item.LoaiCongViecId);
                }
            }
            return lstToreturn;
        }
        public CongViecViewModel GetToDoDetailById(int toDoId)
        {
            var vm = _manager.GetById(toDoId);
            if (vm != null)
            {
                vm.LoaiCongViecIds = _loaiCongViecP.GetIdsByCongViecId(toDoId);
            }
            return vm;
        }

        public CongViecViewModel GetTimeEntryDetailById(int timeEntryId)
        {
            var vm = _manager.GetById(timeEntryId);
            if (vm != null)
            {
                vm.LoaiCongViecIds = _loaiCongViecP.GetIdsByCongViecId(timeEntryId);
                var thoiGianLamViecs = _thoiGianLamViecP.GetByCongViecId(timeEntryId);
                if (thoiGianLamViecs != null && thoiGianLamViecs.Count > 0)
                {
                    vm.StrNgayLamViec = String.Format("{0:dd/MM/yyyy }", thoiGianLamViecs[0].NgayLamViec);
                    var timespent = thoiGianLamViecs[0].TongThoiGian;
                    if (timespent > 0)
                    {
                        if (timespent == 0)
                        {
                            vm.TimeValue = 0;
                        }
                        if (timespent == 300)
                        {
                            vm.TimeValue = 1;
                        }
                        else
                        {
                            vm.TimeValue = (timespent / 900) + 1;
                        }
                    }
                }

            }
            return vm;
        }
        #endregion

        public int InsertToDoByKhoaCha(int taskId, Guid userId)
        {
            var congViec = _manager.GetById(taskId);
            CongViecViewModel newvm = new CongViecViewModel();
            var tuan = Helpers.GetNumerWeek(DateTime.Now);
            var monday = Helpers.GetMonDayBy(DateTime.Now);
            newvm.TenCongViec = congViec.TenCongViec;
            newvm.NguoiXuLyId = userId;
            newvm.NguoiTaoId = userId;
            newvm.Tuan = tuan;
            newvm.Nam = monday.Year;
            newvm.DuAnId = congViec.DuAnId;
            newvm.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.todoDo;
            newvm.LaShipAble = false;
            newvm.LaTask = false;
            newvm.LoaiTimer = 1;
            newvm.KhoaChaId = taskId;
            return _manager.Insert(newvm);
        }

        public bool CheckIsExitTenCongViec(string tenCongViec,int duAnId)
        {
            return _manager.CheckIsExist(tenCongViec, duAnId);
        }
        public CongViecViewModel GetShipByTen(string tenCongViec,int duAnId)
        {
            return _manager.GetShipAbleByTen2(tenCongViec, duAnId);
        }

        public int InsertShipable(CongViecViewModel vm, Guid userId)
        {
            vm.LaShipAble = true;
            vm.NguoiTaoId = userId;
            var check = false;
            if (!string.IsNullOrEmpty(vm.StrNgayDuKienHoanThanh))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrNgayDuKienHoanThanh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.NgayDuKienHoanThanh = dt;
                    check = true;
                }

            }
            if (!check)
            {
                var monday = Helpers.GetFirstMondayOfWeek(vm.Nam, vm.Tuan);
                vm.NgayDuKienHoanThanh = monday.AddDays(4);
            }
            vm.TuanHoanThanh = vm.Tuan;
            CauHinhProvider cauHinhM = new CauHinhProvider();
            vm.MaCongViec = cauHinhM.GetMaShipAble().ToString();
            vm.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.shipablePlan;
            var id = _manager.Insert(vm);

            if (id > 0)
            {
                cauHinhM.InsertMaShipable();
            }
            return id;
        }



        public bool UpdateQuanTam(int congViecId, Guid userId)
        {
            return _manager.InsertOrUpdateQuanTam(congViecId, userId);
        }

        public bool UpdateShipable(CongViecViewModel vm, Guid userId)
        {
            vm.LaShipAble = true;
            vm.NguoiTaoId = userId;
            var shipableO = _manager.GetById(vm.CongViecId);
            if (shipableO == null) return false;
            if (shipableO.TuanHoanThanh != null) vm.TuanHoanThanh = shipableO.TuanHoanThanh;
            var checkNGay = false;
            if (!string.IsNullOrEmpty(vm.StrNgayDuKienHoanThanh))
            {
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParseExact(vm.StrNgayDuKienHoanThanh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    vm.NgayDuKienHoanThanh = dt;
                    checkNGay = true;
                }
            }
            if (!checkNGay)
            {
                var monday = Helpers.GetFirstMondayOfWeek(vm.Nam, vm.Tuan);
                vm.NgayDuKienHoanThanh = monday.AddDays(4);
            }
            var tuanHienTai = Helpers.GetNumerWeek(DateTime.Now);
            var mondayN = Helpers.GetMonDayBy(DateTime.Now);
            if (vm.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDebit && shipableO.TrangThaiCongViecId != (int)EnumTrangThaiCongViecType.shipableDebit)
            {
                var lst = _manager.GetsByKhoaChaId(vm.CongViecId);
                if (lst != null && lst.Count > 0 && vm.XuLyVaoTuanTiepTheo != null && !vm.XuLyVaoTuanTiepTheo.Value)
                {
                    if (!_manager.DeletesByKhoaChaId(vm.CongViecId)) return false;
                }
                if ((lst == null || lst.Count == 0) && vm.XuLyVaoTuanTiepTheo != null && vm.XuLyVaoTuanTiepTheo.Value)
                {
                    var khoaCha = vm.KhoaChaId;
                    var tuan = vm.Tuan;
                    var nam = vm.Nam;
                    var maCongViec = vm.MaCongViec;
                    vm.KhoaChaId = vm.CongViecId;
                    if (khoaCha > 0)
                    {
                        var vmCha = _manager.GetById(khoaCha.Value);
                        if (vmCha != null) vm.TenCongViec = vm.TenCongViec.Replace("(Debit #" + vmCha.MaCongViec + ")", "");
                    }
                    vm.TenCongViec = "(Debit #" + maCongViec + ")" + vm.TenCongViec;
                    var monday = Helpers.GetFirstMondayOfWeek(vm.Tuan, vm.Nam);
                    var maxTuan = Helpers.GetNumberWeekOfYears(monday.Year);
                    if (vm.Tuan == maxTuan)
                    {
                        vm.Tuan = 1;
                        vm.Nam++;
                    }
                    else
                        vm.Tuan++;
                    if (vm.Tuan < tuanHienTai && vm.Nam >= mondayN.Year) vm.Tuan = tuanHienTai;
                    if (InsertShipable(vm, userId) == 0) return false;
                    vm.Tuan = tuan;
                    vm.Nam = nam;
                    vm.KhoaChaId = khoaCha;
                    vm.MaCongViec = maCongViec;
                    vm.TenCongViec = vm.TenCongViec.Replace("(Debit #" + maCongViec + ")", "");
                    vm.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.shipableDebit;
                }
            }
            if (vm.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableContinue && (shipableO.TrangThaiCongViecId != (int)EnumTrangThaiCongViecType.shipableContinue || vm.XuLyVaoTuanTiepTheo == true))
            {

                if (vm.TuanHoanThanh == null)
                {
                    var monday = Helpers.GetFirstMondayOfWeek(vm.Tuan, vm.Nam);
                    var maxTuan = Helpers.GetNumberWeekOfYears(monday.Year);
                    if (vm.Tuan == maxTuan)
                    {
                        vm.TuanHoanThanh = 1;
                        vm.Nam++;
                    }
                    else
                        vm.TuanHoanThanh = 1 + vm.Tuan;

                }
                else
                {
                    var monday = Helpers.GetFirstMondayOfWeek(vm.Tuan, vm.Nam);
                    var maxTuan = Helpers.GetNumberWeekOfYears(monday.Year);
                    if (vm.TuanHoanThanh == maxTuan)
                    {
                        vm.TuanHoanThanh = 1;
                        vm.Nam++;
                    }
                    else
                        vm.TuanHoanThanh++;

                }

            }
            var trangThaiId = shipableO.TrangThaiCongViecId;

            var check = _manager.Update(vm);
            if (check)
            {
                DiemPointProvider diemP = new DiemPointProvider();
                if (trangThaiId == (int)EnumTrangThaiCongViecType.shipableDone)
                {
                    if (shipableO.KhoaChaId == null)
                        diemP.InsertDiemPointByShipableId(vm.CongViecId, true);
                }
                if (trangThaiId == (int)EnumTrangThaiCongViecType.shipableDebit)
                {
                    diemP.InsertDiemPointByShipableId(vm.CongViecId, false);
                }
                return check;
            }
            return check;
        }

        public bool InsertShipableDebit(int shipableId, Guid userId)
        {
            var vm = _manager.GetById(shipableId);
            var tuanHienTai = Helpers.GetNumerWeek(DateTime.Now);
            if (vm.KhoaChaId > 0)
            {
                var vmCha = _manager.GetById(vm.KhoaChaId.Value);
                if (vmCha != null) vm.TenCongViec = vm.TenCongViec.Replace("(Debit #" + vmCha.MaCongViec + ")", "");
            }
            vm.TenCongViec = "(Debit #" + vm.MaCongViec + ")" + vm.TenCongViec;
            vm.Tuan++;
            if (vm.Tuan < tuanHienTai) vm.Tuan = tuanHienTai + 1;
            vm.KhoaChaId = shipableId;
            if (InsertShipable(vm, userId) == 0) return false;
            return true;
        }

        public bool UpdateTrangThai(int congViecId, int trangThaiId)
        {

            return _manager.UpdateTrangThai(congViecId, trangThaiId);
        }
        public bool UpdateTrangThaiShipable(int shipableId, int trangThaiId, Guid userId)
        {
            var user = _nguoiDungM.GetById(userId);
            if (user.Quyen == 3)
            {
                var check = true;
                var vm = _manager.GetById(shipableId);

                if (trangThaiId == (int)EnumTrangThaiCongViecType.shipableContinue)
                {
                    int tuan = 0;
                    if (vm.TuanHoanThanh != null) tuan = vm.TuanHoanThanh.Value + 1;
                    else tuan = vm.Tuan + 1;
                    check = _manager.UpdateTrangThai2(shipableId, (int)EnumTrangThaiCongViecType.shipableDoing, tuan);
                }
                else
                {
                    check = _manager.UpdateTrangThai(shipableId, trangThaiId);
                }
                if (check)
                {
                    DiemPointProvider diemP = new DiemPointProvider();
                    if (trangThaiId == (int)EnumTrangThaiCongViecType.shipableDone)
                    {
                        if (vm.KhoaChaId == null)
                            diemP.InsertDiemPointByShipableId(shipableId, true);
                    }
                    if (trangThaiId == (int)EnumTrangThaiCongViecType.shipableDebit)
                    {
                        diemP.InsertDiemPointByShipableId(shipableId, false);
                    }
                    return check;
                }
                else
                    return check;
            }
            else
            {
                return false;
            }


            //if (vm.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableContinue)
            //{
            //    vm.TuanHoanThanh = vm.Tuan++;
            //}
            //var trangThaiId = congViecO.TrangThaiCongViecId;

            //var check = _manager.Update(vm);
            //if (check)
            //{
            //    var congViecN = _manager.GetById(vm.CongViecId);
            //    if (congViecN.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.shipableDone && congViecN.TrangThaiCongViecId != trangThaiId)
            //    {
            //        _manager.DoneAllCongViecInShipableBy(vm.CongViecId, (int)EnumTrangThaiCongViecType.congViecDone);
            //    }
            //}

        }
        public int InserCongViec(CongViecViewModel model)
        {
            CongViecProvider manager = new CongViecProvider();
            if (model.NguoiHoTroId == Guid.Empty) model.NguoiHoTroId = null;
            var id = manager.Insert(model);
            if (id > 0 && model.LoaiCongViecIds != null && model.LoaiCongViecIds.Count > 0)
            {
                manager.InsertUpdateLKLoaiCongViec(id, model.LoaiCongViecIds);
            }
            return id;
        }

        public AddResultViewModel InsertToDoV2(CongViecViewModel vm, TokenViewModel token)
        {
            AddResultViewModel result = new AddResultViewModel();
            ThoiGianLamViecModel ttlvM = new ThoiGianLamViecModel();

            vm.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.todoNew;
            vm.LaShipAble = false;
            vm.LaTask = false;
            vm.Tuan = Helpers.GetNumberWeekOfDay(vm.NgayLamViec);
            vm.Nam = Helpers.GetMonDayBy(vm.NgayLamViec).Year;
            vm.IsToDoAdd = true;
            vm.LoaiTimer = 1;
            var admin = _nguoiDungM.GetUserByUsername("admin");

            //Lấy shipable Unknowledge
            var shipId = _manager.GetShipableIdByName("unknowledged", vm.DuAnId);
            if (shipId == 0) shipId = _manager.Insert(new CongViecViewModel { TenCongViec = "unknowledged", DuAnId = vm.DuAnId, Tuan = vm.Tuan, Nam = vm.Nam, LaShipAble = true, LoaiTimer = 1, NguoiTaoId = admin.NguoiDungId, NguoiXuLyId = admin.NguoiDungId, TrangThaiCongViecId = 2, ThuTuUuTien = 2 });
            // Lấy task Unknowledge
            var taskId = _manager.GetTaskIdByName("unknowledged", vm.DuAnId);
            if (taskId == 0) taskId = _manager.Insert(new CongViecViewModel { TenCongViec = "unknowledged", DuAnId = vm.DuAnId, Tuan = vm.Tuan, Nam = vm.Nam, LaShipAble = false, LaTask = true, LoaiTimer = 1, KhoaChaId = shipId, NguoiTaoId = admin.NguoiDungId, NguoiXuLyId = admin.NguoiDungId, TrangThaiCongViecId = 9 });
            vm.KhoaChaId = taskId;
            var congViecid = _manager.Insert(vm);
            if (congViecid == 0)
            {
                result.Message = "Thêm mới công việc không thành công.";
            }
            else
            {
                result.Status = true;
                //if (vm.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.todoDo)
                //{
                //    ttlvM.StartTimer(congViecid, vm.NguoiTaoId, token.key_token);
                //}
                result.Message = "Thêm mới công việc thành công.";
            }
            return result;
        }
        public int InsertOrUpdateToDo(CongViecViewModel vm)
        {
            int id = 0;
            if (vm.TrangThaiCongViecId == 0)
                vm.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.todoNew;
            if (vm.CongViecId == 0)
            {
                vm.LaShipAble = false;
                vm.LaTask = false;
                vm.LoaiTimer = 1;
                id = _manager.Insert(vm);

            }
            else
            {
                if (_manager.Update(vm)) id = vm.CongViecId;
            }
            if (id > 0)
            {
                _loaiCongViecP.InsertLienKetLoaiCongViecs(id, vm.LoaiCongViecIds);
            }
            return id;
        }
        public bool UpdateToDo2Task(int congViecId, int khoaChaId)
        {
            return _manager.updateKhoaCha(congViecId, khoaChaId);

        }
        public int InsertOrUpdateTimeEntry(CongViecViewModel vm, string tokenId, Guid userId, DateTime ngayLamViec)
        {
            try
            {
                var nguoiDung = _nguoiDungM.GetById(userId);
                var timeFrom = getDateTime(ngayLamViec, nguoiDung.KhungThoiGianBatDau);
                var timeTo = getDateTime(ngayLamViec, nguoiDung.KhungThoiGianKetThuc);
                var timeNghiTruaF = getDateTime(ngayLamViec, "12:00");
                var timeNghiTruaT = getDateTime(ngayLamViec, "13:00");
                var startTime = getDateTime(ngayLamViec, vm.StrThoiGianBatDau);
                var endTime = getDateTime(ngayLamViec, vm.StrThoiGianKetThuc);
                if (startTime < timeFrom || startTime > timeTo || endTime < timeFrom || endTime > timeTo || (startTime > timeNghiTruaF && startTime < timeNghiTruaT)) return 0;
                var date = DateTime.ParseExact(vm.StrNgayLamViec, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                int id = 0;
                if (vm.TrangThaiCongViecId == 0)
                    vm.TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.todoDone;
                if (vm.CongViecId == 0)
                {
                    vm.LaShipAble = false;
                    vm.LaTask = false;
                    vm.LoaiTimer = 2;
                    id = _manager.Insert(vm);

                }
                else
                {
                    if (_manager.Update(vm)) id = vm.CongViecId;
                }
                if (id > 0)
                {
                    _loaiCongViecP.InsertLienKetLoaiCongViecs(id, vm.LoaiCongViecIds);
                    if (endTime <= timeNghiTruaF || startTime >= timeNghiTruaT)
                    {
                        _thoiGianLamViecP.Insert(id, userId, startTime, endTime, 1, (int)(endTime - startTime).TotalSeconds, true, "", null, tokenId, ngayLamViec);
                    }
                    else
                    {
                        if (startTime <= timeNghiTruaF)
                        {
                            if (endTime >= timeNghiTruaT)
                            {
                                _thoiGianLamViecP.Insert(id, userId, startTime, timeNghiTruaF, 1, (int)(timeNghiTruaF - startTime).TotalSeconds, true, "", null, tokenId, ngayLamViec);
                                _thoiGianLamViecP.Insert(id, userId, timeNghiTruaT, endTime, 1, (int)(endTime - timeNghiTruaT).TotalSeconds, true, "", null, tokenId, ngayLamViec);
                            }
                            else
                            {
                                _thoiGianLamViecP.Insert(id, userId, startTime, timeNghiTruaF, 1, (int)(timeNghiTruaF - startTime).TotalSeconds, true, "", null, tokenId, ngayLamViec);
                            }

                        }
                        else
                        {
                            if (startTime >= timeNghiTruaF && startTime <= timeNghiTruaT)
                            {
                                _thoiGianLamViecP.Insert(id, userId, timeNghiTruaT, endTime, 1, (int)(endTime - timeNghiTruaT).TotalSeconds, true, "", null, tokenId, ngayLamViec);
                            }
                        }

                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public bool PheDuyetToDoOrTimeEntry(string id, bool check)
        {
            var col = id.Split('-');
            var loaiThoiGian = byte.Parse(col[1].ToString());
            var congViecId = int.Parse(col[0].ToString());
            var ngayLamViec = DateTime.ParseExact(col[2].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var result = _thoiGianLamViecP.UpdatePheDuyetBy(congViecId, ngayLamViec, loaiThoiGian, check);
            if (result && loaiThoiGian == 3)
            {
                _manager.XacNhanHoanThanhToDo(congViecId, check);
            }
            return result;

        }
        public bool DeleteShipable(int congViecId)
        {
            return _manager.Delete(congViecId);
        }
        public ResultViewModel UpdateTodo(CongViecViewModel model, Guid nguoiDungId)
        {
            ResultViewModel result = new ResultViewModel();
            var congViec = _manager.GetById(model.CongViecId);
            var nguoiDung = _nguoiDungM.GetById(nguoiDungId);
            if (congViec.NgayDuKienHoanThanh < DateTime.Now && congViec.NgayDuKienHoanThanh.Value.ToString("dd/MM/yyyy") != DateTime.Now.ToString("dd/MM/yyyy") && nguoiDung.Quyen != 3)
            {
                result.Status = false;
                result.Message = "Không thể cập nhật Công việc đã quá thời gian";
            }
            else
            {
                if (model.NguoiHoTroId == Guid.Empty) model.NguoiHoTroId = null;
                if (nguoiDungId != congViec.NguoiXuLyId && nguoiDung.Quyen != 3)
                {
                    result.Status = false;
                    result.Message = "Bạn không có quyền cập nhật công việc này.";
                }
                else
                {
                    if (_manager.Update(model))
                    {
                        result.Status = true;
                        result.Message = "Cập nhật Công việc thành công.";
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Cập nhật Công việc không thành công";
                    }
                }

            }
            return result;


        }
        public ResultViewModel UpdateTodo2(CongViecViewModel model, Guid nguoiDungId)
        {
            ResultViewModel result = new ResultViewModel();
            var congViec = _manager.GetById(model.CongViecId);
            var nguoiDung = _nguoiDungM.GetById(nguoiDungId);            
            
                if (model.NguoiHoTroId == Guid.Empty) model.NguoiHoTroId = null;
                if (nguoiDungId != congViec.NguoiXuLyId && nguoiDung.Quyen != 3)
                {
                    result.Status = false;
                    result.Message = "Bạn không có quyền cập nhật công việc này.";
                }
                else
                {
                    if (_manager.Update(model))
                    {
                        result.Status = true;
                        result.Message = "Cập nhật Công việc thành công.";
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Cập nhật Công việc không thành công";
                    }
                }

           
            return result;


        }

        public bool UpdateStatus(int CongViecId, int TrangThaiId)
        {
            return _manager.UpdateTrangThai(CongViecId, TrangThaiId);
        }
        public bool UpdateStatusV2(int CongViecId, int trangThaiId, string tokenKey, string linkGetToken)
        {
            var congViec = _manager.GetById(CongViecId);

            if (trangThaiId == (int)EnumTrangThaiCongViecType.todoDo)
            {
                if (congViec.TrangThaiCongViecId != trangThaiId)
                    _thoiGianLamViecP.Insert(congViec.CongViecId, congViec.NguoiTaoId, DateTime.Now, null, 1, 0, null, null, null, tokenKey, DateTime.Now);
            }
            else if (congViec.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.todoDo && trangThaiId != congViec.TrangThaiCongViecId)
            {
                _thoiGianLamViecP.UpdateStopByV2(CongViecId, congViec.NguoiTaoId, linkGetToken);
            }
            return _manager.UpdateTrangThai(CongViecId, trangThaiId);
        }
        public bool DeleteCongViec(int CongViecId)
        {
            return _manager.Delete(CongViecId);
        }

        public bool DeleteTimeEntry(int timeEntryId, Guid userId)
        {

            var congViecVm = _manager.GetById(timeEntryId);
            var nguoiDungVm = _nguoiDungM.GetById(userId);
            if (congViecVm.NguoiXuLyId == userId || nguoiDungVm.Quyen == 3)
            {
                return _manager.DeleteTimeEntry(timeEntryId);
            }
            return false;
        }
        public bool DeleteToDo(string id, Guid userId)
        {
            var col = id.Split('-');
            var toDoId = int.Parse(col[0].ToString());
            var ThoiGianLamViecId = int.Parse(col[1].ToString());
            var congViecVm = _manager.GetById(toDoId);
            var nguoiDungVm = _nguoiDungM.GetById(userId);
            if (congViecVm.NguoiXuLyId == userId || nguoiDungVm.Quyen == 3)
            {
                return _manager.DeleteToDo(toDoId, ThoiGianLamViecId);
            }
            return false;
        }
        public bool DeleteToDoV2(int toDoId, Guid userId)
        {
            var congViecVm = _manager.GetById(toDoId);
            var nguoiDungVm = _nguoiDungM.GetById(userId);
            if (congViecVm.NguoiXuLyId == userId || nguoiDungVm.Quyen == 3)
            {
                return _manager.DeleteToDoV2(toDoId);
            }
            return false;
        }

        public bool DeleteTask(int taskId, Guid userId)
        {
            try
            {
                var congViecVm = _manager.GetById(taskId);
                var nguoiDungVm = _nguoiDungM.GetById(userId);
                if (congViecVm.NguoiXuLyId == userId || nguoiDungVm.Quyen == 3)
                {
                    var toDoIds = _manager.GetCongViecIdsByKhoaCha(taskId);
                    foreach (var id in toDoIds)
                    {
                        _manager.DeleteToDoV2(id);
                    }
                    return _manager.Delete(taskId);

                }
                return true;
            }
            catch { return false; }
            
        }

        public string GetAllTimeOfTask(int taskId)
        {

            var timeDone = _thoiGianLamViecP.GetAllTimeSpentByKhoaCha(taskId, true);
            var totalTime = _thoiGianLamViecP.GetAllTimeSpentByKhoaCha(taskId, false);
            return GetStrTime(timeDone) + "/" + GetStrTime(totalTime);
        }
        public ResultViewModel XacNhanHoanThanhToDo(int toDoId, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            var quyen = _nguoiDungM.GetQuyenNguoiDungById(userId);
            if (quyen == 3)
            {
                if (_manager.XacNhanHoanThanhToDo(toDoId, true))
                {
                    result.Status = true;
                    result.Message = "Bạn đã done todo thành công.";
                }
                else
                {
                    result.Status = false;
                    result.Message = "Bạn đã done todo không thành công.";
                }
            }
            else
            {
                result.Status = false;
                result.Message = "Bạn không có quyền done todo này.";
            }
            return result;
        }
        public ResultViewModel XacNhanHoanThanhTask(int taskId, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            var quyen = _nguoiDungM.GetQuyenNguoiDungById(userId);
            if (quyen == 3)
            {
                if (_manager.XacNhanHoanThanhTask(taskId, true))
                {
                    result.Status = true;
                    result.Message = "Bạn đã done task thành công.";
                    var lst = _manager.GetCongViecIdsByKhoaCha(taskId);
                    if (lst != null && lst.Count > 0) result.ItemId = JsonConvert.SerializeObject(lst);
                }
                else
                {
                    result.Status = false;
                    result.Message = "Bạn đã done task không thành công.";
                }
            }
            else
            {
                result.Status = false;
                result.Message = "Bạn không có quyền done task này.";
            }
            return result;
        }
        /// <summary>
        /// get chuỗi string định dạng 4h30min
        /// </summary>
        /// <param name="timeSpent"></param>
        /// <returns></returns>
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
                    result += "0" + minutes + "min";
                else result += minutes + "min";
            }
            else
            {
                result = "0h00min";
            }
            return result;
        }

        private string GetStrTime2(int timeSpent)
        {
            var result = "";
            if (timeSpent > 0)
            {
                var hour = timeSpent / 3600;
                var minutes = (timeSpent % 3600) / 60;
                var second = (timeSpent % 3600) % 60;
                if (hour < 10) result += "0" + hour + ":";
                else result += hour + ":";
                if (minutes < 10)
                    result += "0" + minutes + ":";
                else result += minutes + ":";
                if (second < 10)
                    result += "0" + second;
                else result += second;
            }
            else
            {
                result = "00:00:00";
            }
            return result;
        }
        private DateTime getDateTime(DateTime date, string time)
        {
            //string timeString = "11/12/2009 13:30:00.000";
            IFormatProvider culture = new CultureInfo("en-US", true);
            //DateTime dateVal = DateTime.ParseExact(timeString, "dd/MM/yyyy HH:mm:ss.fff", culture);


            var str = "";
            var day = date.Day;
            if (day < 10) str += "0" + day;
            else str += day;
            var month = date.Month;
            if (month < 10) str += "/0" + month;
            else str += "/" + month;
            str += "/" + date.Year + " " + time;
            str += ":00.000";
            return DateTime.ParseExact(str, "dd/MM/yyyy HH:mm:ss.fff", culture);
        }

        public List<int> GetToDoIdsBy(int taskId, Guid nguoiXuLyId)
        {
            return _manager.GetToDoIdsBy(taskId,nguoiXuLyId);
        }
    }
}