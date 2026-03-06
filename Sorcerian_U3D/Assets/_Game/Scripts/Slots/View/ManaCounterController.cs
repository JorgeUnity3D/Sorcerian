using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Kapibara.ConnectSlots
{
    public class ManaCounterController
    {
        [Inject] private ManaCountersDescriptor _manaCountersDescriptor;
        private Dictionary<SlotType, int> _manaCounter = new Dictionary<SlotType, int>();

        public void AddMana(SlotType slotType)
        {
            if (_manaCounter.ContainsKey(slotType))
            {
                _manaCounter[slotType]++;;
            }
            else
            {
                _manaCounter.Add(slotType, 1);
            }
            _manaCountersDescriptor[slotType].UpdateCount(_manaCounter[slotType]);
        }
    }
}