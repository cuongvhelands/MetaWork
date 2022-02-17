using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public class TrangThaiCongViecViewModel
    {
        public int TrangThaiCongViecId { get; set; }
        public int? KhoaChaId { get; set; }
        public string TenTrangThai { get; set; }
        public string MaMau { get; set; }
    }
    public enum EnumTrangThaiCongViecType
    {      

        shipable = 7,     
        congviec = 8,
        todo = 13,
        shipableDoing=2,
        shipableDone=4,
        shipableNew=1,
        shipableDebit=6,
        congviecNew=18,
        congviecDoing = 9,
        congViecBlock = 10,
        congViecDone =11,
        shipableCheck=3,
        shipableContinue=12,
        shipablePlan=1,
        todoNew = 14,
        todoDo=15,
        todoDone=16,
        todoBlock=17

    }
}
