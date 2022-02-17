using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.Provider
{
    public class FileProvider
    {
        TimerDataContext db = null;
        public FileProvider()
        {
            if (string.IsNullOrEmpty(Main.AdminConnStr))
                db = new TimerDataContext();
            else
                db = new TimerDataContext(Main.AdminConnStr);
        }
        public FileViewModel GetById(Guid fileId)
        {
            try
            {
                return db.ExecuteQuery<FileViewModel>("Select * from [File] where FileId='" + fileId.ToString() + "'").FirstOrDefault();
            }
            catch
            {
                return null;
            }
            
        }
        public Guid Insert (string fileName,string filePath,byte type,Guid nguoiDungId)
        {
            try
            {
                var newFile = Guid.NewGuid();
                while (IsExist(newFile))
                {
                    newFile = Guid.NewGuid();
                }
                File file = new File()
                {
                    FileId=newFile,
                    FileName = fileName,
                    FilePath = filePath,
                    FileType = type,
                    NgayCapNhat = DateTime.Now,
                    NgayTao = DateTime.Now,
                    NguoiTao = nguoiDungId
                };
                db.Files.InsertOnSubmit(file);
                db.SubmitChanges();
                return file.FileId;
            }
            catch(Exception ex)
            {
                return Guid.Empty;
            }
        }
        public bool InsertLienKetFile(Guid FileName,string itemId,byte itemType)
        {
            try
            {
                LienKetFile lk = new LienKetFile()
                {
                    FileId = FileName,
                    ItemId = itemId,
                    ItemType = itemType
                };
                db.LienKetFiles.InsertOnSubmit(lk);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Update(Guid FileId,string fileName,string filePath)
        {
            try
            {
                var file = db.Files.Where(t => t.FileId == FileId).FirstOrDefault();
                if(!string.IsNullOrEmpty(fileName))
                file.FileName = fileName;
                if (!string.IsNullOrEmpty(filePath))
                    file.FilePath = filePath;             
                file.NgayCapNhat = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<FileViewModel> GetFileBy(string itemId,byte type,byte? FileType,Guid fileCha)
        {
            try
            {     
                return (from a in db.Files join b in db.LienKetFiles on a.FileId equals b.FileId join n in db.NguoiDungs on a.NguoiTao equals n.NguoiDungId where b.ItemId == itemId && b.ItemType == type && (FileType == null || a.FileType == FileType) &&(fileCha==Guid.Empty||a.KhoaChaId==fileCha) select new FileViewModel() { FileId = a.FileId, FileName = a.FileName, FileType = a.FileType, FilePath = a.FilePath, ItemId = b.ItemId, ItemType = b.ItemType, NgayTao = a.NgayTao, NguoiTao = a.NguoiTao, NgayCapNhat = a.NgayCapNhat,HoTen=n.HoTen }).OrderByDescending(t=>t.NgayCapNhat).ToList();
            }
            catch
            {
                return null;
            }
        }
        public bool IsExist(Guid FileId)
        {
            try
            {
                var count = db.Files.Where(t => t.FileId == FileId).Count();
                if (count > 0) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public int CountFileBy(string itemId,byte itemType)
        {
            try
            {
                return (from a in db.LienKetFiles where a.ItemId == itemId && a.ItemType == itemType select a.FileId).Count();
            }
            catch
            {
                return 0;
            }
        }
        public bool DeleteFile(Guid fileId)
        {
            //
            try
            {
                var entitys = db.LienKetFiles.Where(t => t.FileId == fileId).ToList();
                db.LienKetFiles.DeleteAllOnSubmit(entitys);
                db.SubmitChanges();
            }
            catch
            {
            }
            try
            {
                var entity = db.Files.Where(t => t.FileId == fileId).FirstOrDefault();
                db.Files.DeleteOnSubmit(entity);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
