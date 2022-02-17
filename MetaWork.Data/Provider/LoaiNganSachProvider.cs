using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaWork.Data.ViewModel;

namespace MetaWork.Data.Provider
{
    public class LoaiNganSachProvider
    {
        TimerDataContext db = null;
        public LoaiNganSachProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public List<LoaiNganSachViewModel> Gets()
        {
            try
            {
                return db.ExecuteQuery<LoaiNganSachViewModel>("Select * from LoaiNganSach").ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}
