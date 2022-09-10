using LinearAlgebra.Storage;
using System;

namespace LinearAlgebra
{
    public class Matrix
    {
        public MatrixStorage<double> Storage { get; }

        public double this[int rowNumber, int columnNumber]
        {
            get => Storage[rowNumber, columnNumber];
            set => Storage[rowNumber, columnNumber] = value;
        }

        public Matrix(MatrixStorage<double> storage)
        {
            Storage = storage;
        }

        public Matrix(int rowCount, int columnCount)
        {
            Storage = new MatrixStorage<double>(rowCount, columnCount);
        }

        public int RowCount => Storage.RowCount;

        public int ColumnCount => Storage.ColumnCount;

        public bool IsSquare => RowCount == ColumnCount;

        /// <exception cref="IndexOutOfRangeException">Если матрицы не равной размерности.</exception>
        public Matrix CopyTo(Matrix matrix)
        {
            if (IsEqualDimension(matrix, this))
            {
                for (int i = 0; i < matrix.RowCount; i++)
                {
                    for (int j = 0; j < matrix.ColumnCount; j++)
                    {
                        matrix[i, j] = this[i, j];
                    }
                }

                return matrix;
            }
            else
            {
                throw new IndexOutOfRangeException("Матрица не равной размерности!");
            }
        }

        /// <summary> Проверка матриц на то являются ли они равными по размеру.</summary>
        /// <remarks> Одинаковыми по размеру являются матрицы, кол-во столбцов и строк которых попарно равны.</remarks>
        /// <param name="firstMatrix">Матрица определитель которой необходимо найти.</param>
        /// <param name="secondMatrix">Матрица определитель которой необходимо найти.</param>
        /// <returns>True - матрицы равными по размеру True - матрицы равными по размеру.</returns>
        public static bool IsEqualDimension(Matrix firstMatrix, Matrix secondMatrix)
        {
            return IsRowsEqual(firstMatrix, secondMatrix) &&
                IsColumnsEqual(firstMatrix, secondMatrix);
        }

        public static bool IsRowsEqual(Matrix firstMatrix, Matrix secondMatrix)
        {
            return firstMatrix.RowCount == secondMatrix.RowCount;
        }

        public static bool IsColumnsEqual(Matrix firstMatrix, Matrix secondMatrix)
        {
            return firstMatrix.ColumnCount == secondMatrix.ColumnCount;
        }

        /// <summary>Поиск определителя (детерминанта).</summary>
        /// <param name="matrix">Матрица определитель которой необходимо найти.</param>
        /// <returns>Значение определителя.</returns>
        /// <exception cref="IndexOutOfRangeException">Если матрица не является квадратной.</exception>
        public static double Determinant(Matrix matrix)
        {
            if (matrix.IsSquare)
            {
                double determinant = 1;

                Matrix triangleMatrix = Triangle(matrix);

                for (int i = 0; i < triangleMatrix.RowCount; i++)
                {
                    determinant *= triangleMatrix[i, i];
                }

                return determinant;
            }
            else
            {
                throw new IndexOutOfRangeException("Матрица не является квадратной!");
            }
        }

        /// <summary>Приведение матрицы к треугольному виду.</summary>
        /// <param name="matrix">Исходная матрица.</param>
        /// <returns>Треугольная матрица.</returns>
        public static Matrix Triangle(Matrix matrix)
        {
            Matrix triangleMatrix = new Matrix(matrix.RowCount, matrix.ColumnCount);
            matrix.CopyTo(triangleMatrix);

            for (int i = 0; i < triangleMatrix.RowCount - 1; i++)
            {
                for (int j = i + 1; j < triangleMatrix.RowCount; j++)
                {
                    double coefficient = triangleMatrix[i, i] == 0 ? 0 : triangleMatrix[j, i] / triangleMatrix[i, i];

                    for (int k = i; k < triangleMatrix.RowCount; k++)
                    {
                        triangleMatrix[j, k] -= triangleMatrix[i, k] * coefficient;
                    }
                }
            }

            return triangleMatrix;
        }

