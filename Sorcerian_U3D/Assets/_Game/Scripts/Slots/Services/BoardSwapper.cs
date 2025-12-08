using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using VContainer;

namespace Kapibara.ConnectSlots
{
    public class BoardSwapper
    {
        [Inject] private Board _board;
        [Inject] private BoardView _boardView;
        
        #region SLOT SWAP LOGIC
        
        public void RequestSwap(Slot slotA, Slot slotB, UnityAction<Slot, Slot> OnSwapComplete = null)
        {
            SwapInGrid(slotA, slotB);
            _boardView.SwapPositions(slotA, slotB, OnSwapComplete);
        }
        
        private void SwapInGrid(Slot slotA, Slot slotB)
        {
            int rowA = slotA.Row;
            int columnA = slotA.Column;

            int rowB = slotB.Row;
            int columnB = slotB.Column;

            // Slot tempSlot = _board[rowA, colA];
            // _board[rowA, colA] = _board[rowB, colB];
            // _board[rowB, colB] = tempSlot;
            (_board[rowA, columnA], _board[rowB, columnB]) = (_board[rowB, columnB], _board[rowA, columnA]);

            slotA.SetPositionData(rowB, columnB);
            slotB.SetPositionData(rowA, columnA);
        }

        public void ApplyGravityInGrid(List<SlotMovement> slotMovements, UnityAction OnComplete, UnityAction OnAnimationComplete = null)
        {
            _boardView.ApplyMovementList(slotMovements, () =>
            {
                OnAnimationComplete?.Invoke();
                slotMovements = slotMovements.OrderByDescending(sm => sm.TargetRow).ToList();
                foreach (SlotMovement slotMovement in slotMovements)
                {
                    int rowA = slotMovement.Slot.Row;
                    int colA = slotMovement.Slot.Column;
                
                    int rowB = slotMovement.TargetRow;
                    int colB = slotMovement.TargetColumn;

                    bool isNew = rowA < 0;
                    
                    if (!isNew)
                    {
                        (_board[rowA, colA], _board[rowB, colB]) = (_board[rowB, colB], _board[rowA, colA]);  
                    }
                    else
                    {
                        slotMovement.Slot.SetPositionData(rowB, colB);
                        _board[rowB, colB] = slotMovement.Slot;
                    }
                }
                OnComplete?.Invoke();
            });
        }
        
        #endregion
    }
}