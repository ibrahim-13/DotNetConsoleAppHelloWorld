using System;

namespace ConsoleAppHelloWorld.App.ExcelGenerate
{
    public class AppMain
    {
        public static void Run()
        {
            using var writer = new ExcelWriter(Properties.ExcelSample.XLSX);
            writer.Write(new TestData
            {
                WriteWholeCell = "Value1",
                AppendCell = "Value2",
                ParamCell = "Value3",
                ListOfBool = new[,]
                {
                    { true, false },
                    { false, true }
                }
            });
            writer.SaveToFile("TestFile.xlsx");
        }
    }

    public sealed class TestData
    {
        [CellBinding("A4")]
        public string? WriteWholeCell { get; set; }
        [CellBinding("B4", CellWriteMode.Append)]
        public string? AppendCell { get; set; }
        [CellBinding("C4", CellWriteMode.Param)]
        public string? ParamCell { get; set; }
        [CellBinding("E1:F4", CellWriteMode.Checkbox)]
        public bool[,]? ListOfBool { get; set; }
    }
}
