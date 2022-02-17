using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MetaWork.Project.Models
{
    public static class EndCode
    {
        static string key = "nsndquangvinh@123#";
        /// <summary>
        /// Mã hóa chuỗi có mật khẩu
        /// </summary>
        /// <param name="toEncrypt">Chuỗi cần mã hóa</param>
        /// <returns>Chuỗi đã mã hóa</returns>
        public static string Encrypt(string toEncrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Giản mã
        /// </summary>
        /// <param name="toDecrypt">Chuỗi đã mã hóa</param>
        /// <returns>Chuỗi giản mã</returns>
        public static string Decrypt(string toDecrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
    public class Helpers
    {
        public static DateTime GetFirstMondayOfMonth(int year, int month)
        {
            DateTime dt = new DateTime(year, month, 1);
            while (dt.DayOfWeek != DayOfWeek.Monday)
            {
                dt = dt.AddDays(1);
            }
            return dt;
        }
        public static DateTime GetFirstMondayOfWeek(int year, int week)
        {
            DateTime dt = new DateTime(year, 1, 1);        
            
            while (dt.DayOfWeek != DayOfWeek.Monday)
            {
                dt = dt.AddDays(1);
            }
            var dayAdd = (week - 1) * 7;
            dt=dt.AddDays(dayAdd);
            return dt;
        }
        public static int GetNumberWeekOfYears(int year)
        {
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            DateTime LastDay = new System.DateTime(year, 12, 31);
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = DayOfWeek.Monday;
            return myCal.GetWeekOfYear(LastDay, myCWR, myFirstDOW);
        }
        public static int GetNumerWeek(DateTime date)
        {          
            var mondayOfDate = GetMonDayBy(date);
            DateTime firstMondayofYear = getFirstMondayOfYear(date.Year);
            var week= ((int)(mondayOfDate - firstMondayofYear).TotalDays / 7) + 1;
            if (mondayOfDate.Year == 2019) week++;
            return week;
        }
      
        public static int GetNumberWeekOfDay(DateTime date)
        {            
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;           
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            return myCal.GetWeekOfYear(date, myCWR, myFirstDOW);
        }


        public static DateTime getFirstMondayOfYear(int year)
        {
            var date = new DateTime(year, 1, 1);
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(1);
            }
            return date;
        }
        public static DateTime GetMonDayBy(DateTime date)
        {          
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
            }
            return date;
        }
        public static int GetNumberCurrentWeekOfMonth(DateTime date)
        {
            var monday = GetMonDayBy(date);
            var month = monday.Month;
            var year = monday.Year;
            var firstMondayOfMonth = GetFirstMondayOfMonth(year, month);
            int i = 1;
            while (firstMondayOfMonth.ToShortDateString() != monday.ToShortDateString())
            {
                firstMondayOfMonth= firstMondayOfMonth.AddDays(7);
                i++;
            }
            return i;
        }
        public static int GetNumberWeekOfMonth(int year, int month)
        {
            DateTime startDate;
            if (month == 12)
                startDate = GetFirstMondayOfMonth(year + 1, 1).AddDays(-1);
            else startDate = GetFirstMondayOfMonth(year, month + 1).AddDays(-1);
            return GetNumberCurrentWeekOfMonth(startDate);
        }
    }
 
}