using System;

namespace SudokuSolver.Utilities
{
    public static class SudokuUtility
    {
        public static bool SolveSudoku(int[,] sudokuGrid)
        {
            return SolveSudokuHelper(sudokuGrid, 0, 0);
        }

        private static bool SolveSudokuHelper(int[,] sudokuGrid, int row, int col)
        {
            if (row == 9)
            {
                return true;
            }

            if (col == 9)
            {
                return SolveSudokuHelper(sudokuGrid, row + 1, 0);
            }

            if (sudokuGrid[row, col] != 0)
            {
                return SolveSudokuHelper(sudokuGrid, row, col + 1);
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(sudokuGrid, row, col, num))
                {
                    sudokuGrid[row, col] = num;

                    if (SolveSudokuHelper(sudokuGrid, row, col + 1))
                    {
                        return true;
                    }

                    sudokuGrid[row, col] = 0;
                }
            }

            return false;
        }

        private static bool IsSafe(int[,] sudokuGrid, int row, int col, int num)
        {
            for (int i = 0; i < 9; i++)
            {
                if (sudokuGrid[row, i] == num || sudokuGrid[i, col] == num)
                {
                    return false;
                }
            }

            int startRow = row - row % 3;
            int startCol = col - col % 3;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (sudokuGrid[startRow + i, startCol + j] == num)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}