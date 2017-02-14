using Aliyun.OTS.DataModel;
using Aliyun.OTS.Response;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace OtsTool
{
    public class ExcelService
    {
        public static void Append(string fileName, string sheetName, IList<RowDataFromGetRange> data,
            IDictionary<string, ColumnType> columnNames)
        {
            HSSFWorkbook workbook;
            if (File.Exists(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    workbook = new HSSFWorkbook(fs);
                }
            }
            else
            {
                workbook = new HSSFWorkbook();
            }
            var sheet = workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);
            var rowIndex = GetLastRowNum(sheet);
            if (rowIndex == 0)
            {
                sheet.CreateRow(rowIndex++);
            }
            foreach (var rowData in data)
            {
                var row = sheet.CreateRow(rowIndex);
                foreach (var columnData in rowData.PrimaryKey)
                {
                    var columnIndex = GetColumnIndex(workbook, sheet, columnData.Key);
                    SetCellValue(row, columnData, columnIndex, columnNames);
                }
                foreach (var columnData in rowData.Attribute)
                {
                    var columnIndex = GetColumnIndex(workbook, sheet, columnData.Key);
                    SetCellValue(row, columnData, columnIndex, columnNames);
                }
                rowIndex++;
            }
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }
        }

        private static void SetCellValue(IRow row, KeyValuePair<string, ColumnValue> columnData,
            int columnIndex, IDictionary<string, ColumnType> columnNames)
        {
            var columnType = columnNames != null && columnNames.ContainsKey(columnData.Key) ?
                columnNames[columnData.Key] : ColumnType.Auto;
            switch (columnData.Value.Type)
            {
                case ColumnValueType.Integer:
                    if (columnType == ColumnType.DateTime)
                    {
                        row.CreateCell(columnIndex, CellType.Numeric).SetCellValue(new DateTime(columnData.Value.IntegerValue));
                    }
                    else
                    {
                        row.CreateCell(columnIndex, CellType.Numeric).SetCellValue(columnData.Value.IntegerValue);
                    }
                    break;
                case ColumnValueType.Boolean:
                    row.CreateCell(columnIndex, CellType.Boolean).SetCellValue(columnData.Value.BooleanValue);
                    break;
                case ColumnValueType.Double:
                    row.CreateCell(columnIndex, CellType.Numeric).SetCellValue(columnData.Value.DoubleValue);
                    break;
                case ColumnValueType.Binary:
                    row.CreateCell(columnIndex, CellType.String).SetCellValue(columnData.Value.BinaryValue?.ToString() ?? string.Empty);
                    break;
                case ColumnValueType.String:
                default:
                    row.CreateCell(columnIndex, CellType.String).SetCellValue(columnData.Value.StringValue);
                    break;
            }
        }

        private static int GetLastRowNum(ISheet sheet)
        {
            return sheet.LastRowNum;
        }

        private static int GetColumnIndex(HSSFWorkbook workbook, ISheet sheet, string key)
        {
            var header = sheet.GetRow(0);
            var i = 0;
            ICell cell;
            while (true)
            {
                cell = header.GetCell(i);
                if (cell == null || cell.StringCellValue == key)
                {
                    break;
                }
                else
                {
                    i++;
                }
            }
            if (cell == null)
            {
                header.CreateCell(i, CellType.String).SetCellValue(key);
            }
            return i;
        }
    }
}
