using System.Collections.Generic;
using UnityEngine.Events;

namespace Kapibara.ConnectSlots
{
    public class BoardDestroyer
    {
        private BoardView _boardView;

        public BoardDestroyer(BoardView boardView)
        {
            _boardView = boardView;
        }

        /// Destroys a list of slots and notifies when each is destroyed and when all are destroyed.
        public void DestroySlots(List<Slot> slots, UnityAction<Slot> OnSlotDestroyed = null, UnityAction OnAllDestroyed = null)
        {
            if (slots == null || slots.Count == 0)
            {
                OnAllDestroyed?.Invoke();
                return;
            }

            int destroyedCount = 0;
            foreach (Slot slot in slots)
            {
                _boardView.DestroySlot(slot, s =>
                {
                    destroyedCount++;
                    OnSlotDestroyed?.Invoke(s);
                    
                    if (destroyedCount == slots.Count)
                        OnAllDestroyed?.Invoke();
                });
            }
        }
    }
}