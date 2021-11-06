using System;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace ConsoleAppHelloWorld.App.ExcelGenerate
{
    public class ExcelWriterConfig
    {
        public int ImageHeight = 13;
        public int ImageWidth = 13;
        public int OffsetTop = 2;
        public int OffsetLeft = 15;
    }

    public class ExcelWriter : IDisposable
    {
        private readonly MemoryStream _sampleExcelStream;
        private readonly ExcelWriterConfig _config = new();
        protected ExcelPackage Package;

        static ExcelWriter() => ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        public ExcelWriter(byte[]? fileFromResourceBytes)
        {
            if (fileFromResourceBytes is null) return;
            _sampleExcelStream = new MemoryStream(fileFromResourceBytes);
            Package = new ExcelPackage(_sampleExcelStream);
        }

        public ExcelWriter(byte[]? fileFromResourceBytes, ExcelWriterConfig config) : this(fileFromResourceBytes)
        {
            _config = config;
        }

        public void Dispose()
        {
            Package?.Dispose();
            _sampleExcelStream?.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual void Write(object data)
        {
            var sheet = Package?.Workbook.Worksheets[0];
            if (sheet is null)
                return;

            WriteNonCalculativeData(data, sheet);
        }

        public byte[] GetBytes(bool reopen = false)
        {
            var xlsxBytes = Package?.GetAsByteArray();
            Package = reopen && _sampleExcelStream != null ? new ExcelPackage(_sampleExcelStream) : null;
            return xlsxBytes;
        }

        public void SaveToFile(string filePathWithName)
        {
            var file = new FileInfo(filePathWithName);
            Package?.SaveAs(file);
        }

        protected void WriteNonCalculativeData(object data, ExcelWorksheet sheet)
        {
            if (data is null)
                return;
            var type = data.GetType();

            foreach (var field in type.GetProperties())
            {
                var attributes = Attribute.GetCustomAttributes(field);
                var isStringValueType = field.PropertyType == typeof(string);
                var isBool2dArray = field.PropertyType == typeof(bool[,]);
                attributes
                    .OfType<CellBindingAttribute>()
                    .ToList()
                    .ForEach(cellBinding =>
                    {
                        if (isStringValueType)
                        {
                            var fieldValue = (field.GetValue(data) as string);
                            sheet.Cells[cellBinding.Cell].Value = cellBinding.Mode switch
                            {
                                CellWriteMode.Write => fieldValue,
                                CellWriteMode.Append => sheet.Cells[cellBinding.Cell].Value + " " + fieldValue,
                                CellWriteMode.Param => sheet.Cells[cellBinding.Cell].Value?
                                    .ToString()?
                                    .Replace(cellBinding.ParamTemplate, fieldValue),
                                _ => sheet.Cells[cellBinding.Cell].Value
                            };
                        }
                        else if (isBool2dArray)
                        {
                            var fieldValue = (field.GetValue(data) as bool[,]);
                            sheet.Cells[cellBinding.Cell].Value = cellBinding.Mode switch
                            {
                                CellWriteMode.Checkbox => WriteImageCheckboxOnRange(cellBinding.Cell, sheet, in fieldValue),
                                _ => sheet.Cells[cellBinding.Cell].Value
                            };
                        }
                    });
            }
        }

        private string WriteImageCheckboxOnRange(string range, ExcelWorksheet sheet, in bool[,] values)
        {
            sheet.Cells[range].Value = string.Empty;
            var excelRange = sheet.Cells[range];
            var firstCell = excelRange.First();
            foreach (var cell in sheet.Cells[range])
            {
                var (row, col) = (cell.Start.Row - 1, cell.Start.Column - 1);
                var (indexRow, indexCol) = (cell.Start.Row - firstCell.Start.Row,
                    cell.Start.Column - firstCell.Start.Column);
                var imgName = $"img{row}{col}";
                var imgByte = values.GetImageBasedOnState(indexRow, indexCol);
                var img = sheet.Drawings.AddPicture(imgName, imgByte);
                img.SetSize(_config.ImageWidth, _config.ImageHeight);
                img.SetPosition(row, _config.OffsetTop, col, _config.OffsetLeft);
            }
            return string.Empty;
        }
    }

    static class Extension
    {
        public static Bitmap GetImageBasedOnState(this bool[,] values, int indexRow, int indexCol)
        {
            try
            {
                var value = values[indexRow, indexCol];
                if (value == true)
                    return Properties.Image.CheckboxChecked;
            }
            catch (Exception)
            {}

            return Properties.Image.CheckboxUnchecked;
        }
    }
}
