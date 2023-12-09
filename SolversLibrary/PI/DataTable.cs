using System;
using System.Collections.Generic;
using System.Linq;

namespace SolversLibrary.PI
{
    public class DataTable : IDataTable
    {
        public DataTable()
        {
            _columns = new Dictionary<string, double[]>();
            _numberRows = 0;
        }
        public DataTable(IEnumerable<string> columns, int numRows = 0)
        {
            _columns = new Dictionary<string, double[]>();
            foreach (var c in columns)
                _columns.Add(c, new double[numRows]);
            _numberRows = numRows;
        }
        public DataTable(IDictionary<string, double[]> data)
        {
            _numberRows = data.Select(kvp => kvp.Value.Length).Distinct().Single(); // distinct and single to ensure each column has equal amount of rows
            _columns = new Dictionary<string, double[]>(data);
        }
        private Dictionary<string, double[]> _columns;
        public double[] this[string name]
        {
            get
            {
                return GetColumn(name);
            }
        }

        public int NumberOfColumns
        {
            get
            {
                return _columns.Count;
            }
        }

        private int _numberRows;
        public int NumberOfRows
        {
            get
            {
                return _numberRows;
            }
        }

        public IEnumerable<string> ColumnNames
        {
            get
            {
                return _columns.Keys;
            }
        }

        public string ColumnAtIndex(int index)
        {
            throw new NotImplementedException();
        }

        public int ColumnIndex(string name)
        {
            throw new NotImplementedException();
        }

        public double[] GetColumn(string name)
        {
            return _columns[name];
        }

        public IDictionary<string, double> GetRow(int index)
        {
            var result = new Dictionary<string, double>();
            foreach (var c in _columns.Keys)
                result[c] = _columns[c][index];
            return result;
        }

        public void SetData(string[] columnNames, double[,] values)
        {
            _numberRows = values.GetLength(0);
            throw new NotImplementedException();
        }

        public void AddRow(IDictionary<string, double> row)
        {
            if (row.Count != NumberOfColumns)
                throw new ArgumentException();
            foreach (var kvp in row)
                _columns[kvp.Key] = _columns[kvp.Key].Concat(new double[] { kvp.Value }).ToArray();
            _numberRows++;
        }

        public void SetRow(int index, IDictionary<string, double> row)
        {
            foreach (var kvp in row)
                _columns[kvp.Key][index] = kvp.Value;
        }
    }
}