        /// <summary>Сложение двух матриц</summary>
        /// <param name="firstMatrix">Первое слагаемое</param>
        /// <param name="secondMatrix">Второе слагаемое</param>
        /// <returns>Результат сложения двух матриц</returns>
        /// <exception cref="IndexOutOfRangeException">Если матрицы не равной размерности.</exception>
        public static Matrix Addition(Matrix firstMatrix, Matrix secondMatrix)
        {
            if (IsEqualDimension(firstMatrix, secondMatrix))
            {
                Matrix resultOfAdditionMatrix = new Matrix(firstMatrix.RowCount, firstMatrix.ColumnCount);

                for (int i = 0; i < resultOfAdditionMatrix.RowCount; i++)
                {
                    for (int j = 0; j < resultOfAdditionMatrix.ColumnCount; j++)
                    {
                        resultOfAdditionMatrix[i, j] = firstMatrix[i, j] + secondMatrix[i, j];
                    }
                }

                return resultOfAdditionMatrix;
            }
            else
            {
                throw new IndexOutOfRangeException("Матрицы не одинаковой размерности!");
            }
        }

        /// <summary>Возвращает противоположную матрицу</summary>
        /// <remarks>Не путать с транспонированием</remarks>
        /// <param name="matrix">Исходная матрица</param>
        /// <returns>Матрица все значения которой противоположны относительно исходной матрицы</returns>
        public static Matrix Opposite(Matrix matrix)
        {
            Matrix oppositeMatrix = new Matrix(matrix.RowCount, matrix.ColumnCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    oppositeMatrix[i, j] = -matrix[i, j];
                }
            }

            return oppositeMatrix;
        }

        /// <summary>Алгоритм транспонирования матрицы</summary>
        /// <remarks>Не путать с поиском противоположной матрицы</remarks>
        /// <param name="matrix">Исходная матрица</param>
        /// <returns>Возвращает транспонированную матрицу</returns>
        public static Matrix Transpose(Matrix matrix)
        {
            Matrix transposeMatrix = new Matrix(matrix.ColumnCount, matrix.RowCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    transposeMatrix[j, i] = matrix[i, j];
                }
            }

