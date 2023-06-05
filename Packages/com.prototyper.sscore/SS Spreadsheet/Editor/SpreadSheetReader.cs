using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;
// NPOI (Excel)
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace SS.Core
{
    public class SpreadSheet : IEnumerable
    {
        #region Enumerator

        private class SpreadSheetEnumerator : IEnumerator
        {
            private SpreadSheet _spreadSheet;
            private IWorkbook _workbook;
            private IEnumerator _enumerator;

            public SpreadSheetEnumerator(SpreadSheet spreadSheet, IWorkbook workbook)
            {
                _spreadSheet = spreadSheet;
                _workbook = workbook;
                _enumerator = _workbook.GetEnumerator();

            }

            public Sheet Current
            {
                get { return _spreadSheet.sheetMap[_enumerator.Current as ISheet]; }
            }

            object IEnumerator.Current
            {
                get { return _spreadSheet.sheetMap[_enumerator.Current as ISheet]; }
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SpreadSheetEnumerator(this, workbook);
        }
        #endregion

        #region Private
        private Dictionary<ISheet, Sheet> sheetMap = new Dictionary<ISheet, Sheet>();
        private Dictionary<IRow, Row> rowMap = new Dictionary<IRow, Row>();
        private Dictionary<ICell, Cell> cellMap = new Dictionary<ICell, Cell>();
        private IWorkbook workbook = null;
        #endregion

        #region Class
        public class Sheet : IEnumerable
        {
            private SpreadSheet _spreadSheet;
            private ISheet _sheet;

            public Sheet(SpreadSheet spreadSheet, ISheet sheet)
            {
                _spreadSheet = spreadSheet;
                _sheet = sheet;
            }

            public string SheetName { get { return _sheet?.SheetName; } }

            #region Enumerator

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new SheetEnumerator(_spreadSheet, _sheet);
            }

            private class SheetEnumerator : IEnumerator
            {
                private SpreadSheet _spreadSheet;
                private ISheet _sheet;
                private IEnumerator _enumerator;

                public SheetEnumerator(SpreadSheet spreadSheet, ISheet sheet)
                {
                    _spreadSheet = spreadSheet;
                    _sheet = sheet;
                    _enumerator = _sheet.GetRowEnumerator();
                }

                public object Current
                {
                    get { return _spreadSheet.rowMap[_enumerator.Current as IRow]; }
                }

                public bool MoveNext()
                {
                    return _enumerator.MoveNext();
                }

                public void Reset()
                {
                    _enumerator.Reset();
                }
            }


            #endregion
        }

        public class Row : IEnumerable<Cell>
        {
            private SpreadSheet _spreadSheet;
            private IRow _row;

            public Row(SpreadSheet spreadSheet, IRow row)
            {
                _spreadSheet = spreadSheet;
                _row = row;
            }

            public int RowNum { get { return _row.RowNum; } }

            public Cell GetCell(int cellnum)
            {
                if (_row != null)
                {
                    var c = _row.GetCell(cellnum);
                    return _spreadSheet.cellMap[c];
                }
                return null;
            }

            public void ForEach(Action<Cell> eachCell)
            {
                foreach (var c in _row)
                {
                    eachCell?.Invoke(_spreadSheet.cellMap[c]);
                }
            }

            #region Enumerator

            private class RowEnumerator : IEnumerator<Cell>
            {
                private SpreadSheet _spreadSheet;
                private IRow _row;
                private IEnumerator<ICell> _enumerator;

                public RowEnumerator(SpreadSheet spreadSheet, IRow row)
                {
                    _spreadSheet = spreadSheet;
                    _row = row;
                    _enumerator = _row.GetEnumerator();
                }

                public object Current
                {
                    get { return _spreadSheet.cellMap[_enumerator.Current]; }
                }

                Cell IEnumerator<Cell>.Current
                {
                    get { return _spreadSheet.cellMap[_enumerator.Current]; }
                }

                public void Dispose()
                {
                    _enumerator.Dispose();
                }

                public bool MoveNext()
                {
                    return _enumerator.MoveNext();
                }

                public void Reset()
                {
                    _enumerator.Reset();
                }
            }

            public IEnumerator<Cell> GetEnumerator()
            {
                return new RowEnumerator(_spreadSheet, _row);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new RowEnumerator(_spreadSheet, _row);
            }
            #endregion
        }

        public class Cell
        {
            SpreadSheet _spreadSheet;
            ICell _cell;

            public Cell(SpreadSheet spreadSheet, ICell cell)
            {
                _spreadSheet = spreadSheet;
                _cell = cell;
            }
            public int ColumnIndex { get { return (_cell != null) ? _cell.ColumnIndex : 0; } }
            public int RowIndex { get { return (_cell != null) ? _cell.RowIndex : 0; } }

            public Row Row
            {
                get {
                    var r = _cell.Row;
                    return _spreadSheet.rowMap[r];
                }
            }

            public override string ToString()
            {
                if (_cell != null)
                    return _cell.ToString();
                return String.Empty;
            }

            public string GetStringValue()
            {
                string result = "";
                if (_cell.CellType == CellType.Formula && _cell.CachedFormulaResultType == CellType.String)
                {
                    result = _cell.StringCellValue;
                }
                else
                {
                    result = _cell.ToString();
                }
                return result;
            }

            public int GetIntegerValue()
            {
                int result = 0;
                if (_cell.CellType == CellType.Numeric || (_cell.CellType == CellType.Formula && _cell.CachedFormulaResultType == CellType.Numeric))
                {
                    result = (int)_cell.NumericCellValue;
                }
                else if (int.TryParse(_cell.ToString(), out result))
                {
                }
                return result;
            }

            public bool GetBooleanValue()
            {
                if (_cell.CellType == CellType.Boolean || (_cell.CellType == CellType.Formula && _cell.CachedFormulaResultType == CellType.Boolean))
                    return _cell.BooleanCellValue;
                else if (_cell.CellType == CellType.String || (_cell.CellType == CellType.Formula && _cell.CachedFormulaResultType == CellType.String))
                {
                    if (_cell.StringCellValue.IndexOf("true", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        return true;
                    else if (_cell.StringCellValue.IndexOf("false", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        return false;
                    else
                        return false;
                }
                else
                    return GetIntegerValue() != 0;
            }

            public float GetFloatValue()
            {
                float result = 0;
                if (_cell.CellType == CellType.Numeric || (_cell.CellType == CellType.Formula && _cell.CachedFormulaResultType == CellType.Numeric))
                {
                    result = (float)_cell.NumericCellValue;
                }
                else if (float.TryParse(_cell.ToString(), out result))
                {
                }
                return result;
            }

            public double GetDoubleValue()
            {
                double result = 0;
                if (_cell.CellType == CellType.Numeric || (_cell.CellType == CellType.Formula && _cell.CachedFormulaResultType == CellType.Numeric))
                {
                    result = _cell.NumericCellValue;
                }
                else
                {
                    double.TryParse(_cell.ToString(), out result);
                }
                return result;
            }

            static T Get<T>(ICell cell) where T : IConvertible
            {
                T result = default(T);
                if (cell != null)
                {
                    try
                    {
                        var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                        if (converter != null)
                        {
                            if (cell.CellType == CellType.Numeric || (cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Numeric))
                            {
                                result = (T)Convert.ChangeType(cell.NumericCellValue, typeof(T));
                            }
                            else if (cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.String)
                            {
                                result = (T)converter.ConvertFromString(cell.StringCellValue);
                            }
                            else
                            {
                                result = (T)converter.ConvertFromString(cell.ToString());
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                return result;
            }

            public T GetValue<T>() where T : IConvertible
            {
                return Get<T>(_cell);
            }
        }
        #endregion

        public void Load(string fullPath)
        {
            // Ignore temporary files
            if (fullPath.Contains("~$"))
                return;

            Clear();

            using (Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    if (fullPath.EndsWith(".xls", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        workbook = new HSSFWorkbook(stream);
                    }
                    else if (fullPath.EndsWith(".xlsx", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        workbook = new XSSFWorkbook(stream);
                    }
                    else if (fullPath.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // TODO
                    }
                    else
                    {

                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (workbook != null)
                {
                    // Parse all sheets, rows, cells
                    IEnumerator sheetEnum = workbook.GetEnumerator();
                    while (sheetEnum.MoveNext())
                    {
                        var sheet = sheetEnum.Current as ISheet;
                        var currSheet = new Sheet(this, sheet);
                        sheetMap.Add(sheet, currSheet);

                        // for each row
                        IEnumerator rowEnum = sheet.GetRowEnumerator();
                        while (rowEnum.MoveNext())
                        {
                            var row = rowEnum.Current as IRow;
                            var currRow = new Row(this, row);
                            rowMap.Add(row, currRow);

                            if (row != null)
                            {
                                // for each cell
                                IEnumerator cellEnum = row.GetEnumerator();
                                while (cellEnum.MoveNext())
                                {
                                    var cell = cellEnum.Current as ICell;
                                    var currCell = new Cell(this, cell);
                                    cellMap.Add(cell, currCell);
                                }
                            }
                        }
                    }
                }
                // Close FileStream
                stream.Close();
            }
        }

        public void ForEach(Action<Sheet> eachSheet = null, Action<Row> eachRow = null, Action<Cell> eachCell = null)
        {
            if (workbook != null)
            {
                // for each sheet
                IEnumerator sheetEnum = workbook.GetEnumerator();
                while (sheetEnum.MoveNext())
                {
                    var sheet = sheetEnum.Current as ISheet;
                    var currSheet = new Sheet(this, sheet);

                    //Debug.Log(sheet.SheetName);
                    eachSheet?.Invoke(currSheet);
    
                    // for each row
                    IEnumerator rowEnum = sheet.GetRowEnumerator();
                    while (rowEnum.MoveNext())
                    {
                        var row = rowEnum.Current as IRow;
                        var currRow = new Row(this, row);

                        eachRow?.Invoke(currRow);

                        if (row != null)
                        {
                            // for each cell
                            IEnumerator cellEnum = row.GetEnumerator();
                            while (cellEnum.MoveNext())
                            {
                                var cell = cellEnum.Current as ICell;
                                var currCell = new Cell(this, cell);

                                if (cell != null)
                                {
                                    //Debug.LogFormat("{0},{1}={2}.",cell.RowIndex, cell.ColumnIndex, cell.ToString());
                                    eachCell?.Invoke(currCell);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            sheetMap.Clear();
            rowMap.Clear();
            cellMap.Clear();
        }

    }
}