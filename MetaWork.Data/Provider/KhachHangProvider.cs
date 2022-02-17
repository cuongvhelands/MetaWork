using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class KhachHangProvider
    {
        TimerDataContext db = null;
        public KhachHangProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public List<KhachHangViewModel> Gets()
        {
            try
            {
                return db.ExecuteQuery<KhachHangViewModel>("Select * from KhachHang").ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}
