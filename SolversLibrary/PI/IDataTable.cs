using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.PI
{
    public interface IDataTable
    {
        void SetData(string[] columnNames, double[,] values);
        double[] GetColumn(string name);
        IDictionary<string, double> GetRow(int index);
        IEnumerable<string> ColumnNames { get; }
        void AddRow(IDictionary<string, double> row);

        int NumberOfRows { get; }
        int NumberOfColumns { get; }
        string ColumnAtIndex(int index);
        int ColumnIndex(string name);

        double[] this[string name] { get; }
    }
}
