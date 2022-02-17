using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Security;
using MetaWork.Data;
using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using static MetaWork.WorkTime.Controllers.TimeController;

namespace MetaWork.WorkTime.Models
{
    public class ThoiGianLamViecModel
    {
        ThoiGianLamViecProvider thoiGianLamViecProvider = new ThoiGianLamViecProvider();

        NguoiDungProvider _nguoiDungP = new NguoiDungProvider();

        public CongViecViewModel GetCurrentTodoCountTime(Guid userId)
        {
            return thoiGianLamViecProvider.GetToDoStartTimeOfUser(userId);
        }

        public int GetTimeLogedOfTodo(int todoId)
        {
            return thoiGianLamViecProvider.GetAllTimeSpentOfToDo(todoId, true);
        }

        public ResultViewModel ApproveTime(int toDoId)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecModel model = new CongViecModel();
            var congviec = model.GetById(toDoId);
            if (congviec.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.todoDo)
            {
                result.Message = "Bạn hãy dừng công việc đang chạy để tiến hành approve";
            }
            else
            {
                result.Status = thoiGianLamViecProvider.PheDuyet(toDoId);
                if (result.Status)
                {
                    result.Message = "Bạn đã approve thời gian làm việc thành công.";
                }
                else result.Message = "Bạn approve thời gian làm việc không thành công.";
            }
            return result;
        }

        public ResultViewModel StopTodo(int congViecId, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecModel model = new CongViecModel();
            var thoiGian = thoiGianLamViecProvider.GetThoiGianIsStartOfUser(userId);
            if (thoiGian != null)
            {
                var startTime = thoiGian.ThoiGianBatDau;
                var endtime = DateTime.Now;
                if (String.Format("{0:dd/MM/yyyy}", startTime) != String.Format("{0:dd/MM/yyyy}", endtime)) endtime = getDateTime(startTime.Value, "23:59");
                
                    result.Status = thoiGianLamViecProvider.UpdateStopBy2(congViecId, userId);
                

            }
            else
            {
                result.Message = "không có khoảng time";
            }


            return result;

        }

        public int GetTimeSpentOfDuAn(int duAnId)
        {
            return thoiGianLamViecProvider.GetTimeSpentOfDuAn(duAnId);
        }

