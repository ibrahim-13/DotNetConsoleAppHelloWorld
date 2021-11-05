using System;
using System.IO;
using OfficeOpenXml;
using System.Linq;

namespace ConsoleAppHelloWorld.App.ExcelGenerate
{
    public class AppMain
    {
        public static void Run()
        {
            using var writer = new ServiceLogWriter();
            writer.Write(new ServiceLogData
            {
                WriteWholeCell = "Value1",
                AppendCell = "Value2",
                ParamCell = "Value3"
            });
            writer.SaveToFile("TestFile.xlsx");
        }
    }

    public enum CellWriteMode
    {
        Write,
        Append,
        Param
    }

    public class CellBindingAttribute : Attribute
    {
        public readonly string Cell;
        public readonly CellWriteMode Mode = CellWriteMode.Write;
        public readonly string ParamTemplate = "{param1}";

        public CellBindingAttribute(string cell) => Cell = cell;
        public CellBindingAttribute(string cell, CellWriteMode mode) : this(cell) => Mode = mode;
        public CellBindingAttribute(string cell, CellWriteMode mode, string paramTemplate) : this(cell, mode) => ParamTemplate = paramTemplate;
    }

    public sealed class ServiceLogData
    {
        [CellBinding("A4")]
        public string? WriteWholeCell { get; set; }
        [CellBinding("B4", CellWriteMode.Append)]
        public string? AppendCell { get; set; }
        [CellBinding("C4", CellWriteMode.Param)]
        public string? ParamCell { get; set; }
    }

    public class ServiceLogWriter : IDisposable
    {
        private readonly MemoryStream? _sampleExcelStream;
        private ExcelPackage? _package;

        static ServiceLogWriter() => ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        public ServiceLogWriter()
        {
            var fileStream = Properties.ExcelSample.XLSX;
            if (fileStream is null) return;
            _sampleExcelStream = new MemoryStream(fileStream);
            _package = new ExcelPackage(_sampleExcelStream);
        }

        public void Dispose()
        {
            _package?.Dispose();
            _sampleExcelStream?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Write(ServiceLogData data)
        {
            var sheet = _package?.Workbook.Worksheets[0];
            if (sheet is null)
                return;

            WriteNonCalculativeData(data, sheet);
        }

        public byte[]? GetBytes(bool reopen = false)
        {
            var xlsxBytes = _package?.GetAsByteArray();
            _package = reopen && _sampleExcelStream is not null ? new ExcelPackage(_sampleExcelStream) : null;
            return xlsxBytes;
        }

        public void SaveToFile(string fileName)
        {
            var newFile = new FileInfo(fileName);
            _package?.SaveAs(newFile);
        }

        private static void WriteNonCalculativeData(ServiceLogData data, ExcelWorksheet sheet)
        {
            var type = data.GetType();
            type.GetProperties()
                .Where(field => field.PropertyType == typeof(string))
                .ToList()
                .ForEach(field =>
                {
                    var attributes = Attribute.GetCustomAttributes(field);
                    attributes
                        .OfType<CellBindingAttribute>()
                        .ToList()
                        .ForEach(cellBinding =>
                        {
                            sheet.Cells[cellBinding.Cell].Value = cellBinding.Mode switch
                            {
                                CellWriteMode.Write => (field.GetValue(data) as string),
                                CellWriteMode.Append => sheet.Cells[cellBinding.Cell].Value + " " + (field.GetValue(data) as string),
                                CellWriteMode.Param => sheet.Cells[cellBinding.Cell].Value?
                                    .ToString()?
                                    .Replace(cellBinding.ParamTemplate, (field.GetValue(data) as string)),
                                _ => sheet.Cells[cellBinding.Cell]
                            };
                        });
                });
        }
    }
}
