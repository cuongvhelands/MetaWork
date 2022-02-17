using MetaWork.Data.Provider;
using MetaWork.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaWork.WorkTime.Models
{
    public class FileModel
    {
        FileProvider _manager;
        public FileModel()
        {
            _manager = new FileProvider();
        }
        public Guid InsertFileLocal(string fileName,string filePath,byte fileType,Guid nguoiDungId,string itemId,byte itemType)
        {
            var newFileId= _manager.Insert(fileName, filePath, fileType, nguoiDungId);
            if (newFileId != Guid.Empty)
            {
                _manager.InsertLienKetFile(newFileId, itemId, itemType);
            }
            return newFileId;
        }
        
        public FileViewModel GetById(Guid fileId)
        {
            return _manager.GetById(fileId);
        }
        public bool UpdateFile(string fileName,string filePath,Guid fileId)
        {
            return _manager.Update(fileId, fileName, filePath);
        }
        
    }
}