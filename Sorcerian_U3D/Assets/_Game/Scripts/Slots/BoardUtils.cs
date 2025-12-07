using System.Collections.Generic;

namespace Kapibara.ConnectSlots
{
    public static class Directions
    {
        // Single directions as constants
        public static readonly (int, int) Up = (-1, 0);
        public static readonly (int, int) Down = (1, 0);
        public static readonly (int, int) Left = (0, -1);
        public static readonly (int, int) Right = (0, 1);
        public static readonly (int, int) UpLeft = (-1, -1);
        public static readonly (int, int) UpRight = (-1, 1);
        public static readonly (int, int) DownLeft = (1, -1);
        public static readonly (int, int) DownRight = (1, 1);

        // Direction groups
        public static readonly (int, int)[] HorizontalDirs = { Left, Right };
        public static readonly (int, int)[] VerticalDirs = { Up, Down };
        public static readonly (int, int)[] OrthogonalDirs = { Up, Down, Left, Right };
        public static readonly (int, int)[] DiagonalDirs = { UpLeft, UpRight, DownLeft, DownRight };
        public static readonly (int, int)[] AllDirs = { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight };
    }

    public static class BoardUtils
    {
        #region MATCH CHECKERS

        public static List<Slot> CheckMatchesInRow(Slot[,] grid, int row, int minMatch)
        {
            List<Slot> matches = new List<Slot>();
            
            int columns = grid.GetLength(1);
            int count = 1;
            
            for (int c = 1; c < columns; c++)
            {
                if (grid[row, c].SlotType == grid[row, c - 1].SlotType)
                {
                    count++;
                }
                else
                {
                    if (count >= minMatch)
                    {
                        for (int k = 1; k <= count; k++)
                        {
                            matches.Add(grid[row, c - k]);
                        }
                    }

                    count = 1;
                }
            }

            if (count >= minMatch)
            {
                for (int k = 0; k < count; k++)
                {
                    matches.Add(grid[row, columns - 1 - k]);
                }
            }

            return matches;
        }

        public static List<Slot> CheckMatchesInColumn(Slot[,] grid, int column, int minMatch)
        {
            List<Slot> matches = new List<Slot>();

            int rows = grid.GetLength(0);
            int count = 1;

            for (int r = 1; r < rows; r++)
            {
                if (grid[r, column].SlotType == grid[r - 1, column].SlotType)
                {
                    count++;
                }
                else
                {
                    if (count >= minMatch)
                    {
                        for (int k = 1; k <= count; k++)
                        {
                            matches.Add(grid[r - k, column]);
                        }
                    }
                    count = 1;
                }
            }

            // Final check for bottom of column
            if (count >= minMatch)
            {
                for (int k = 0; k < count; k++)
                {
                    matches.Add(grid[rows - 1 - k, column]);
                }
            }

            return matches;
        }

        #endregion

        #region SLOT GETTERS

