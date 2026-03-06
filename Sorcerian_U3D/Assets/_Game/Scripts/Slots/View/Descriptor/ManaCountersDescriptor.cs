using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kapibara.ConnectSlots
{
    public class ManaCountersDescriptor : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private List<ManaCounter> _manaCounters;

        public Transform Parent
        {
            get => _parent;
        }
        
        public List<ManaCounterView> CounterViews
        {
            get =>  _manaCounters.Select(mc => mc.ManaCounterView).ToList();
        }
        
        public ManaCounterView this[SlotType slotType]
        {
            get => _manaCounters.Find(mc => mc.SlotType == slotType).ManaCounterView;
        }
    }
}