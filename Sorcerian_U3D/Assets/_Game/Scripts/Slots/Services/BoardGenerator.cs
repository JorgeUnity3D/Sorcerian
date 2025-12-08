using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Kapibara.ConnectSlots
{
    public class BoardGenerator
    {
        [Inject] private Board _board;
        [Inject] private BoardConfig _boardConfig;
        [Inject] private SlotSprites _slotSprites;

        public void GenerateBoardData()
        {
            for (int r = 0; r < _boardConfig.Rows; r++)
            {
                for (int c = 0; c < _boardConfig.Columns; c++)
                {
                    _board[r, c] = GenerateSlot(r, c);
                }
            }
        }

        public void RegenerateEmptySlotsData(List<SlotMovement> slotMovements)
        {
            foreach (SlotMovement slotMovement in slotMovements)
            {
                RegenerateEmptySlotData(slotMovement.Slot);
            }
        }
        
        private Slot GenerateSlot(int row, int colum)
        {
            SlotType type = GetValidRandomType(row, colum);
            Sprite sprite = _slotSprites[type];
            Slot newSlot = new Slot(_boardConfig.Id, row, colum, type, sprite);
            _boardConfig.Id++;
            return newSlot;
        }
        
        private void RegenerateEmptySlotData(Slot slot)
        {
            SlotType type = RNG.PickOne<SlotType>();
            Sprite sprite = _slotSprites[type];
            slot.SlotType = type;
            slot.Sprite = sprite;
            slot.Id = _boardConfig.Id;
            _boardConfig.Id++;
        }

        private SlotType GetValidRandomType(int row, int column)
        {
            while (true)
            {
                SlotType candidate = RNG.PickOne<SlotType>();

                // Check horizontal (only need to check left side)
                if (column >= 2)
                {
                    SlotType left1 = _board[row, column - 1].SlotType;
                    SlotType left2 = _board[row, column - 2].SlotType;

                    if (left1 == candidate && left2 == candidate)
                    {
                        continue; // invalid candidate
                    }
                }

                // Check vertical (only need to check upward)
                if (row >= 2)
                {
                    SlotType up1 = _board[row - 1, column].SlotType;
                    SlotType up2 = _board[row - 2, column].SlotType;

                    if (up1 == candidate && up2 == candidate)
                    {
                        continue; // invalid candidate
                    }
                }

                return candidate;
            }
        }

        public Slot[,] GenerateTestBoardData()
        {
            Slot[,] board = new Slot[_boardConfig.Rows, _boardConfig.Columns];
            int id = 0;
            if (_boardConfig.Rows == 3)
            {
                board[0, 0] = new Slot(id, 0, 0, SlotType.SLOT_EARTH, _slotSprites[SlotType.SLOT_EARTH]);
                board[0, 1] = new Slot(id, 0, 1, SlotType.SLOT_FIRE, _slotSprites[SlotType.SLOT_FIRE]);
                board[0, 2] = new Slot(id, 0, 2, SlotType.SLOT_FOREST, _slotSprites[SlotType.SLOT_FOREST]);
                board[1, 0] = new Slot(id, 1, 0, SlotType.SLOT_SWAMP, _slotSprites[SlotType.SLOT_SWAMP]);
                board[1, 1] = new Slot(id, 1, 1, SlotType.SLOT_WATER, _slotSprites[SlotType.SLOT_WATER]);
                board[1, 2] = new Slot(id, 1, 2, SlotType.SLOT_WIND, _slotSprites[SlotType.SLOT_WIND]);
                board[2, 0] = new Slot(id, 2, 0, SlotType.SLOT_WATER, _slotSprites[SlotType.SLOT_WATER]);
                board[2, 1] = new Slot(id, 2, 1, SlotType.SLOT_FOREST, _slotSprites[SlotType.SLOT_FOREST]);
                board[2, 2] = new Slot(id, 2, 2, SlotType.SLOT_WATER, _slotSprites[SlotType.SLOT_WATER]);
            }

            if (_boardConfig.Rows == 4)
            {
                board[0, 0] = new Slot(id, 0, 0, SlotType.SLOT_EARTH, _slotSprites[SlotType.SLOT_EARTH]);
                board[0, 1] = new Slot(id, 0, 1, SlotType.SLOT_FIRE, _slotSprites[SlotType.SLOT_FIRE]);
                board[0, 2] = new Slot(id, 0, 2, SlotType.SLOT_FOREST, _slotSprites[SlotType.SLOT_FOREST]);
                board[0, 3] = new Slot(id, 0, 3, SlotType.SLOT_EARTH, _slotSprites[SlotType.SLOT_EARTH]);

                board[1, 0] = new Slot(id, 1, 0, SlotType.SLOT_SWAMP, _slotSprites[SlotType.SLOT_SWAMP]);
                board[1, 1] = new Slot(id, 1, 1, SlotType.SLOT_WATER, _slotSprites[SlotType.SLOT_WATER]);
                board[1, 2] = new Slot(id, 1, 2, SlotType.SLOT_FOREST, _slotSprites[SlotType.SLOT_FOREST]);
                board[1, 3] = new Slot(id, 1, 3, SlotType.SLOT_WIND, _slotSprites[SlotType.SLOT_WIND]);

                board[2, 0] = new Slot(id, 2, 0, SlotType.SLOT_FOREST, _slotSprites[SlotType.SLOT_FOREST]);
                board[2, 1] = new Slot(id, 2, 1, SlotType.SLOT_WATER, _slotSprites[SlotType.SLOT_WATER]);
                board[2, 2] = new Slot(id, 2, 2, SlotType.SLOT_FOREST, _slotSprites[SlotType.SLOT_FOREST]);
                board[2, 3] = new Slot(id, 2, 3, SlotType.SLOT_EARTH, _slotSprites[SlotType.SLOT_EARTH]);

                board[3, 0] = new Slot(id, 3, 0, SlotType.SLOT_WATER, _slotSprites[SlotType.SLOT_WATER]);
                board[3, 1] = new Slot(id, 3, 1, SlotType.SLOT_FOREST, _slotSprites[SlotType.SLOT_FOREST]);
                board[3, 2] = new Slot(id, 3, 2, SlotType.SLOT_WATER, _slotSprites[SlotType.SLOT_WATER]);
                board[3, 3] = new Slot(id, 3, 3, SlotType.SLOT_WIND, _slotSprites[SlotType.SLOT_WIND]);
            }
            
            return board;
        }
    }
}