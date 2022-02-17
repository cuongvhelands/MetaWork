using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MetaWork.WorkTime.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetaWork.WorkTime.Controllers
{
    [RoutePrefix("api/DownUploadApi")]
  
    public class DownUploadApiController : ApiController
    {
        



        /// <summary>
        /// 
        /// </summary>
        /// <returns> Tên file </returns>
        /// Createbu;
        /// 
        [HttpPost]
        [Route("Upload2Temp")]
        public async Task<HttpResponseMessage> Upload2Temp()
        {
            if (!Request.Content.IsMimeMultipartContent()) // Check if the request contains multipart/form-data.
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/Uploads");
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (MultipartFileData file in provider.FileData)
                {
                    string filename = "dataExcel" + Path.GetExtension(file.Headers.ContentDisposition.FileName.Replace("\"", ""));
                    var time = DateTime.Now;                    
                    string newFileName = String.Format("{0}\\{1}", root, filename);
                    if (File.Exists(newFileName))
                    {
                        File.Delete(newFileName);
                    }
                    File.Move(file.LocalFileName, newFileName);
                    ThoiGianLamViecModel model = new ThoiGianLamViecModel();
                    model.AddTimeToDb(newFileName);
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(filename),
                        StatusCode = HttpStatusCode.OK
                    };
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error processing upload");
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }     

        private static Bitmap ResizeFitWidth(Image image, int width)
        {
            if (width > image.Width)
            {
                return new Bitmap(image);
            }
            else
            {
                float ratio = (float)image.Height / image.Width;
                int height = (int)(width * ratio);
                return ResizeImage(image, width, height);
            }
        }

        private static Bitmap ResizeFitHeight(Image image, int height)
        {
            if (height > image.Height)
            {
                return new Bitmap(image);
            }
            else
            {
                float ratio = (float)image.Height / image.Width;
                int width = (int)(height / ratio);
                return ResizeImage(image, width, height);
            }
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        [AllowAnonymous]
        [Route("GetImageTempAsset/{filePath}/{fileName}/{ext}")]
        public HttpResponseMessage GetImageTempAsset(string filePath, string fileName, string ext)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var absolutePath = HttpContext.Current.Server.MapPath("~/Uploads");
            var col = filePath.Split('_');
            foreach (var item in col)
            {
                absolutePath += "\\" + item;
            }
            absolutePath += "\\" + fileName + "." + ext;
            if (File.Exists(absolutePath))
            {

                var stream = new FileStream(absolutePath, FileMode.Open);
                result.Content = new StreamContent(stream);
                makeContentHeader4Download(fileName, ext, stream.Length, ref result);
            }
            return result;
        }

      

        #region PrivateMethod
        private void makeContentHeader4Download(string fileName, string ext, long fileSize, ref HttpResponseMessage result)
        {
            string extUpper = ext.ToUpper();
            string[] arrInlineTypes = new string[] { "PNG", "GIF", "JPG", "JPEG", "PDF", };
            if (arrInlineTypes.Contains(extUpper)) result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            else result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = $"{fileName}.{ext}";
            if (extUpper == "PDF") result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            else if (extUpper == "PNG") result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            else result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = fileSize;
        }
        private string getDisplayFileSize(long fileSize)
        {
            string str = "";
            if (fileSize > 1000000) return Math.Round((double)fileSize / 1000000, 2) + " MB";
            else if (fileSize < 1000000 && fileSize > 1000) return fileSize / 1000 + "KB";
            else return fileSize + " B";
        }
        string GetFileIconByFileExt(string fileExt)
        {
            fileExt = fileExt.ToUpper();
            if (fileExt == "PDF")
            {
                return "<i class=\"far fa-file-pdf\"></i>";
            }
            else if (fileExt == "XLS" || fileExt == "XLSX")
            {
                return "<i class=\"far fa-file-excel\"></i>";
            }

            else if (fileExt == "DOC" || fileExt == "DOCX") return "<i class=\"far fa-file-word\"></i>";
            else if (fileExt == "PNG" || fileExt == "GIF" || fileExt == "JPG" || fileExt == "JPEG") return "<i class=\"far fa-file-image\"></i>";
            else return "<i class=\"far fa-file\"></i>";
        }
      
        #endregion
    }
}