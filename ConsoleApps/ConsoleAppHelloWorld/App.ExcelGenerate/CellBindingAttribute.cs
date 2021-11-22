using System;

namespace ConsoleAppHelloWorld.App.ExcelGenerate
{
    public enum CellWriteMode
    {
        Write,
        Append,
        Param,
        Checkbox
    }

    public sealed class CellBindingAttribute : Attribute
    {
        public readonly string Cell;
        public readonly CellWriteMode Mode = CellWriteMode.Write;
        public readonly string ParamTemplate = "{param1}";

        public CellBindingAttribute(string cell) => Cell = cell;
        public CellBindingAttribute(string cell, CellWriteMode mode) : this(cell) => Mode = mode;
        public CellBindingAttribute(string cell, CellWriteMode mode, string paramTemplate) : this(cell, mode) => ParamTemplate = paramTemplate;
    }
}
