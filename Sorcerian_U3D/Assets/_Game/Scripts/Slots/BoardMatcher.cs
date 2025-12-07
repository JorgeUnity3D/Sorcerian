using System.Collections.Generic;

namespace Kapibara.ConnectSlots
{
    public class BoardMatcher
    {
        private Slot[,] _board;
        private int _rows;
        private int _columns;

        public BoardMatcher(Slot[,] board, int rows, int columns)
        {
            _board = board;
            _rows = rows;
            _columns = columns;
        }

        #region MATCH CHECK
        
        public List<Slot> FindAllMatches(int minMatch)
        {
            List<Slot> result = new List<Slot>();

            // Check all rows
            for (int r = 0; r < _rows; r++)
            {
                List<Slot> rowMatches = BoardUtils.CheckMatchesInRow(_board, r, minMatch);
                AddUnique(result, rowMatches);
            }

            // Check all columns
            for (int c = 0; c < _columns; c++)
            {
                List<Slot> columnMatches = BoardUtils.CheckMatchesInColumn(_board, c, minMatch);
                AddUnique(result, columnMatches);
            }

            return result;
        }
        
        private static void AddUnique(List<Slot> target, List<Slot> source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                Slot slot = source[i];
                if (!target.Contains(slot))
                {
                    target.Add(slot);
                }
            }
        }

        #endregion
    }
}