            return transposeMatrix;
        }

        /// <summary>Возвращает еденичную матрицу</summary>
        /// <remarks>Еденичная матрица является квадратной</remarks>
        /// <param name="dimension">Размер матрицы</param>
        /// <returns>Квадратная матрица все элементы которой равны единице</returns>
        public static Matrix Identity(int dimension)
        {
            Matrix identityMatrix = new Matrix(dimension, dimension);

            for (int i = 0; i < identityMatrix.RowCount; i++)
            {
                for (int j = 0; j < identityMatrix.ColumnCount; j++)
                {
                    identityMatrix[i, j] = i == j ? 1 : 0;
                }
            }

            return identityMatrix;
        }

        /// <summary>Умножение двух матриц</summary>
        /// <param name="firstMatrix">Первое слагаемое</param>
        /// <param name="secondMatrix">Второе слагаемое</param>
        /// <returns>Результат сложения двух матриц</returns>
        /// <exception cref="IndexOutOfRangeException">Число столбцов первой матрицы не равно числу строк второй матрицы.</exception>
        public static Matrix Multiplication(Matrix firstMatrix, Matrix secondMatrix)
        {
            if (firstMatrix.ColumnCount == secondMatrix.RowCount)
            {
                Matrix resultMultiplicationMatrix = new Matrix(firstMatrix.RowCount, secondMatrix.ColumnCount);

                for (int i = 0; i < resultMultiplicationMatrix.RowCount; i++)
                {
                    for (int j = 0; j < resultMultiplicationMatrix.ColumnCount; j++)
                    {
                        for (int m = 0; m < firstMatrix.ColumnCount; m++)
                        {
                            resultMultiplicationMatrix[i, j] += firstMatrix[i, m] * secondMatrix[m, j];
                        }
                    }
                }

                return resultMultiplicationMatrix;
            }
            else
            {
                throw new IndexOutOfRangeException("Число столбцов первой матрицы не равно числу строк второй матрицы!");
            }
        }

        /// <summary>Вычитание матриц</summary>
        /// <param name="firstMatrix">Уменьшаемая матрица</param>
        /// <param name="secondMatrix">Вычитаемая матрица</param>
        /// <returns>Результат вычитания матриц</returns>
        /// <exception cref="IndexOutOfRangeException">Матрицы не одинаковой размерности.</exception>
        public static Matrix Difference(Matrix firstMatrix, Matrix secondMatrix)
        {
            if (IsEqualDimension(firstMatrix, secondMatrix))
            {
                Matrix resultDifferenceMatrix = new Matrix(firstMatrix.RowCount, firstMatrix.ColumnCount);

                for (int i = 0; i < resultDifferenceMatrix.RowCount; i++)
                {
                    for (int j = 0; j < resultDifferenceMatrix.ColumnCount; j++)
                    {
                        resultDifferenceMatrix[i, j] = firstMatrix[i, j] - secondMatrix[i, j];
                    }
                }

                return resultDifferenceMatrix;
            }
            else
            {
                throw new IndexOutOfRangeException("Матрицы не одинаковой размерности!");
            }
        }

        /// <summary>Замена столбцов</summary>
        /// <param name="matrix">Исходная матрица</param>
        /// <param name="replaceColumn">Столбец с новыми значениями</param>
        /// <param name="columnNumber">Номер столбца для замены</param>
        /// <returns>Матрица с заменой столбцов</returns>
        /// <exception cref="IndexOutOfRangeException">Указанного столбца не существует.</exception>
        public static Matrix ReplaceColumn(Matrix matrix, double[] replaceColumn, int columnNumber)
        {
            if (columnNumber <= matrix.ColumnCount && replaceColumn.Length == matrix.ColumnCount)
            {
                Matrix resultReplaceMatrix = new Matrix(matrix.RowCount, matrix.ColumnCount);
                matrix.CopyTo(resultReplaceMatrix);

                for (int i = 0; i < resultReplaceMatrix.RowCount; i++)
                {
                    resultReplaceMatrix[i, columnNumber] = replaceColumn[i];
                }

                return resultReplaceMatrix;
            }
            else
            {
                throw new IndexOutOfRangeException("Такого столбца не существует!");
            }
        }

        /// <summary>Метод Гаусса-Жордана для решения слау</summary>
        /// <param name="matrix">Матрица состоящая из коэффициентов при агрументах</param>
        /// <param name="freeCoefficients">Массив свободных коэффициентов</param>
        /// <returns>Матрица с заменой столбцов</returns>
        public static double[] GaussJordan(Matrix matrix, double[] freeCoefficients)
        {
            double[] result = new double[freeCoefficients.Length];
            freeCoefficients.CopyTo(result, 0);

            Matrix currentMatrix = new Matrix(matrix.RowCount, matrix.ColumnCount);
            matrix.CopyTo(currentMatrix);

            Matrix temporaryMatrix = new Matrix(currentMatrix.RowCount, currentMatrix.ColumnCount);
            double[] temporaryResult = new double[result.Length];

            int allowRow = 0;
            while (allowRow < currentMatrix.RowCount)
            {
                double allow = currentMatrix[allowRow, allowRow];

                for (int i = 0; i < currentMatrix.RowCount; i++)
                {
                    for (int j = 0; j < currentMatrix.ColumnCount; j++)
                    {
                        if (i == allowRow)
                        {
                            temporaryMatrix[i, j] = currentMatrix[i, j] / allow;
                            temporaryResult[i] = result[i] / allow;
                        }
                        else
                        {

                            temporaryMatrix[i, j] = currentMatrix[i, j] - (currentMatrix[allowRow, j] / allow * currentMatrix[i, allowRow]);

                            if (j == currentMatrix.ColumnCount - 1)
                            {
                                temporaryResult[i] = result[i] - (result[allowRow] / allow * currentMatrix[i, allowRow]);
                            }
                        }
                    }
                }

                for (int i = 0; i < currentMatrix.RowCount; i++)
                {
                    for (int j = 0; j < currentMatrix.ColumnCount; j++)
                    {
                        currentMatrix[i, j] = temporaryMatrix[i, j];
                        temporaryMatrix[i, j] = 0;

                    }

                    result[i] = temporaryResult[i];

                    temporaryResult[i] = 0;
                }

                allowRow++;
            }

            return result;
        }
    }
}
