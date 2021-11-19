using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var xlsxBytes = Package?.GetAsByteArray() ?? Array.Empty<byte>();
            Package = reopen ? new ExcelPackage(_sampleExcelStream) : null;
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

            var members = new List<MemberInfo>();
            members.AddRange(type.GetProperties());
            members.AddRange(type.GetFields());

            foreach (var memberInfo in members)
            {
                var attributes = Attribute.GetCustomAttributes(memberInfo);
                var memberType = memberInfo switch
                {
                    FieldInfo fieldInfo => fieldInfo.FieldType,
                    PropertyInfo propertyInfo => propertyInfo.PropertyType,
                    _ => null
                };
                var isBool2dArray = memberType == typeof(bool[,]);
                var isStringArray = memberType == typeof(string[]);
                attributes
                    .OfType<CellBindingAttribute>()
                    .ToList()
                    .ForEach(cellBinding =>
                    {
                        if (isBool2dArray)
                        {
                            var memberValue = memberInfo switch
                            {
                                FieldInfo info => (info.GetValue(data) as bool[,]),
                                PropertyInfo pInfo => (pInfo.GetValue(data) as bool[,]),
                                _ => null
                            } ?? new bool[,] { };
                            WriteImageCheckboxOnRange(cellBinding.Cell, sheet, in memberValue);
                        }
                        else if (isStringArray)
                        {
                            var memberValue = memberInfo switch
                            {
                                FieldInfo info => info.GetValue(data) as string[],
                                PropertyInfo pInfo => pInfo.GetValue(data) as string[],
                                _ => null
                            } ?? Array.Empty<string>();
                            WriteStringArrayOnRange(cellBinding.Cell, sheet, in memberValue);
                        }
                        else
                        {
                            var memberValue = memberInfo switch
                            {
                                FieldInfo info => info.GetValue(data)?.ToString() ?? string.Empty,
                                PropertyInfo pInfo => pInfo.GetValue(data)?.ToString() ?? string.Empty,
                                _ => null
                            };
                            sheet.Cells[cellBinding.Cell].Value = cellBinding.Mode switch
                            {
                                CellWriteMode.Write => memberValue,
                                CellWriteMode.Append => sheet.Cells[cellBinding.Cell].Value + " " + memberValue,
                                CellWriteMode.Param => sheet.Cells[cellBinding.Cell].Value?
                                    .ToString()?
                                    .Replace(cellBinding.ParamTemplate, memberValue),
                                _ => sheet.Cells[cellBinding.Cell].Value
                            };
                        }
                    });
            }
        }

        private void WriteImageCheckboxOnRange(string range, ExcelWorksheet sheet, in bool[,] values)
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
                cell.Value = string.Empty;
            }
        }

        private void WriteStringArrayOnRange(string range, ExcelWorksheet sheet, in string[] values)
        {
            sheet.Cells[range].Value = string.Empty;
            var excelRange = sheet.Cells[range];
            var firstCell = excelRange.First();
            foreach (var cell in sheet.Cells[range])
            {
                var indexCol = cell.Start.Column - firstCell.Start.Column;
                if (indexCol >= values.Length)
                    break;
                cell.Value = values[indexCol];
            }
        }
    }

    static class Extension
    {
        public static Bitmap GetImageBasedOnState(this bool[,] values, int indexRow, int indexCol)
        {
            try
            {
                var value = values[indexRow, indexCol];
                if (value)
                    return AppResources.CheckboxChecked;
            }
            catch (Exception)
            {
                // ignored
            }

            return AppResources.CheckboxUnchecked;
        }
    }
}
