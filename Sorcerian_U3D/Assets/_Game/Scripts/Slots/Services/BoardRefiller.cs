using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Kapibara.ConnectSlots
{
    public class BoardRefiller
    {
        [Inject] private Board _board;

        /*
         *  - - -       (-,-)(-,-)(-,-)
         *  1 2 3   =>  (1,0)(1,1)(1,2) =>  SlotMovements = (-1,0)(-1,1)(-1,2) => to = (0,0)(0,1)(0,2)
         *  3 1 2       (2,0)(2,1)(2,2)
         *
         *
         *  - - - 1       (-,-)(-,-)(-,-)(0,3)                     (-2,0)(-2,1)(-2,2)         (0,0)(0,1)(0,2)
         *  - - - 3   =>  (-,-)(-,-)(-,-)(1,3) =>  SlotMovements = (-1,0)(-1,1)(-1,2) => to = (1,0)(1,1)(1,2)
         *  3 1 4 2       (2,0)(2,1)(2,2)(2,3)
         *  1 2 3 4       (2,0)(2,1)(2,2)(2,3)
         *
         *
         *                                                                           (-5,2)                   (0,2)
         *                                                                           (-4,2)                   (1,2)
         *                                                                           (-3,2)                   (2,2)
         *  - - - 1 2      (-,-)(-,-)(-,-)(0,3)(0,4)                     (-2,0)(-2,1)(-2,2)         (0,0)(0,1)(3,2)
         *  - - - 3 3  =>  (-,-)(-,-)(-,-)(1,3)(1,4) =>  SlotMovements = (-1,0)(-1,1)(-1,2) => to = (1,0)(1,1)(4,2)
         *  3 1 - 2 4      (2,0)(2,1)(-,-)(2,3)(2,4)
         *  1 2 - 4 1      (3,0)(3,1)(-,-)(3,3)(3,4)
         *  3 4 3 1 2      (4,0)(4,1)(-,-)(4,3)(4,4)
         *
         */
        public List<SlotMovement> RefillBoardMovements()
        {
            List<SlotMovement> movements = new List<SlotMovement>();
            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Columns; c++)
                {
                    if (_board[r, c] == null)
                    {
                        int auxRow = 0;
                        int slotsBelow = 0;
                        //Count below
                        while (auxRow < _board.Rows)
                        {
                            if (_board[auxRow, c] == null)
                            {
                                slotsBelow++;
                            }

                            auxRow++;
                        }

                        int targetRow = r - slotsBelow;
                        Slot newSlot = CreateEmptySlot(targetRow, c);
                        SlotMovement slotMovement = new SlotMovement(newSlot, r, c);
                        movements.Add(slotMovement);
                        Debug.Log($"Adding new refiller movement for <color=#55FFFF>({r},{c})</color>: {slotMovement}");
                    }
                }
            }

            return movements;
        }

        private Slot CreateEmptySlot(int row, int column)
        {
            return new Slot(row, column);
        }
    }
}