using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Provider
{
    public class CauHinhProvider
    {
        TimerDataContext db = null;
        public CauHinhProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public int GetMaShipAble()
        {
            try
            {
                var str = "select GiaTri from CauHinh where TenCauHinh like N'MaShipable'";
                var a= db.ExecuteQuery<string>("select GiaTri from CauHinh where TenCauHinh like N'MaShipable'").FirstOrDefault();
                return int.Parse(a);
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
        public string GetValueByTen(string TenCauHinh)
        {
            try
            {
                return (from a in db.CauHinhs where a.TenCauHinh.ToUpper().Equals(TenCauHinh.ToUpper()) select a.GiaTri).FirstOrDefault();
               
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public bool InsertMaShipable()
        {
            try
            {
                var entity = db.CauHinhs.Where(t => t.TenCauHinh == "MaShipable").FirstOrDefault();
                entity.GiaTri = (int.Parse(entity.GiaTri) + 1).ToString();
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertOrUpdte(string tenCauHinh, string value)
        {
            try
            {
                try
                {
                    CauHinh entity = db.CauHinhs.Where(t => t.TenCauHinh.ToUpper() == tenCauHinh.ToUpper()).FirstOrDefault();
                    if (entity != null) entity.GiaTri = value;
                    else
                    {
                        CauHinh newch = new CauHinh();
                        newch.TenCauHinh = tenCauHinh;
                        newch.GiaTri = value;
                        db.CauHinhs.InsertOnSubmit(newch);
                    }
                    db.SubmitChanges();
                    return true;
                }
                catch
                {
                    CauHinh newch = new CauHinh();
                    newch.TenCauHinh = tenCauHinh;
                    newch.GiaTri = value;
                    db.CauHinhs.InsertOnSubmit(newch);
                    db.SubmitChanges();
                    return true;
                }
                
            }
            catch
            {
                return false;
            }
        }

    }
}
