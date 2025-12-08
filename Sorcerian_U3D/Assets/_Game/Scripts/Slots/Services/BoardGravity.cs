using System.Collections.Generic;
using VContainer;

namespace Kapibara.ConnectSlots
{
    public class BoardGravity
    {
        [Inject] private Board _board;

        /*
         *      132     XXX     132                 132
         *      321 ->  x1x ->  311                 311 ->
         *      212     X2X     222 =>  Se elimina  ---
         *
         *      1 (0, 0) --> aux 1, sB 0 -> (1, 0) 3 -> aux 2, sB 1 -> (2 - sb 1 = 1, 0)
         *      3 --> baja 1
         *
         *      1xxx
         *      2xxx
         *      ---x
         *      ---x
         *      ---x
         *      1 (0,0) -
         *          sB 1 - nB 0 - auxRow 1 =>
         *          sB 1 - nB 1 - auxRow 2 =>
         *          sB 1 - nB 2 - auxRow 3 =>
         *          sB 1 - nB 3 - auxRow 4 =>
         *      1(3,0) ->
         *
         */
        public List<SlotMovement> CalculateGravityMovements()
        {
            List<SlotMovement> movements = new List<SlotMovement>();
            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Columns; c++)
                {
                    Slot slot = _board[r, c];
                    if (slot != null && !slot.IsDestroyed)
                    {
                        int auxRow = r + 1;
                        int slotsBelow = 0;
                        bool hasMovement = false;
                        while (auxRow < _board.Rows)
                        {
                            if (_board[auxRow, c].IsDestroyed)
                            {
                                hasMovement = true;
                            }
                            else
                            {
                                slotsBelow++;
                            }

                            auxRow++;
                        }

                        if (hasMovement)
                        {
                            movements.Add(new SlotMovement(slot, auxRow - slotsBelow - 1, c));
                        }
                    }
                }
            }

            return movements;
        }
    }
}