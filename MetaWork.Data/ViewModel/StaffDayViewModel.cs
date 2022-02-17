using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MetaWork.Data.ViewModel
{
    /// <summary>
    /// Lịch thực tế của một nhân viên
    /// </summary>
    public class StaffDayViewModel : IProvideSampleApiObject
    {
        /// <summary>
        /// ID nhân viên
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Ngày mà nhân viên đó nghỉ hay làm việc
        /// </summary>
        public DateTime Day { get; set; }
        /// <summary>
        /// Chi tiết ngày nghỉ hay làm việc của nhân viên
        /// </summary>
        public int DayTypeId { get; set; }
       
        public string Description { get; set; }
        /// <summary>
        /// Nếu đăng ký là ngày nghỉ thì cần được duyệt bởi ban giám đốc
        /// </summary>
        public bool Approved { get; set; }
        /// <summary>
        /// ID người duyệt
        /// </summary>
        public Guid UserModified { get; set; }

        KeyValuePair<Type, object> IProvideSampleApiObject.CreateSampleObjectEntry()
        {
            return new KeyValuePair<Type, object>(GetType(),
               new StaffDayViewModel()
               {
                   Day = new DateTime(2018, 03, 01),
                   UserId = new Guid("5EA5835C-4AF1-4C65-8761-078C222BBB15"),
                   DayTypeId = 4,
                   Approved = true,
                   Description = "Nghỉ ốm",
                   UserModified = new Guid("F1638DA9-7927-4D7E-91C3-64C0E1169138"),
               });
        }
        KeyValuePair<Type, object> IProvideSampleApiObject.CreateSampleObjectsEntry()
        {
            return new KeyValuePair<Type, object>(GetType(),
               new StaffDayViewModel[2]
               {
                  new StaffDayViewModel()
                   {
                         Day = new DateTime(2018 , 03 , 01),
                   UserId = new Guid("5EA5835C-4AF1-4C65-8761-078C222BBB15"),
                   DayTypeId = 4,
                   Approved = true,
                   Description = "Nghỉ ốm",
                   UserModified = new Guid("F1638DA9-7927-4D7E-91C3-64C0E1169138"),
                   },
                  new StaffDayViewModel()
                   {
                        Day = new DateTime(2018, 06, 18),
                   UserId = new Guid("5EA5835C-4AF1-4C65-8761-078C222BBB15"),
                   DayTypeId = 2,
                   Approved = true,
                   Description = "Không tham gia hoạt động chung của công ty nên làm việc bình thường",
                   UserModified = new Guid("F1638DA9-7927-4D7E-91C3-64C0E1169138"),
                   },
               });
        }
    }

    /// <summary>
    /// Chi tiết về các ngày người dùng tự đăng ký
    /// </summary>
    public class QryStaffDayViewModel : StaffDayViewModel
    {

        /// <summary>
        /// Tên người dùng.
        /// </summary>
        public string HoTenNguoiDung { get; set; }

        /// <summary>
        /// Email người dùng.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Tên người dùng chỉnh sửa bản tin
        /// </summary>
        public string HoTenNguoiDungModified { get; set; }

        /// <summary>
        /// Email người dùng chỉnh sửa bản tin
        /// </summary>
        public string EmailModified { get; set; }

        /// <summary>
        /// Nội dung ghi chú khi nghỉ phép
        /// </summary>
        public string SelfDescription { get; set; }

        /// <summary>
        /// Tên loại ngày nghỉ có phép (nghỉ nửa ngày hay nghỉ một ngày)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Thời gian phải làm việc trong ngày này
        /// </summary>
        public double TimeRequireInSeconds { get; set; }

        /// <summary>
        /// Có cộng vào ngày phép không
        /// </summary>
        public double AddStaffLeaveInDays { get; set; }

    }

    /// <summary>
    /// Lấy thông tin chi tiết về các ngày đăng ký của nhân viên
    /// </summary>
    public class GetStaffDayViewModel
    {
        /// <summary>
        /// Ngày phép của năm nào.
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// ID của nhân viên cần tra cứu.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Đã approved hay chưa
        /// </summary>
        public bool? Approved { get; set; }

        ///// <summary>
        ///// Tìm theo loại ngày
        ///// </summary>
        //public int? DayTypeId { get; set; }
    }
}
