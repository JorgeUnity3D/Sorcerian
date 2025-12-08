using UnityEngine;
using UnityEngine.Events;

namespace Kapibara.ConnectSlots
{
    public class BoardInputHandler
    {
        private Slot _firstSlot;
        private Slot _secondSlot;

        private bool _inputLocked = false;

        public UnityAction<Slot> OnFirstSlotSelected;
        public UnityAction<Slot> OnSecondSlotSelected;
        public UnityAction<Slot, Slot> OnSwapRequested;
        public UnityAction<Slot, Slot> OnSwapCancelled;
        
        public void OnSlotClicked(Slot clickedSlot)
        {
            if (_inputLocked) return;
            if (_firstSlot == null)
            {
                _firstSlot = clickedSlot;
                OnFirstSlotSelected?.Invoke(_firstSlot);
                return;
            }

            if (_secondSlot == null)
            {
                if (_firstSlot == clickedSlot)
                {
                    OnSwapCancelled?.Invoke(_firstSlot, clickedSlot);
                    ReleaseInput();
                    return;
                }

                _secondSlot = clickedSlot;
                OnSecondSlotSelected?.Invoke(_secondSlot);
                ValidateAndSwap();
            }
        }

        private void ValidateAndSwap()
        {
            if (AreAdjacent(_firstSlot, _secondSlot))
            {
                _inputLocked = true;
                OnSwapRequested?.Invoke(_firstSlot, _secondSlot);
            }
            else
            {
                OnSwapCancelled?.Invoke(_firstSlot, _secondSlot);
                ReleaseInput();
            }
        }

        private bool AreAdjacent(Slot slotA, Slot slotB)
        {
            int deltaRow = Mathf.Abs(slotA.Row - slotB.Row);
            int deltaColumn = Mathf.Abs(slotA.Column - slotB.Column);
            return (deltaRow + deltaColumn) == 1;
        }

        public void ReleaseInput()
        {
            _inputLocked = false;
            _firstSlot = null;
            _secondSlot = null;
        }
    }
}