        /// <summary>
        /// Returns all cells in the given row.
        /// Example (row = 1):
        /// Grid:
        /// 0 1 2
        /// 3 4 5 
        /// 6 7 8
        /// Returns: 3, 4, 5
        /// </summary>
        public static List<T> GetRow<T>(T[,] grid, int row)
        {
            List<T> result = new List<T>();
            for (int c = 0; c < grid.GetLength(1); c++)
            {
                if (InBounds(grid, row, c))
                {
                    result.Add(grid[row, c]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all cells in the given column.
        /// Example (column = 2):
        /// Grid:
        /// 0 1 2
        /// 3 4 5
        /// 6 7 8
        /// Returns: 2, 5, 8
        /// </summary>
        public static List<T> GetColumn<T>(T[,] grid, int column)
        {
            List<T> result = new List<T>();
            for (int r = 0; r < grid.GetLength(0); r++)
            {
                if (InBounds(grid, r, column))
                {
                    result.Add(grid[r, column]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all 8 neighbors (orthogonal + diagonal) of the cell at (row, column).
        /// Example (row = 1, column = 1):
        /// Grid indexes:
        /// 0 1 2
        /// 3 4 5
        /// 6 7 8
        /// Neighbors of 4: 0, 1, 2, 3, 5, 6, 7, 8
        /// </summary>
        public static List<T> GetNeighbors<T>(T[,] grid, int row, int column, int distance)
        {
            List<T> result = new List<T>();
            for (int r = row - distance; r <= row + distance; r++)
            {
                for (int c = column - distance; c <= column + distance; c++)
                {
                    if (r == row && c == column)
                    {
                        continue;
                    }

                    if (InBounds(grid, r, c))
                    {
                        result.Add(grid[r, c]);
                    }
                }
            }

            if (InBounds(grid, row, column))
            {
                result.Add(grid[row, column]);
            }

            return result;
        }


        /// <summary>
        /// Returns horizontal neighbors (left and right) of the cell at (row, column).
        /// Example (row = 1, column = 1, distance = 3):
        /// Grid indexes:
        /// 0 1 2 3 4
        /// Left neighbors: 0, 1
        /// Right neighbors: 3, 4
        /// Returned in order: left side, then right side.
        /// </summary>
        public static List<T> GetNeighborsHorizontal<T>(T[,] grid, int row, int column, int distance = 3)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < Directions.HorizontalDirs.Length; i++)
            {
                int deltaRow = Directions.HorizontalDirs[i].Item1;
                int deltaColumn = Directions.HorizontalDirs[i].Item2;

                for (int d = 1; d <= distance; d++)
                {
                    int newRow = row + deltaRow * d;
                    int newColumn = column + deltaColumn * d;

                    if (InBounds(grid, newRow, newColumn))
                    {
                        result.Add(grid[newRow, newColumn]);
                    }
                }
            }

            if (InBounds(grid, row, column))
            {
                result.Add(grid[row, column]);
            }

            return result;
        }

        /// <summary>
        /// Returns vertical neighbors (up and down) of the cell at (row, column).
        /// Example (row = 2, column = 1, distance = 2):
        /// Grid indexes:
        /// 0, 1, 2
        /// 3, 4, 5
        /// 6, 7, 8
        /// 9,10,11
        /// Up neighbors: 4, 1
        /// Down neighbors: 10
        /// Returned in order: upward first, then downward.
        /// </summary>
        public static List<T> GetNeighborsVertical<T>(T[,] grid, int row, int column, int distance = 3)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < Directions.VerticalDirs.Length; i++)
            {
                int deltaRow = Directions.VerticalDirs[i].Item1;
                int deltaColumn = Directions.VerticalDirs[i].Item2;

                for (int d = 1; d <= distance; d++)
                {
                    int newRow = row + deltaRow * d;
                    int newColumn = column + deltaColumn * d;

                    if (InBounds(grid, newRow, newColumn))
                    {
                        result.Add(grid[newRow, newColumn]);
                    }
                }
            }

            if (InBounds(grid, row, column))
            {
                result.Add(grid[row, column]);
            }

            return result;
        }

        /// <summary>
        /// Returns the 4 orthogonal neighbors (up, down, left, right) of the cell at (row, column).
        /// Example (row = 1, column = 1):
        /// Grid indexes:
        /// 0 1 2
        /// 3 4 5
        /// 6 7 8
        /// Orthogonal neighbors of 4: 1 (up), 7 (down), 3 (left), 5 (right)
        /// </summary>
        public static List<T> GetNeighborsOrthogonal<T>(T[,] grid, int row, int column, int distance = 1)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < Directions.OrthogonalDirs.Length; i++)
            {
                int deltaRow = Directions.OrthogonalDirs[i].Item1;
                int deltaColumn = Directions.OrthogonalDirs[i].Item2;

                for (int d = 1; d <= distance; d++)
                {
                    int newRow = row + deltaRow * d;
                    int newColumn = column + deltaColumn * d;

                    if (InBounds(grid, newRow, newColumn))
                    {
                        result.Add(grid[newRow, newColumn]);
                    }
                }
            }

            if (InBounds(grid, row, column))
            {
                result.Add(grid[row, column]);
            }

            return result;
        }

        /// <summary>
        /// Returns the 4 diagonal neighbors of the cell at (row, column).
        /// Example (row = 1, column = 1):
        /// Grid indexes:
        /// 0 1 2
        /// 3 4 5
        /// 6 7 8
        /// Diagonal neighbors of 4: 0 (up-left), 2 (up-right), 6 (down-left), 8 (down-right)
        /// </summary>
        public static List<T> GetNeighborsDiagonal<T>(T[,] grid, int row, int column, int distance = 1)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < Directions.DiagonalDirs.Length; i++)
            {
                int deltaRow = Directions.DiagonalDirs[i].Item1;
                int deltaColumn = Directions.DiagonalDirs[i].Item2;

                for (int d = 1; d <= distance; d++)
                {
                    int newRow = row + deltaRow * d;
                    int newColumn = column + deltaColumn * d;

                    if (InBounds(grid, newRow, newColumn))
                    {
                        result.Add(grid[newRow, newColumn]);
                    }
                }
            }

            if (InBounds(grid, row, column))
            {
                result.Add(grid[row, column]);
            }

            return result;
        }

        public static List<T> CollectInDirection<T>(T[,] grid, int startRow, int startColumn, (int, int) direction,
            int maxDistance = 10)
        {
            List<T> result = new List<T>();

            int dRow = direction.Item1;
            int dCol = direction.Item2;

            for (int step = 1; step <= maxDistance; step++)
            {
                int r = startRow + dRow * step;
                int c = startColumn + dCol * step;

                if (!InBounds(grid, r, c))
                    break;

                result.Add(grid[r, c]);
            }

            return result;
        }

        /// <summary>
        /// Returns true if the given row and column are inside the bounds of the grid.
        /// </summary>
        public static bool InBounds<T>(T[,] grid, int row, int column)
        {
            return row >= 0 && row < grid.GetLength(0) &&
                   column >= 0 && column < grid.GetLength(1);
        }

        #endregion
    }
}