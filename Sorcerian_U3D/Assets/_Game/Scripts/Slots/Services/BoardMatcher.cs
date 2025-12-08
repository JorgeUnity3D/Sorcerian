using System.Collections.Generic;
using VContainer;

namespace Kapibara.ConnectSlots
{
    public class BoardMatcher
    {
        [Inject] private Board _board;

        #region MATCH CHECK
        
        public List<Slot> FindAllMatches(int minMatch)
        {
            List<Slot> result = new List<Slot>();

            // Check all rows
            for (int r = 0; r < _board.Rows; r++)
            {
                List<Slot> rowMatches = BoardUtils.CheckMatchesInRow(_board, r, minMatch);
                AddUnique(result, rowMatches);
            }

            // Check all columns
            for (int c = 0; c < _board.Columns; c++)
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