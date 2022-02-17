using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;


namespace MetaWork.WorkTime.Models
{
    public class ExcelOpenXml : IDisposable
    {
        private string _templatePath;
        private string _fileName2Response;

        private string _fileType2Response;
        public string FileType2Response
        {
            get { return _fileType2Response; }
        }

        private string _tempFilePath2Respone;
        public string FilePath2Respone
        {
            get { return _tempFilePath2Respone; }
        }


        private string _tempFolderPath;

        private ExcelPackage _xlPackage;
        private ExcelWorksheet _xlWorksheet;
        private System.Web.HttpContext _httpContext;




        private bool _disposed = false;
        // Instantiate a SafeHandle instance.
        private System.Runtime.InteropServices.SafeHandle _handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);


        public ExcelOpenXml(System.Web.HttpContext httpContext, string templatePath, string sheetName, string tempFolderPath)
        {
            initialize(httpContext, templatePath, tempFolderPath, string.Empty);
            _xlWorksheet = _xlPackage.Workbook.Worksheets[sheetName];
        }

        public ExcelOpenXml(System.Web.HttpContext httpContext, string templatePath, int sheetNumber, string tempFolderPath)
        {
            initialize(httpContext, templatePath, tempFolderPath, string.Empty);
            _xlWorksheet = _xlPackage.Workbook.Worksheets[sheetNumber];
        }

        public ExcelOpenXml(System.Web.HttpContext httpContext, string templatePath, string sheetName, string tempFolderPath, string tempFileName)
        {
            initialize(httpContext, templatePath, tempFolderPath, tempFileName);
            _xlWorksheet = _xlPackage.Workbook.Worksheets[sheetName];
        }

        /// <summary>
        /// Hàm Constructor để read file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="sheetName">Name of the sheet. If empty get first.</param>
        public ExcelOpenXml(string filePath, string sheetName)
        {

            FileInfo file2Read = new FileInfo(filePath);
            if (!file2Read.Exists) throw new Exception("File 2 read does not exist!" + filePath);

            _xlPackage = new ExcelPackage(file2Read);
            _httpContext = null;

            _xlWorksheet = string.IsNullOrEmpty(sheetName) ? _xlPackage.Workbook.Worksheets.First() : _xlPackage.Workbook.Worksheets[sheetName];
        }



        private void initialize(System.Web.HttpContext httpContext, string templatePath, string tempFolderPath, string tempFileName)
        {
            //if (!Main.CheckDLL) return;
            _templatePath = templatePath;
            FileInfo templateFile = new FileInfo(_templatePath);
            if (!templateFile.Exists) throw new Exception("Template file does not exist!" + _templatePath);

            _fileName2Response = _templatePath.Substring(_templatePath.LastIndexOf('\\') + 1);
            _fileType2Response = _fileName2Response.Substring(_fileName2Response.LastIndexOf('.'));

            _tempFolderPath = tempFolderPath;


            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
            if (string.IsNullOrEmpty(tempFileName)) tempFileName = Guid.NewGuid().ToString("N") + _fileType2Response;
            _tempFilePath2Respone = _tempFolderPath + "\\" + tempFileName;
            FileInfo tempFile2Respone = new FileInfo(_tempFilePath2Respone);
            _xlPackage = new ExcelPackage(tempFile2Respone, templateFile);
            _httpContext = httpContext;
        }


        ~ExcelOpenXml()
        {
            Dispose();
        }

        public void CopynChangeCurrentWorkSheet(string nameOfExistWorkSheet, string newName)
        {
            _xlWorksheet = _xlPackage.Workbook.Worksheets.Copy(nameOfExistWorkSheet, newName);
        }

        public void AddDataTablev1(int startCol, int startRow, DataTable data)
        {
            int idataRowCount = data.Rows.Count;
            int idataColumnCount = data.Columns.Count;

            for (int iRow = 0; iRow < idataRowCount; iRow++)
            {
                for (int iColumn = 0; iColumn < idataColumnCount; iColumn++)
                {
                    _xlWorksheet.SetValue(startRow + iRow, startCol + iColumn, data.Rows[iRow][iColumn]);
                }
            }
        }

        public void AddDataTable(int startCol, int startRow, DataTable data)
        {
            int idataRowCount = data.Rows.Count;
            int idataColumnCount = data.Columns.Count;

            _xlWorksheet.InsertRow(startRow + 1, idataRowCount - 2, startRow);

            for (int iRow = 0; iRow < idataRowCount; iRow++)
            {
                for (int iColumn = 0; iColumn < idataColumnCount; iColumn++)
                {
                    _xlWorksheet.SetValue(startRow + iRow, startCol + iColumn, data.Rows[iRow][iColumn]);
                }
            }
        }

        public void AddItemToSpreadsheet(int row, int col, string value)
        {
            _xlWorksheet.SetValue(row, col, value);
        }
        public void AddItemToSpreadsheet(int row, int col, object value)
        {
            _xlWorksheet.SetValue(row, col, value);

        }
        public void RemoveValue(int row, int col)
        {
            //_xlWorksheet.Cell(row, col).RemoveValue();
            _xlWorksheet.SetValue(row, col, null);
        }

        public void SetWrapText(int row, int col, bool bWrapText)
        {
            _xlWorksheet.Cells[row, col].Style.WrapText = bWrapText;  //false by default
        }

