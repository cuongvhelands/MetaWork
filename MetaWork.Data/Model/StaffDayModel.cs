using MetaWork.Data.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Model
{
    public class StaffDayModel
    {
        private IDataBase _dataBaseContext;

        public StaffDayModel(IDataBase dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }

        public DataTable GetStaffDayOfUser(string email,int year, bool? approved)
        {
            try
            {
                var startTime = new DateTime(year, 1, 1);
                var endTime= new DateTime(year, 12, 31, 23, 59, 59, 999);
                var str = $"Select * from kpi.qryStaffDay where Email = '{email}' and Day >= '{startTime.ToString("yyyy-MM-dd")}' and Day <= '{endTime.ToString("yyyy-MM-dd")}'";
                if (approved != null) {
                    if (approved.Value)                    
                        str += " and approved =1";
                    else str += " and approved =0";
                } 
                return _dataBaseContext.GetDataTable(str);
                
               
            }
            catch
            {
                return null;
            }
           
        }

        public int CountStaffDayOfUser(string email, int year, bool? approved)
        {
            try
            {
                var startTime = new DateTime(year, 1, 1);
                var endTime = new DateTime(year, 12, 31, 23, 59, 59, 999);
                var str = $"Select count(Day) from kpi.qryStaffDay where Email = '{email}' and Day >= '{startTime.ToString("yyyy-MM-dd")}' and Day <= '{endTime.ToString("yyyy-MM-dd")}'";
                if (approved != null)
                {
                    if (approved.Value)
                        str += " and approved =1";
                    else str += " and approved =0";
                }
                var table= _dataBaseContext.GetDataTable(str);            
                if (table.Rows.Count > 0) return int.Parse(table.Rows[0].ItemArray[0].ToString());
                return 0;


            }
            catch
            {
                return 0;
            }

        }

        
    }
  
}
