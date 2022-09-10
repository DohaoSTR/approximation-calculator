using System;

namespace LinearAlgebra.Storage
{
    public class MatrixStorage<T> where T : struct, IEquatable<T>, IFormattable
    {
        public T[,] Data { get; private set; }

        public int RowCount { get; private set; }

        public int ColumnCount { get; private set; }

        public MatrixStorage(int rowCount, int columnCount)
        {
            if (rowCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "Количество строк матрицы не может быть меньше нуля.");
            }

            if (columnCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnCount), "Количество столбцов матрицы не может быть меньше нуля.");
            }

            RowCount = rowCount;
            ColumnCount = columnCount;

            Data = new T[RowCount, ColumnCount];
        }

        public MatrixStorage(T[,] data)
        {
            Data = data;

            RowCount = data.GetLength(0);
            ColumnCount = data.GetLength(1);
        }

        public T this[int row, int column]
        {
            get
            {
                ValidateRange(row, column);
                return Data[row, column];
            }

            set
            {
                ValidateRange(row, column);
                Data[row, column] = value;
            }
        }

        private void ValidateRange(int row, int column)
        {
            if ((uint)row >= (uint)RowCount)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if ((uint)column >= (uint)ColumnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }
        }
    }
}