        public int StartTimer(int congViecId, Guid userId, string tokenId)
        {
            CongViecProvider model = new CongViecProvider();
            var current_todo = GetCurrentTodoCountTime(userId);
            if (current_todo != null)
            {
                if (StopTodo(current_todo.CongViecId, userId).Status)
                {
                    model.UpdateTrangThai(current_todo.CongViecId, (int)EnumTrangThaiCongViecType.todoBlock);
                    var ThoiGianBatDau = DateTime.Now;
                    return thoiGianLamViecProvider.Insert(congViecId, userId, ThoiGianBatDau, null, 1, 0, null, null, null, tokenId, ThoiGianBatDau);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var ThoiGianBatDau = DateTime.Now;
                return thoiGianLamViecProvider.Insert(congViecId, userId, ThoiGianBatDau, null, 1, 0, null, null, null, tokenId, ThoiGianBatDau);
            }

        }

        public List<ThoiGianLamViecViewModel> GetThoiGianLamViecOfUser(Guid userId, DateTime ngayLamViec)
        {
            return thoiGianLamViecProvider.GetDanhSachLamViecOfUserBy(userId, ngayLamViec);
        }
        public List<ThoiGianLamViecViewModel> GetThoiGianLamViecOfUser2(Guid userId, DateTime ngayLamViec)
        {
            return thoiGianLamViecProvider.GetDanhSachLamViecOfUserBy2(userId, ngayLamViec);
        }

        public List<NguoiDungViewModel> GetTimeOfUserBys(List<string> emails, int fromTime, int toTime)
        {
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            lstToReturn = _nguoiDungP.GetNguoiDungByEmails(emails);
            if (toTime < fromTime) return lstToReturn;
            var startTime = epoch2datetime(fromTime);
            var endTime = epoch2datetime(toTime);
            var lstDate = getAllDateBeetween(startTime, endTime);
            if (lstToReturn != null && lstToReturn.Count > 0)
            {
                PhongBanProvider pbP = new PhongBanProvider();
                foreach (var user in lstToReturn)
                {
                    user.NgayLamViecs = new List<NgayLamViecViewModel>();

                    var phongBanIds = pbP.GetIdsByNguoiDungId(user.NguoiDungId);
                    if (phongBanIds != null && phongBanIds.Count > 0 && phongBanIds.Contains((int)EnumPhongBanId.Sale))
                    {                       
                        var end =(int)(new DateTime(endTime.Year, endTime.Month, endTime.Day, 0, 0, 0).AddDays(1).AddSeconds(-1)-new DateTime(1970,1,1,0,0,0)).TotalSeconds;
                        var lst = GetDanhSachActivity(user.Email, fromTime, end);
                        var LichLamViecCaNhan = getLichLamViecCaNhan(user.Email, fromTime, toTime);
                        if (lstDate != null && lstDate.Count > 0)
                        {
                            foreach (var day in lstDate)
                            {
                                try
                                {
                                    var endTime1 = day.AddDays(1);
                                    var ngayCaNhan = LichLamViecCaNhan.Where(t => t.Day >= day && t.Day < endTime1).FirstOrDefault();
                                    NgayLamViecViewModel ngayLamViec = new NgayLamViecViewModel();
                                    ngayLamViec.NgayLamViec = day;
                                    ngayLamViec.TongThoiGianInTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(user.NguoiDungId, day, 1);
                                    ngayLamViec.TongThoiGianOutTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(user.NguoiDungId, day, 2);
                                    ngayLamViec.TongThoiGianOffTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(user.NguoiDungId, day, 3);
                                    ngayLamViec.key_token = thoiGianLamViecProvider.GetTokenKeyBy(user.NguoiDungId, day);
                                    user.NgayLamViecs.Add(ngayLamViec);

                                    if (ngayLamViec.TongThoiGianInTime < ngayCaNhan.DayType.TimeRequireInSeconds)
                                    {
                                        try
                                        {
                                            var lstE = lst.Where(t => t.DNgayBatDau >= ngayCaNhan.Day.Value && t.DNgayBatDau < endTime1).ToList();
                                            int timeall = ngayLamViec.TongThoiGianInTime;
                                            if (timeall < ngayCaNhan.DayType.TimeRequireInSeconds)
                                            {
                                                timeall += lstE.Count * 3600 * 4;
                                                if (timeall > ngayCaNhan.DayType.TimeRequireInSeconds) timeall = (int)ngayCaNhan.DayType.TimeRequireInSeconds;
                                            }
                                            ngayLamViec.TongThoiGianInTime = timeall;
                                        }
                                        catch
                                        {

                                        }
                                      


                                    }
                                    if(ngayLamViec.TongThoiGianInTime>0&& string.IsNullOrEmpty(ngayLamViec.key_token))
                                    {
                                        RegisterToken(user.Email, (int)(day - new DateTime(1970, 1, 1)).TotalSeconds);
                                        var token = GetTokenKey2(day, user.NguoiDungId);
                                        if (token != null && !string.IsNullOrEmpty(token.key_token))
                                            ngayLamViec.key_token = token.key_token;
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    else if (lstDate != null && lstDate.Count > 0)
                    {
                        foreach (var day in lstDate)
                        {
                            NgayLamViecViewModel ngayLamViec = new NgayLamViecViewModel();
                            ngayLamViec.NgayLamViec = day;
                            ngayLamViec.TongThoiGianInTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(user.NguoiDungId, day, 1);
                            ngayLamViec.TongThoiGianOutTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(user.NguoiDungId, day, 2);
                            ngayLamViec.TongThoiGianOffTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(user.NguoiDungId, day, 3);
                            ngayLamViec.key_token = thoiGianLamViecProvider.GetTokenKeyBy(user.NguoiDungId, day);
                            user.NgayLamViecs.Add(ngayLamViec);
                        }
                    }                  
                    

                }
            }
            return lstToReturn;
        }

        public List<NgayLamViecViewModel> GetNgayLamViecsOfUser(Guid userId, DateTime startTime, DateTime endTime)
        {
            List<NgayLamViecViewModel> lstToReturn = new List<NgayLamViecViewModel>();
            var lstDate = getAllDateBeetween(startTime, endTime);

            if (lstDate != null && lstDate.Count > 0)
            {
                foreach (var day in lstDate)
                {
                    NgayLamViecViewModel ngayLamViec = new NgayLamViecViewModel();
                    ngayLamViec.NgayLamViec = day;
                    ngayLamViec.TongThoiGianInTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(userId, day, 1);
                    ngayLamViec.TongThoiGianOutTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(userId, day, 2);
                    ngayLamViec.TongThoiGianOffTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(userId, day, 3);
                    lstToReturn.Add(ngayLamViec);
                }
            }
            return lstToReturn;
        }

        public List<LichLamViecCaNhanViewModel> GetNgayLamViecsOfUser2(Guid userId, DateTime startTime, DateTime endTime, List<LichLamViecCaNhanViewModel> lichLamViecCaNhan, out string strTotal, out string strTotolTimeNeed, out string percel)
        {
            int total = 0;
            int totalTimeNeed = 0;
            var lstDate = getAllDateBeetween(startTime, endTime);
            var danhSachNgay = getDanhSachLoaiNgayDangKy();
            int start = ((int)(startTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            int end = ((int)(endTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            var nguoiDung = _nguoiDungP.GetById(userId);
            PhongBanProvider pbM = new PhongBanProvider();
            var phongbanIds = pbM.GetIdsByNguoiDungId(userId);
            var lst = GetDanhSachActivity(nguoiDung.Email, start, end);
            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    if (!string.IsNullOrEmpty(item.NgayBatDau))
                    {
                        try
                        {
                            item.DNgayBatDau = DateTime.ParseExact(item.NgayBatDau, "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
                        }
                        catch { }

                    }
                }
            }
            var countdb = lst.Count(t => t.DNgayBatDau != null);
            if (lichLamViecCaNhan != null && lichLamViecCaNhan.Count > 0)
            {
                foreach (var ngayLamViec in lichLamViecCaNhan)
                {
                    ngayLamViec.TongThoiGianInTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(userId, ngayLamViec.Day.Value, 1);
                    ngayLamViec.TongThoiGianOutTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(userId, ngayLamViec.Day.Value, 2);
                    ngayLamViec.TongThoiGianOffTime = thoiGianLamViecProvider.GetAllTimeOfUserInDay(userId, ngayLamViec.Day.Value, 3);
                    var timeInRankTime = thoiGianLamViecProvider.GetTimePheDuyetInRankTimeInDay(userId, ngayLamViec.Day.Value);
                    if (phongbanIds.Contains((int)EnumPhongBanId.Sale))
                    {
                        if (timeInRankTime < ngayLamViec.DayType.TimeRequireInSeconds)
                        {
                            var endTimeOfDay = ngayLamViec.Day.Value.AddDays(1).AddSeconds(-1);
                            var count = lst.Count(t => t.DNgayBatDau >= ngayLamViec.Day.Value && t.DNgayBatDau < endTimeOfDay);
                            if (count > 0)
                            {
                                var timeall = ngayLamViec.TongThoiGianInTime + count * 3600 * 4;
                                if (timeall > ngayLamViec.DayType.TimeRequireInSeconds) timeall = (int)ngayLamViec.DayType.TimeRequireInSeconds;
                                ngayLamViec.TongThoiGianInTime = timeall;
                            }

                        }

                    }
                    foreach (var item in danhSachNgay)
                    {
                        if (item.DayTypeId == ngayLamViec.DayType.DayTypeId)
                        {
                            ngayLamViec.TongThoiGianOffTime += getTimeOfDayType((byte)item.DayTypeId);
                        }
                    }
                    ngayLamViec.StrTongThoiGianInWork = GetStrTime2(ngayLamViec.TongThoiGianInTime);
                    ngayLamViec.StrTongThoiGianOff = GetStrTime2(ngayLamViec.TongThoiGianOutTime + ngayLamViec.TongThoiGianOffTime);
                    ngayLamViec.StrTongThoiGian = GetStrTime2(ngayLamViec.TongThoiGianInTime + ngayLamViec.TongThoiGianOutTime + ngayLamViec.TongThoiGianOffTime);
                    ngayLamViec.StrTongThoiGianNeed = GetStrTime2((int)(ngayLamViec.DayType.TimeRequireInSeconds));
                    total += ngayLamViec.TongThoiGianInTime + ngayLamViec.TongThoiGianOutTime + ngayLamViec.TongThoiGianOffTime;
                    totalTimeNeed += (int)(ngayLamViec.DayType.TimeRequireInSeconds);
                    totalTimeNeed += getTimeOfDayType((byte)ngayLamViec.DayType.DayTypeId);
                    var time = thoiGianLamViecProvider.GetAddDayTypeOfUser(userId, ngayLamViec.Day.Value);
                    if (time != null)
                    {
                        foreach (var item in danhSachNgay)
                        {
                            if (item.DayTypeId == time.LoaiNgayLamViec)
                            {
                                ngayLamViec.DayType = item;
                            }
                        }
                    }
                }
            }
            strTotal = GetStrTime2(total);
            strTotolTimeNeed = GetStrTime2(totalTimeNeed);
            if (totalTimeNeed != 0)
                percel = ((total * 100) / totalTimeNeed) + "%";
            else percel = "00%";
            return lichLamViecCaNhan;
        }

        private List<DateTime> getAllDateBeetween(DateTime startTime, DateTime endTime)
        {
            List<DateTime> lstToReturn = new List<DateTime>();
            if (String.Format("{0:dd/MM/yyyy}", startTime) == String.Format("{0:dd/MM/yyyy}", endTime))
            {
                lstToReturn.Add(startTime);
                return lstToReturn;
            }
            else
            {
                lstToReturn.Add(startTime);
                do
                {
                    startTime = startTime.AddDays(1);
                    lstToReturn.Add(startTime);

                } while (String.Format("{0:dd/MM/yyyy}", startTime) != String.Format("{0:dd/MM/yyyy}", endTime));
            }
            return lstToReturn;
        }

        private DateTime epoch2datetime(int epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(epoch);
        }

        public bool DeleteThoiGianLamViec(int thoiGianLamViecId, Guid userId)
        {
            var user = _nguoiDungP.GetById(userId);
            var thoiGian = thoiGianLamViecProvider.GetById(thoiGianLamViecId);
            if (thoiGian == null) return false;
            if (user.Quyen == 3 || user.NguoiDungId == thoiGian.NguoiDungId)
            {
                if (thoiGian.CongViecId != null)
                {
                    CongViecProvider cvp = new CongViecProvider();
                    var cv = cvp.GetById(thoiGian.CongViecId.Value);
                    if (cv.LoaiTimer != 1)
                    {
                        return cvp.DeleteTimeEntry(cv.CongViecId);
                    }
                    else
                    {
                        return thoiGianLamViecProvider.Delete(thoiGianLamViecId);
                    }
                }
                else
                {
                    return thoiGianLamViecProvider.Delete(thoiGianLamViecId);
                }
            }
            else
            {
                return false;
            }



        }

        public bool CheckStartTime(Guid userId)
        {
            return thoiGianLamViecProvider.CheckStartTime(userId, DateTime.Now);
        }

        public bool CheckInsertTimeEntry(string timeFrom, string TimeTo, DateTime ngayLamViec, Guid userId)
        {
            var nguoiDung = _nguoiDungP.GetById(userId);
            var startTime = getDateTime(ngayLamViec, timeFrom);
            var endTime = getDateTime(ngayLamViec, TimeTo);
            return thoiGianLamViecProvider.CheckInsertTime2(startTime, endTime, userId);
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

        private int getTimeOfDayType(byte dayType)
        {
            int totalTime = 0;
            switch (dayType)
            {
                case 4:
                    totalTime = 3600 * 8;
                    break;
                case 5:
                    totalTime = 3600 * 4;
                    break;
                case 6:
                    totalTime = 3600 * 8;
                    break;
                default:
                    totalTime = 0;
                    break;
            }
            return totalTime;
        }

        public bool AddTimeToDb(string fileName)
        {
            List<NguoiDungViewModel> nguoiDungs = _nguoiDungP.GetAll();
            if (nguoiDungs != null && nguoiDungs.Count > 0)
            {
                foreach (var nguoiDung in nguoiDungs)
                {
                    try
                    {
                        ExcelOpenXml excel = new ExcelOpenXml(fileName, nguoiDung.HoTen);
                        var table = excel.ReadDataTable(6, false);
                        if (table != null && table.Rows.Count > 0)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                var date = row[0].ToString();
                                if (!string.IsNullOrEmpty(date))
                                {
                                    var day = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                                    if (!AddTime(row[1].ToString(), day, row[4].ToString(), nguoiDung.NguoiDungId)) return false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return true;
        }

        public bool AddTime(string time, DateTime day, string offtime, Guid userId)
        {
            var col = time.Split(':');
            var hours = int.Parse(col[0].ToString());
            var minus = int.Parse(col[1].ToString());
            var col1 = offtime.Split(':');
            var hoursOff = int.Parse(col1[0].ToString());
            var minusOff = int.Parse(col1[1].ToString());
            DateTime startTime = getDateTime(day, "08:00");
            var timeNghiTruaF = getDateTime(day, "12:00");
            var timeNghiTruaT = getDateTime(day, "13:00");
            DateTime endTime = getDateTime(day, "08:00");
            if (day.Day == 30)
            {
                var c = 1;

            }
            if (hours < 4)
            {
                if (hours > 0 || minus > 0)
                {
                    endTime = startTime.AddHours(hours).AddMinutes(minus);
                    if (!addTimeToDb(startTime, endTime, userId, null)) return false;
                }
            }
            else
            {
                endTime = startTime.AddHours(4);
                if (!addTimeToDb(startTime, endTime, userId, null)) return false;
                startTime = endTime.AddHours(1);
                if (hours > 4 || (hours == 4 && minus > 0))
                {
                    endTime = endTime.AddHours(hours - 3).AddMinutes(minus);
                    if (!addTimeToDb(startTime, endTime, userId, null)) return false;
                }


            }
            if (hours > 0 || minus > 0)
            {
                startTime = endTime.AddTicks(1);
                if (hoursOff > 0 || minusOff > 0)
                {
                    if (startTime < timeNghiTruaF) startTime = timeNghiTruaT;
                    endTime = startTime.AddHours(hoursOff).AddMinutes(minusOff);
                    if (thoiGianLamViecProvider.Insert(null, userId, startTime, endTime, 3, (int)(endTime - startTime).TotalSeconds, true, null, null, null, startTime) == 0) return false;
                }

            }
            else
            {
                if (hoursOff < 4)
                {
                    if (minusOff > 0 || hoursOff > 0)
                    {
                        endTime = startTime.AddHours(hoursOff).AddMinutes(minusOff);
                        thoiGianLamViecProvider.Insert(null, userId, startTime, endTime, 3, (int)(endTime - startTime).TotalSeconds, true, null, null, null, startTime);
                    }

                }
                else
                {
                    endTime = startTime.AddHours(4);
                    if (thoiGianLamViecProvider.Insert(null, userId, startTime, endTime, 3, (int)(endTime - startTime).TotalSeconds, true, null, null, null, startTime) == 0) return false;
                    if (hoursOff > 4 || (hoursOff == 4 && minusOff > 0))
                    {
                        startTime = timeNghiTruaT;
                        endTime = endTime.AddHours(hoursOff - 3).AddMinutes(minusOff);
                        if (thoiGianLamViecProvider.Insert(null, userId, startTime, endTime, 3, (int)(endTime - startTime).TotalSeconds, true, null, null, null, startTime) == 0) return false;
                    }
                }
            }
            return true;

        }

        private bool addTimeToDb(DateTime startTime, DateTime endTime, Guid userId, string tokenId)
        {
            try
            {
                var nguoiDung = _nguoiDungP.GetById(userId);
                var timeFrom = getDateTime(startTime, nguoiDung.KhungThoiGianBatDau);
                var timeTo = getDateTime(startTime, nguoiDung.KhungThoiGianKetThuc);
                var timeNghiTruaF = getDateTime(startTime, "12:00");
                var timeNghiTruaT = getDateTime(startTime, "13:00");

                if (endTime <= timeNghiTruaF || startTime >= timeNghiTruaT)
                {
                    if (thoiGianLamViecProvider.Insert(null, userId, startTime, endTime, 1, (int)(endTime - startTime).TotalSeconds, true, "", null, tokenId, startTime) == 0) return false;
                }
                else
                {
                    if (startTime <= timeNghiTruaF)
                    {
                        if (endTime >= timeNghiTruaT)
                        {
                            if (thoiGianLamViecProvider.Insert(null, userId, startTime, timeNghiTruaF, 1, (int)(timeNghiTruaF - startTime).TotalSeconds, true, "", null, tokenId, startTime) == 0) return false;
                            if (thoiGianLamViecProvider.Insert(null, userId, timeNghiTruaT, endTime, 1, (int)(endTime - timeNghiTruaT).TotalSeconds, true, "", null, tokenId, startTime) == 0) return false;
                        }
                        else
                        {
                            if (thoiGianLamViecProvider.Insert(null, userId, startTime, timeNghiTruaF, 1, (int)(timeNghiTruaF - startTime).TotalSeconds, true, "", null, tokenId, startTime) == 0) return false;
                        }

                    }
                    else
                    {
                        if (startTime >= timeNghiTruaF && startTime <= timeNghiTruaT)
                        {
                            if (thoiGianLamViecProvider.Insert(null, userId, timeNghiTruaT, endTime, 1, (int)(endTime - timeNghiTruaT).TotalSeconds, true, "", null, tokenId, startTime) == 0) return false;
                        }
                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
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
                result = "0h00p";
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

        public List<DanhSachChoDuyetViewModel> GetDanhSachChoDuyetBy(List<string> emails)
        {
            List<DanhSachChoDuyetViewModel> lstToReturn = new List<DanhSachChoDuyetViewModel>();
            CongViecProvider cvM = new CongViecProvider();
            var nguoiDungs = _nguoiDungP.GetNguoiDungByEmails(emails);
            if (nguoiDungs != null && nguoiDungs.Count > 0)
            {
                foreach (var nguoiDung in nguoiDungs)
                {
                    var lst = thoiGianLamViecProvider.GetDanhSachChoDuyetsBy(nguoiDung.NguoiDungId);
                    var lstNgay = thoiGianLamViecProvider.GetNgayChoDuyetsBy(nguoiDung.NguoiDungId);
                    List<LichLamViecCaNhanViewModel> lamcns = new List<LichLamViecCaNhanViewModel>();
                    if (lstNgay != null && lstNgay.Count > 0)
                    {
                        var startDay = lstNgay[lstNgay.Count - 1];
                        var endDay = lstNgay[0];
                        var startDayToInt = (int)(startDay - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                        var endDayToInt = (int)(endDay - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                        lamcns = getLichLamViecCaNhan(nguoiDung.Email, startDayToInt, endDayToInt);
                    }
                    if (lst != null && lst.Count > 0)
                    {
                        foreach (var item in lst)
                        {
                            DanhSachChoDuyetViewModel ds = new DanhSachChoDuyetViewModel();
                            if (item.CongViecId != null)
                            {
                                var congViec = cvM.GetById2(item.CongViecId.Value);
                                if (congViec != null)
                                {
                                    ds.TenTask = congViec.TenCongViec;
                                    ds.TenToDo = congViec.TenKhoaCha;
                                    ds.TenDuAn = congViec.TenDuAn;
                                    var todo = cvM.GetById2(congViec.KhoaChaId.Value);
                                    ds.TenShipAble = todo.TenKhoaCha;
                                }
                            }
                            ds.LyDo = item.NoiDungLamViec;
                            ds.NgayDangKy = item.NgayLamViec;
                            ds.NgayDangKyInSeconds = (int)(ds.NgayDangKy - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                            if (item.LoaiNgayLamViec == null)
                            {
                                foreach (var lamcn in lamcns)
                                {
                                    if (String.Format("dd/MM/yyyy", lamcn.Day) == String.Format("dd/MM/yyyy", ds.NgayDangKy))
                                    {
                                        ds.DayTypeId = (byte)lamcn.DayType.DayTypeId;
                                        ds.DayType = lamcn.DayType.Name;
                                    }
                                }
                            }
                            else {
                                ds.DayTypeId = item.LoaiNgayLamViec.Value;  
                            }
                            ds.ThoiGianLamViecId = item.ThoiGianLamViecId;
                            ds.Email = nguoiDung.Email;
                            ds.HoTen = nguoiDung.HoTen;
                            ds.LinkHuyDuyet = System.Configuration.ConfigurationManager.AppSettings.Get("LinkHuyDuyet") + "?thoiGianLamViecId=" + item.ThoiGianLamViecId;
                            ds.LinkPheDuyet = System.Configuration.ConfigurationManager.AppSettings.Get("LinkPheDuyet") + "?thoiGianLamViecId=" + item.ThoiGianLamViecId;
                            if (item.ThoiGianBatDau != null)
                            {
                                ds.ThoiGianBatDau = item.ThoiGianBatDau.Value;
                                ds.ThoiGianBatDauInSeconds = (int)(ds.ThoiGianBatDau - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                            }
                            if (item.ThoiGianKetThuc != null)
                            {
                                ds.ThoiGianKetThuc = item.ThoiGianKetThuc.Value;
                                ds.ThoiGianKetThucInSeconds = (int)(ds.ThoiGianKetThuc - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                            }
                            ds.TongThoiGian = GetStrTime(item.TongThoiGian);
                            ds.TokenId = item.TokenId;                        
                            lstToReturn.Add(ds);
                        }
                    }

                }
            }
            return lstToReturn;
        }

        private List<LichLamViecCaNhanViewModel> getLichLamViecCaNhan(string email, int startDateToInt, int endDateToInt)
        {
            try
            {
                var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings.Get("ApiLichLamViecCaNhan") + "?email=" + email + "&startDate=" + startDateToInt + "&endDate=" + endDateToInt);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<List<LichLamViecCaNhanViewModel>>(response.Content);
                //return response.Content;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ResultViewModel AddOffTime(CongViecViewModel vm, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecProvider congViecProvider = new CongViecProvider();
            var user = _nguoiDungP.GetById(userId);
            if (user.Quyen != 3) vm.NguoiXuLyId = userId;
            var ngayLamViec = DateTime.ParseExact(vm.StrNgayLamViec, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var startTime = getDateTime(ngayLamViec, vm.StrThoiGianBatDau);
            var endTime = getDateTime(ngayLamViec, vm.StrThoiGianKetThuc);
            var current_todo = GetCurrentTodoCountTime(vm.NguoiXuLyId.Value);
            if (userId != vm.NguoiXuLyId.Value)
            {
                current_todo = null;
            }

            var week = Helpers.GetNumerWeek(ngayLamViec);
            if (current_todo != null && current_todo.CongViecId > 0)
            {
                result.Status = false;
                result.Message = "Cần dừng công việc hiện tại để có thể đăng ký Công việc mới!";
            }
            else
            {
                if (startTime >= endTime)
                {
                    result.Status = false;
                    result.Message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc!";
                }
                else
                 if ((endTime - startTime).TotalHours > 4)
                {

                }
                else
                {
                    if (CheckInsertTimeEntry(vm.StrThoiGianBatDau, vm.StrThoiGianKetThuc, ngayLamViec, vm.NguoiXuLyId.Value))
                    {
                        var congViec = congViecProvider.GetTenCongViecById(vm.CongViecId);
                        var newId = congViecProvider.InsertEntry(vm.DuAnId, vm.CongViecId, userId, vm.NguoiXuLyId.Value, congViec.TenCongViec, vm.MoTa, week, vm.Nam);
                        if (newId > 0)
                        {
                            if (thoiGianLamViecProvider.Insert(newId, vm.NguoiXuLyId.Value, startTime, endTime, 3, (int)(endTime - startTime).TotalSeconds, null, vm.MoTa, null, null, ngayLamViec) > 0)
                            {
                                result.Status = true;
                                result.Message = "Đăng ký giờ thành công";
                            }
                            else
                            {
                                result.Status = false;
                                result.Message = "Error";
                            }
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "Error";
                        }



                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Thời gian đăng ký bị trùng!";
                    }
                }

            }
            return result;
        }

        public ResultViewModel AddCongTac(CongViecViewModel vm, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecProvider congViecProvider = new CongViecProvider();
            var user = _nguoiDungP.GetById(userId);
            if (user.Quyen != 3) vm.NguoiXuLyId = userId;
            var startTime = DateTime.ParseExact(vm.StrThoiGianBatDau, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var endTime = DateTime.ParseExact(vm.StrThoiGianKetThuc, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            int congViecId = 0;
            var shipId = congViecProvider.GetShipAbleByTen("Công tác", vm.DuAnId);
            if (shipId == 0) shipId = congViecProvider.Insert(new CongViecViewModel() { LaShipAble = true, LaTask = false, DuAnId = vm.DuAnId, TenCongViec = "Công tác", TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.shipableDoing, Nam = DateTime.Now.Year, Tuan = Helpers.GetNumerWeek(DateTime.Now), IsToDoAdd = false });
            var taskId = congViecProvider.GetTaskByTen("Công tác", shipId, vm.DuAnId);
            if (taskId == 0) taskId = congViecProvider.Insert(new CongViecViewModel() { LaShipAble = false, LaTask = true, DuAnId = vm.DuAnId, TenCongViec = "Công tác", TrangThaiCongViecId = (int)EnumTrangThaiCongViecType.shipableDoing, Nam = DateTime.Now.Year, Tuan = Helpers.GetNumerWeek(DateTime.Now), IsToDoAdd = false, KhoaChaId = shipId });
            while (startTime <= endTime)
            {
                var nam = startTime.Year;
                var week = Helpers.GetNumerWeek(startTime);
                var newId = congViecProvider.InsertEntry(vm.DuAnId, taskId, userId, userId, "Công tác", "Công tác", week, nam);
                if (newId > 0)
                {
                    if (thoiGianLamViecProvider.OverWriteTime(userId, startTime.AddHours(8), startTime.AddHours(16), null, 4, newId, null))
                    {
                        result.Status = true;
                        result.Message = "Đăng ký giờ thành công";
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Error";
                    }
                }
                else
                {
                    result.Status = false;
                    result.Message = "Error";
                }
                startTime = startTime.AddDays(1);
            }





            return result;
        }


        public ResultViewModel AddQTA(CongViecViewModel vm, TokenViewModel token, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecProvider congViecProvider = new CongViecProvider();
            var user = _nguoiDungP.GetById(userId);
            var ngayLamViec = DateTime.ParseExact(vm.StrNgayLamViec, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var startTime = getDateTime(ngayLamViec, vm.StrThoiGianBatDau);
            var endTime = getDateTime(ngayLamViec, vm.StrThoiGianKetThuc);
            var current_todo = GetCurrentTodoCountTime(userId);
            var week = Helpers.GetNumerWeek(ngayLamViec);
            if (token == null || string.IsNullOrEmpty(token.key_token))
            {
                result.Status = false;
                result.Message = "Bạn chưa lấy token trong ngày " + vm.NgayLamViec.ToString("dd/MM/yyyy") + "!";
            }
            else
            {
                if (current_todo != null && current_todo.CongViecId > 0)
                {
                    result.Status = false;
                    result.Message = "Cần dừng công việc hiện tại để có thể đăng ký Công việc mới!";
                }
                else
                {
                    if (startTime >= endTime)
                    {
                        result.Status = false;
                        result.Message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc!";
                    }
                    else
                    {
                        if (CheckInsertTimeEntry(vm.StrThoiGianBatDau, vm.StrThoiGianKetThuc, ngayLamViec, userId))
                        {
                            var check = true;
                            var timeFromDefault = getDateTime(vm.NgayLamViec, user.KhungThoiGianBatDau);
                            var timeToDefault = getDateTime(vm.NgayLamViec, user.KhungThoiGianKetThuc);
                            if (startTime < timeFromDefault || startTime > timeToDefault || timeToDefault < timeFromDefault || timeToDefault > timeToDefault)
                            {
                                check = false;
                                result.Status = false;
                                result.Message += "Thời gian bạn thêm phải trong khoảng " + user.KhungThoiGianBatDau + "-" + user.KhungThoiGianKetThuc + "!";
                            }
                            if (startTime < token.manufacturing_date)
                            {
                                check = false;
                                result.Status = false;
                                result.Message += "Thời gian bạn thêm phải sau khoảng thời gian bạn lấy token " + token.manufacturing_date.ToString("dd/MM/yyyy HH:mm") + " !";
                            }
                            if (check)
                            {
                                var lstTodoIds = congViecProvider.GetToDoIdsBy(vm.CongViecId, userId);
                                int toDoId = 0;
                                if (lstTodoIds != null && lstTodoIds.Count > 0)
                                {
                                    toDoId = lstTodoIds[0];
                                }
                                else
                                {
                                    CongViecModel cvM = new CongViecModel();
                                    toDoId = cvM.InsertToDoByKhoaCha(vm.CongViecId, userId);
                                }
                                if (toDoId > 0)
                                {
                                    if (thoiGianLamViecProvider.Insert(toDoId, userId, startTime, endTime, 1, (int)(endTime - startTime).TotalSeconds, true, null, null, token.key_token, ngayLamViec) > 0)
                                    {
                                        result.Status = true;
                                        result.Message = "Đăng ký giờ thành công";
                                    }
                                    else
                                    {
                                        result.Status = false;
                                        result.Message = "Error";
                                    }
                                }
                                else
                                {
                                    result.Status = false;
                                    result.Message = "Error";
                                }
                            }

                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "Thời gian đăng ký bị trùng!";
                        }
                    }
                }
            }



            return result;
        }

        public ResultViewModel AddQTA2(CongViecViewModel vm, TokenViewModel token, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            CongViecProvider congViecProvider = new CongViecProvider();
            var user = _nguoiDungP.GetById(userId);
            var ngayLamViec = DateTime.ParseExact(vm.StrNgayLamViec, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var startTime = getDateTime(ngayLamViec, vm.StrThoiGianBatDau);
            var endTime = getDateTime(ngayLamViec, vm.StrThoiGianKetThuc);
            var current_todo = GetCurrentTodoCountTime(userId);
            var week = Helpers.GetNumerWeek(ngayLamViec);

            if (current_todo != null && current_todo.CongViecId > 0)
            {
                result.Status = false;
                result.Message = "Cần dừng công việc hiện tại để có thể đăng ký Công việc mới!";
            }
            else
            {
                if (startTime >= endTime)
                {
                    result.Status = false;
                    result.Message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc!";
                }
                else
                {
                    if (CheckInsertTimeEntry(vm.StrThoiGianBatDau, vm.StrThoiGianKetThuc, ngayLamViec, userId))
                    {
                        var check = true;
                        var timeFromDefault = getDateTime(vm.NgayLamViec, user.KhungThoiGianBatDau);
                        var timeToDefault = getDateTime(vm.NgayLamViec, user.KhungThoiGianKetThuc);
                        if (startTime < timeFromDefault || startTime > timeToDefault || endTime < timeFromDefault || endTime > timeToDefault)
                        {
                            check = false;
                        }
                        if (token == null || string.IsNullOrEmpty(token.key_token))
                        {
                            check = false;
                        }
                        
                        var lstTodoIds = congViecProvider.GetToDoIdsBy(vm.CongViecId, userId);
                        int toDoId = 0;
                        if (lstTodoIds != null && lstTodoIds.Count > 0)
                        {
                            toDoId = lstTodoIds[0];
                        }
                        else
                        {
                            CongViecModel cvM = new CongViecModel();
                            toDoId = cvM.InsertToDoByKhoaCha(vm.CongViecId, userId);
                        }
                        if (toDoId > 0)
                        {
                            var newT = 0;
                            if (check) newT = thoiGianLamViecProvider.Insert(toDoId, userId, startTime, endTime, 1, (int)(endTime - startTime).TotalSeconds, true, null, null, token.key_token, ngayLamViec);
                            else newT = thoiGianLamViecProvider.Insert(toDoId, userId, startTime, endTime, 3, (int)(endTime - startTime).TotalSeconds, null, null, null, null, ngayLamViec);
                            if (newT > 0)
                            {
                                result.Status = true;
                                result.Message = "Đăng ký giờ thành công";
                            }
                            else
                            {
                                result.Status = false;
                                result.Message = "Error";
                            }
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "Error";
                        }


                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Thời gian đăng ký bị trùng!";
                    }
                }
            }




            return result;
        }

        public ResultViewModel AddDayTime(CongViecViewModel vm, Guid userId)
        {
            ResultViewModel result = new ResultViewModel();
            var ngayLamViec = DateTime.ParseExact(vm.StrNgayLamViec, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var current_todo = GetCurrentTodoCountTime(userId);
            if (current_todo != null && current_todo.CongViecId > 0)
            {
                result.Status = false;
                result.Message = "Cần dừng công việc hiện tại để có thể đăng ký Công việc mới!";
            }
            else
            {
                if (!thoiGianLamViecProvider.CheckIsExitDayType(userId, ngayLamViec, vm.DayType))
                {
                    //check xem đã add ngày nghỉ vào ngày hôm đấy chưa.
                    var time = thoiGianLamViecProvider.GetAddDayTypeOfUser(userId, ngayLamViec);
                    if (time != null)
                    {
                        if (time.PheDuyet == true)
                        {
                            result.Status = false;
                            result.Message = "Ngày " + String.Format("{0:dd/MM/yyyy}", ngayLamViec) + " bạn đã đăng ký và được phê duyệt rồi, bạn không đăng ký tiếp được";
                        }
                        else
                        {
                            thoiGianLamViecProvider.Delete(time.ThoiGianLamViecId);
                        }
                    }
                    else
                    {
                        int total = 0;
                        var danhSachNgay = getDanhSachLoaiNgayDangKy();
                        foreach (var item in danhSachNgay)
                        {
                            if (item.DayTypeId == vm.DayType)
                            {
                                if (item.AddStaffLeaveInDays == -0.5)
                                {
                                    total = 3600 * 4;
                                }
                                else
                                {
                                    total = 3600 * 8;
                                }
                            }
                        }

                        if (thoiGianLamViecProvider.Insert2(null, userId, null, null, 3, total, null, vm.MoTa, null, null, ngayLamViec, vm.DayType) > 0)
                        {
                            result.Status = true;
                            result.Message = "Đăng ký giờ thành công";
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "Error";
                        }
                    }


                }
                else
                {
                    result.Status = false;
                    result.Message = "Bạn đã đăng ký rôì";
                }
            }
            return result;
        }

        public ResourceUserViewModel GetsBy(int? phongBanId, List<Guid> nguoiDungIds, DateTime startDate, DateTime endDate)
        {
            ResourceUserViewModel result = new ResourceUserViewModel();
            CongViecProvider manager = new CongViecProvider();
            DuAnProvider duAnM = new DuAnProvider();
            var i = 1;
            List<DuAnViewModel> duAnVms = new List<DuAnViewModel>();
            List<NguoiDungViewModel> lstToReturn = new List<NguoiDungViewModel>();
            if (nguoiDungIds != null && nguoiDungIds.Count > 0)
            {
                foreach (var id in nguoiDungIds)
                {
                    lstToReturn.Add(_nguoiDungP.GetById(id));
                }

            }
            else if (phongBanId > 0) lstToReturn.AddRange(_nguoiDungP.GetNguoiDungsByPhongBanId(phongBanId.Value));
            else lstToReturn.AddRange(_nguoiDungP.GetAll());
            int maxTotal = 0;
            if (lstToReturn.Count > 0)
            {
                var startDateToInt = (int)(startDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                var endDateToInt = (int)(endDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                var danhSachNgay = getDanhSachLoaiNgayDangKy();
                if (danhSachNgay != null)
                {
                    foreach (var nguoiDung in lstToReturn)
                    {
                        var lichLamViecCaNhan = getLichLamViecCaNhan(nguoiDung.Email, startDateToInt, endDateToInt);
                        int totalTimeNeed = 0;
                        if (lichLamViecCaNhan != null && lichLamViecCaNhan.Count > 0)
                        {
                            foreach (var ngayLamViec in lichLamViecCaNhan)
                            {
                                foreach (var item in danhSachNgay)
                                {
                                    if (item.DayTypeId == ngayLamViec.DayType.DayTypeId)
                                    {
                                        ngayLamViec.TongThoiGianOffTime += getTimeOfDayType((byte)item.DayTypeId);
                                    }
                                }
                                totalTimeNeed += (int)(ngayLamViec.DayType.TimeRequireInSeconds);
                                totalTimeNeed += getTimeOfDayType((byte)ngayLamViec.DayType.DayTypeId);
                            }
                        }
                        nguoiDung.TotalTimeNeed = totalTimeNeed;
                        nguoiDung.StrToTalTimeNeed = GetStrTime2(totalTimeNeed);
                        var lstTimeUser = thoiGianLamViecProvider.GetThoiGianLamViecsBy(nguoiDung.NguoiDungId, startDate, endDate);
                        int totaltime = 0;
                        if (lstTimeUser != null && lstTimeUser.Count() > 0)
                        {
                            List<DuAnViewModel> duAns = new List<DuAnViewModel>();
                            foreach (var time in lstTimeUser)
                            {
                                var congViec = manager.GetById(time.CongViecId.Value);
                                var duAn = duAns.Where(t => t.DuAnId == congViec.DuAnId).FirstOrDefault();
                                if (duAn == null)
                                {
                                    duAn = duAnM.GetById(congViec.DuAnId);
                                    duAn.CongViecs = new List<CongViecViewModel>();
                                    duAns.Add(duAn);
                                }
                                if (duAnVms.Where(t => t.DuAnId == duAn.DuAnId).FirstOrDefault() == null)
                                {
                                    duAn.BackGroundColor = "grogressBg" + i;
                                    duAnVms.Add(duAn);
                                    i++;
                                }
                                else
                                {
                                    var duAn2 = duAnVms.Where(t => t.DuAnId == duAn.DuAnId).FirstOrDefault();
                                    if (duAn2 != null)
                                        duAn.BackGroundColor = duAn2.BackGroundColor;
                                }
                                var ship = duAn.CongViecs.Where(t => t.CongViecId == congViec.KhoaChaId).FirstOrDefault();
                                if (congViec.KhoaChaId == null) ship = congViec;
                                if (ship == null)
                                {
                                    ship = manager.GetById(congViec.KhoaChaId.Value);
                                    ship.TongThoiGian = time.TongThoiGian;
                                    duAn.CongViecs.Add(ship);
                                }
                                else
                                {
                                    ship.TongThoiGian += time.TongThoiGian;
                                }
                                totaltime += time.TongThoiGian;
                            }
                            nguoiDung.DuAns = duAns;
                            nguoiDung.StrTotalTime = GetStrTime(totaltime);
                        }
                        if (maxTotal < totaltime) maxTotal = totaltime;
                    }
                    maxTotal = (int)Math.Round(maxTotal * 1.02, 0);
                    result.MaxTotalTime = maxTotal;
                    result.StrMaxTotalTime = GetStrTime(maxTotal);
                    foreach (var nguoiDung in lstToReturn)
                    {
                        if (nguoiDung.DuAns != null && nguoiDung.DuAns.Count > 0)
                        {
                            foreach (var duAn in nguoiDung.DuAns)
                            {
                                if (duAn.CongViecs != null && duAn.CongViecs.Count > 0)
                                {
                                    foreach (var ship in duAn.CongViecs)
                                    {
                                        if (maxTotal > 0)
                                            ship.PercentTime = Math.Round(((decimal)ship.TongThoiGian / (decimal)maxTotal) * 100, 2);
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
            result.NguoiDungs = lstToReturn;
            return result;
        }

        public List<NgayLamCongViecViewModel> GetToDosByV2(int? phongBanId, List<Guid> nguoiDungIds, DateTime startDate, DateTime endDate)
        {
            List<NgayLamCongViecViewModel> lstToReturn = new List<NgayLamCongViecViewModel>();
            CongViecProvider manager = new CongViecProvider();
            DuAnProvider duAnM = new DuAnProvider();
            List<DuAnViewModel> duAnVms = new List<DuAnViewModel>();
            List<NguoiDungViewModel> lstNguoiDung = new List<NguoiDungViewModel>();
            if (nguoiDungIds == null || nguoiDungIds.Count == 0)
            {
                if (phongBanId > 0) lstNguoiDung.AddRange(_nguoiDungP.GetNguoiDungsByPhongBanId(phongBanId.Value));
                else lstNguoiDung.AddRange(_nguoiDungP.GetAll());

                if (lstNguoiDung != null && lstNguoiDung.Count > 0)
                {
                    nguoiDungIds = lstNguoiDung.Select(t => t.NguoiDungId).ToList();
                }
            }
            var result = manager.GetTodoByV2(nguoiDungIds, startDate, endDate);
            lstToReturn = getNgayLamViecs(startDate, endDate);
            if (lstToReturn != null && lstToReturn.Count > 0 && result != null && result.Count > 0)
            {
                TrangThaiCongViecProvider ttcvM = new TrangThaiCongViecProvider();
                var lstTrangThai = ttcvM.GetByKhoaChaId((int)EnumTrangThaiCongViecType.todoNew);
                foreach (var item in lstToReturn)
                {
                    var times = result.Where(t => (t.NgayTao >= item.NgayLamViec && t.IsToDoAdd == false && t.NgayTao <= item.End) || (t.IsToDoAdd == true && t.NgayDuKienHoanThanh >= item.NgayLamViec && t.NgayDuKienHoanThanh <= item.End));
                    if (times != null && times.Count() > 0)
                    {
                        var lstToDo = new List<CongViecViewModel>();
                        foreach (var time in times)
                        {
                            var en = manager.GetTimeFromCongViecId(time.CongViecId);
                            if (en != null)
                            {
                                time.ThoiGianBatDau = en.ThoiGianBatDau;
                                time.ThoiGianKetThuc = en.ThoiGianKetThuc;
                                time.TongThoiGian = en.TongThoiGian;
                                if (time.TongThoiGian > 0)
                                {
                                    time.StrTotalTime = GetStrTime2(time.TongThoiGian);
                                    if (time.ThoiGianBatDau != null)
                                    {
                                        time.StrTotalTime += " <br/>  <span class=\"text-mute font-11 font-w-300\">(" + time.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm");
                                        if (time.ThoiGianKetThuc != null) time.StrTotalTime += " - " + time.ThoiGianKetThuc.Value.ToString("dd/MM/yyyy HH:mm");
                                        time.StrTotalTime += ")</span>";
                                    }

                                }
                                else if (time.ThoiGianBatDau != null)
                                {
                                    time.StrTotalTime = "Thời gian bắt đâu: " + time.ThoiGianBatDau.Value.ToString("dd/MM/yyyy hh:mm");
                                }


                            }
                            time.TrangThaiCongViecs = new List<TrangThaiCongViecViewModel>();
                            if (time.TrangThaiCongViecId == (int)EnumTrangThaiCongViecType.todoNew) time.TrangThaiCongViecs.Add(new TrangThaiCongViecViewModel { TrangThaiCongViecId = time.TrangThaiCongViecId, TenTrangThai = time.TenTrangThai });
                            time.TrangThaiCongViecs.AddRange(lstTrangThai);
                        }
                        item.ToDos = times;
                    }
                }
            }
            return lstToReturn;
        }

        private List<NgayLamCongViecViewModel> getNgayLamViecs(DateTime start, DateTime end)
        {
            List<NgayLamCongViecViewModel> lstToReturn = new List<NgayLamCongViecViewModel>();
            while (end >= start)
            {
                var ngayLamViec = new NgayLamCongViecViewModel { NgayLamViec = start, End = start.AddDays(1).AddTicks(-1) };
                if (start.DayOfWeek == DayOfWeek.Sunday) ngayLamViec.DisPlayNgayLamViec = "Chủ nhật";
                else ngayLamViec.DisPlayNgayLamViec = "Thứ" + ((int)start.DayOfWeek + 1);
                lstToReturn.Add(ngayLamViec);
                start = start.AddDays(1);
            }


            return lstToReturn;
        }

        private List<DayType> getDanhSachLoaiNgayDangKy()
        {
            try
            {
                var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings.Get("ApiDanhMucLoaiNgayDangKy"));
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<List<DayType>>(response.Content);
                //return response.Content;
            }
            catch
            {
                return null;
            }

        }

        public List<ActivityViewModel> GetDanhSachActivity(string email, int startTime, int endTime)
        {
            try
            {
                var str = System.Configuration.ConfigurationManager.AppSettings.Get("LinkActivityCRM") + "?startTime=" + startTime + "000&endTime=" + endTime + "000&email=" + email;
                var client = new RestClient(str);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                var lst = JsonConvert.DeserializeObject<List<ActivityViewModel>>(response.Content);
                if (lst != null && lst.Count > 0)
                {
                    foreach(var item in lst)
                    {
                        item.DNgayBatDau = DateTime.ParseExact(item.NgayBatDau, "dd/MM/yyyy HH:mm",CultureInfo.CurrentCulture);
                    }
                }
                return lst;
                //return response.Content;
            }
            catch
            {
                return null;
            }

        }

        //private bool addOffTimeToDb(DateTime startTime, DateTime endTime, Guid userId)
        //{

        //}

        public class SendMessageBlockToUserSlackViewModel
        {
            public string channel { get; set; }
            public string text { get; set; }
            public bool as_user { get; set; }
            public string blocks { get; set; }
        }
        public class SendMessageBlockToChannelSlackViewModel
        {
            public string channel { get; set; }
            public string text { get; set; }           
            public string blocks { get; set; }
        }

        public class SendMessageToChannelSlackViewModel
        {
            public string channel { get; set; }
            public string text { get; set; }         
        }
        public class SendMessageToUserSlackViewModel
        {
            public string channel { get; set; }
            public string text { get; set; }
            public bool as_user { get; set; }
        }
        public bool RegisterToken(string email, int ngayLamViecToInt)
        {
           
                var link = System.Configuration.ConfigurationManager.AppSettings.Get("RegisterToken3");
                var client = new RestClient(link);
                var request = new RestRequest(Method.POST);
            var nguoiDung = _nguoiDungP.GetNguoiDungByEmail(email);
                request.AddHeader("Content-Type", "application/json");
                RegisterMyTokenViewModel vm = new RegisterMyTokenViewModel();
                vm.UserName = nguoiDung.TenDangNhap;
                vm.Password = EndCode.Decrypt(nguoiDung.MatKhau);
            vm.DateTimeToInt = ngayLamViecToInt;
                var json = JsonConvert.SerializeObject(vm);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);            
            return true;
        }
        private TokenViewModel GetTokenKey2(DateTime date,Guid userId)
        {
            try
            {                
                var time = (date - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                var str = System.Configuration.ConfigurationManager.AppSettings.Get("ApiGetTokenKeyV2")+ "/" +userId.ToString() + "/" + time;
                var client = new RestClient(str);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<TokenViewModel>(response.Content);
            }
            catch
            {
                return new TokenViewModel();
            }

        }
        public bool SendMessageToSlack(string message)
        {
            var link = System.Configuration.ConfigurationManager.AppSettings.Get("apiSendMessageSlack");
            var channel = System.Configuration.ConfigurationManager.AppSettings.Get("SendMessageToChannel");
            var token = System.Configuration.ConfigurationManager.AppSettings.Get("TokenBotSlack");            
            var client = new RestClient(link);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);          
            request.AddHeader("Content-Type", "application/json");
            SendMessageToChannelSlackViewModel body = new SendMessageToChannelSlackViewModel();
            body.channel = channel;
            body.text = message;
            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        public string GetChannelIdByChannelName(string ChannelName)
        {
            
               var link = System.Configuration.ConfigurationManager.AppSettings.Get("apiConversationsList");
            var token = System.Configuration.ConfigurationManager.AppSettings.Get("TokenBotSlack");            
            var client = new RestClient(link);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);   
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                try
                {
                    var lst = JsonConvert.DeserializeObject<ConversationsListViewModel>(response.Content);
                    return lst.channels.Where(t => t.name.ToLower() == ChannelName.ToLower()).FirstOrDefault().id;
                }
                catch
                {

                }

                return "";
            }
            return "";
           
        }


        public bool SendMessageToSlackWithChannelId(string message,string channelId,bool as_user)
        {
            var link = System.Configuration.ConfigurationManager.AppSettings.Get("apiSendMessageSlack"); 
            var token = System.Configuration.ConfigurationManager.AppSettings.Get("TokenBotSlack");           
            var client = new RestClient(link);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);            
            request.AddHeader("Content-Type", "application/json");
            string text = "";
            if (as_user)
            {
                SendMessageToUserSlackViewModel body = new SendMessageToUserSlackViewModel();
                body.channel = channelId;
                body.text = message;
                body.as_user = as_user;
                text = JsonConvert.SerializeObject(body);
            }
                else
                {
                SendMessageToChannelSlackViewModel body = new SendMessageToChannelSlackViewModel();
                body.channel = channelId;
                body.text = message;              
                text = JsonConvert.SerializeObject(body);
            }
         
            request.AddParameter("application/json", text, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        public bool SendMessageToSlackWithChannelIdBlock(string message,string block, string channelId, bool as_user)
        {
            var link = System.Configuration.ConfigurationManager.AppSettings.Get("apiSendMessageSlack");
            var token = System.Configuration.ConfigurationManager.AppSettings.Get("TokenBotSlack");
           
            var client = new RestClient(link);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
          
            request.AddHeader("Content-Type", "application/json");
            string text = "";
            if (as_user)
            {
                SendMessageBlockToUserSlackViewModel body = new SendMessageBlockToUserSlackViewModel();
                body.channel = channelId;
                body.text = message;
                body.as_user = true;
                body.blocks = block;
                text = JsonConvert.SerializeObject(body);
            }
            else
            {
                SendMessageBlockToChannelSlackViewModel body = new SendMessageBlockToChannelSlackViewModel();
                body.channel = channelId;
                body.text = message;
                body.blocks = block;
                text = JsonConvert.SerializeObject(body);
            }

            request.AddParameter("application/json", text, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }
    }
}