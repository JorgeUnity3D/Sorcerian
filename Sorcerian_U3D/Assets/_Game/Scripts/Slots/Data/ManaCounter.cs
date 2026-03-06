using System;
using UnityEngine;

namespace Kapibara.ConnectSlots
{
    [Serializable]
    public class ManaCounter
    {
        [SerializeField] private SlotType _slotType;
        [SerializeField] private ManaCounterView _manaCounterView;

        public SlotType SlotType
        {
            get => _slotType;
        }

        public ManaCounterView ManaCounterView
        {
            get => _manaCounterView;
        }
    }
}