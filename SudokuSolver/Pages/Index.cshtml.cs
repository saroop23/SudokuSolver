using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using SudokuSolver.Utilities;


namespace SudokuSolver.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int[,] SudokuGrid { get; set; }


        public IActionResult OnGet()
        {
            GenerateRandomSudoku();
            return Page();
        }
        private int[,] ConvertSudokuToArray(string sudokuString)
        {
            int[,] sudokuGrid = new int[9, 9];

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    char cellValue = sudokuString[(row * 9) + col];

                    if (cellValue >= '1' && cellValue <= '9')
                    {
                        sudokuGrid[row, col] = cellValue - '0';
                    }
                    else if (cellValue == '0')
                    {
                        sudokuGrid[row, col] = 0;
                    }
                    else
                    {
                        // Handle invalid characters here (optional)
                    }
                }
            }

            return sudokuGrid;
        }



        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (!IsSolved(SudokuGrid))
                {
                    if (SudokuUtility.SolveSudoku(SudokuGrid))
                    {
                        return Page();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The provided Sudoku puzzle cannot be solved.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please provide a valid Sudoku puzzle.");
            }

            return Page();
        }


        private bool IsSolved(int[,] sudokuGrid)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (sudokuGrid[row, col] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void GenerateRandomSudoku()
        {
            int[,] sudokuGrid = new int[9, 9];
            Random random = new Random();

            // Generate a solved Sudoku grid
            SolveSudokuHelper(sudokuGrid, 0, 0);

            // Remove random cells to create a puzzle
            int cellsToRemove = random.Next(40, 55); // Adjust the range as per desired difficulty
            for (int i = 0; i < cellsToRemove; i++)
            {
                int row = random.Next(9);
                int col = random.Next(9);
                while (sudokuGrid[row, col] == 0) // Skip if the cell is already empty
                {
                    row = random.Next(9);
                    col = random.Next(9);
                }
                int temp = sudokuGrid[row, col];
                sudokuGrid[row, col] = 0;
                int[,] tempGrid = (int[,])sudokuGrid.Clone();

                // Check if the puzzle has a unique solution
                if (!HasUniqueSolution(tempGrid))
                {
                    sudokuGrid[row, col] = temp; // Restore the original cell value if it doesn't have a unique solution
                }
            }

            SudokuGrid = sudokuGrid;
        }

        private bool SolveSudokuHelper(int[,] sudokuGrid, int row, int col)
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


        private bool IsSafe(int[,] sudokuGrid, int row, int col, int num)
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

        private bool HasUniqueSolution(int[,] sudokuGrid)
        {
            int[,] tempGrid = (int[,])sudokuGrid.Clone();
            return SolveSudokuHelper(tempGrid, 0, 0) && !SolveSudokuHelper(tempGrid, 0, 0);
        }
    }
}