        public void CopyRange(int srcFromRow, int srcFromColumn, int srcToRow, int srcToColumn, int desFromRowNumber, int desToRowNumber)
        {
            _xlWorksheet.Cells[srcFromRow, srcFromColumn, srcToRow, srcToColumn].Copy(_xlWorksheet.Cells[desFromRowNumber, srcFromColumn, desToRowNumber, srcToColumn]);
            _xlPackage.Save();
        }
        /// <summary>
        /// Copy cell 
        /// </summary>
        /// <param name="srcFromRow"></param>
        /// <param name="srcFromColumn"></param>
        /// <param name="desFromRowNumber"></param>
        /// <param name="desToRowNumber"></param>
        /// Create by kienntgeoit 2018.09.07
        public void Copy(int srcFromRow, int srcFromColumn, int desFromRowNumber, int desToRowNumber)
        {
            //Console.WriteLine($"({srcFromRow},{srcFromColumn}) --> ({desFromRowNumber},{desToRowNumber})");
            _xlWorksheet.Cells[srcFromRow, srcFromColumn].Copy(_xlWorksheet.Cells[desFromRowNumber, desToRowNumber]);
        }
        public void Insert(int fromRow, int numberRows)
        {
            _xlWorksheet.InsertRow(fromRow, numberRows);
        }
        public void RemoveRow(int fromRow, int numberRows)
        {
            _xlWorksheet.DeleteRow(fromRow, numberRows, true);
        }
        public void InsertImage(int row, int col, string path)
        {
            FileInfo file = new FileInfo(path);
            var picture = _xlWorksheet.Drawings.AddPicture("3499b896-9cad-4bc2-be56-426d59850b5a.jpg", file);
            _xlWorksheet.Row(7).Height = picture.Image.Height;
            picture.SetPosition(row, 0, col, 0);
        }
        public void SaveAndClose()
        {
            _xlPackage.Save();
            _xlPackage.Dispose();

            _xlWorksheet = null;
            _xlPackage = null;
        }

        /// <summary>
        /// Chuyển file xuống client
        /// </summary>
        /// <param name="fileName2Response">mặc định có xlsx</param>
        public void ResponeAndReleaseTemp(string fileName2Response = "")
        {
            //_httpContext.Response.Buffer = false;
            if (_fileType2Response == ".xls") _httpContext.Response.ContentType = "application/vnd.ms-excel";
            else _httpContext.Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            if (string.IsNullOrEmpty(fileName2Response)) _httpContext.Response.AppendHeader("content-disposition", "attachment; filename=" + _fileName2Response);
            else _httpContext.Response.AppendHeader("content-disposition", "attachment; filename=" + fileName2Response);


            FileInfo tempFile = new FileInfo(_tempFilePath2Respone);
            int len = (int)tempFile.Length, bytes;
            _httpContext.Response.AppendHeader("content-length", len.ToString());
            byte[] buffer = new byte[1024];
            Stream outStream = _httpContext.Response.OutputStream;
            using (Stream stream = File.OpenRead(tempFile.FullName))
            {
                while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    len -= bytes;
                }
            }

            tempFile.Delete();
            tempFile = null;
        }
        public HttpResponseMessage ResponeApiAndReleaseTemp(string fileName2Response = "")
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            if (File.Exists(_tempFilePath2Respone))
            {
                var stream = new FileStream(_tempFilePath2Respone, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                if (string.IsNullOrEmpty(fileName2Response)) result.Content.Headers.ContentDisposition.FileName = _fileName2Response;
                else result.Content.Headers.ContentDisposition.FileName = fileName2Response;
                //if (_fileType2Response == ".xls") result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                //else result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentLength = stream.Length;

            }
            return result;
        }


        /// <summary>
        /// Đọc dữ liệu từ file excel
        /// </summary>
        /// <param name="startRow">Dòng bắt đầu dữ liệu trong sheet</param>
        /// <param name="startRowIsHeader">Dòng bắt đầu là dữ liệu luôn hay là dòng tiêu đề</param>
        /// <returns>Dữ liệu</returns>
        /// ref https://codealoc.wordpress.com/2012/04/19/reading-an-excel-xlsx-file-from-c/
        public DataTable ReadDataTable(int startRow, bool startRowIsHeader)
        {
            if (_xlWorksheet.Dimension == null) return null;
            int iStartRow = _xlWorksheet.Dimension.Start.Row;
            iStartRow = startRow > iStartRow ? startRow : iStartRow;
            int iEndRow = _xlWorksheet.Dimension.End.Row;

            int iStartCol = _xlWorksheet.Dimension.Start.Column;
            int iEndCol = _xlWorksheet.Dimension.End.Column;

            int iCurrentRow = startRowIsHeader ? iStartRow + 1 : iStartRow;

            DataTable result = new DataTable();
            for (int j = iStartCol; j <= iEndCol; j++)
            {
                string columnName = "Column" + j.ToString();
                if (startRowIsHeader) columnName = _xlWorksheet.Cells[iStartRow, j].Value.ToString();
                result.Columns.Add(new DataColumn(columnName));
            }

            for (int i = iCurrentRow; i <= iEndRow; i++)
            {
                DataRow drCurrent = result.NewRow();
                for (int j = iStartCol; j <= iEndCol; j++)
                {
                    object cellValue = _xlWorksheet.Cells[i, j].Value;
                    if (cellValue != null)
                        drCurrent[j - 1] = cellValue.ToString();
                }
                result.Rows.Add(drCurrent);
            }
            return result;
        }

        public virtual void Dispose()
        {
            if (_disposed)
                return;

            _handle.Dispose();
            // Free any other managed objects here.
            _xlWorksheet = null;
            _xlPackage = null;


            // Free any unmanaged objects here.
            _disposed = true;
        }

    }
}