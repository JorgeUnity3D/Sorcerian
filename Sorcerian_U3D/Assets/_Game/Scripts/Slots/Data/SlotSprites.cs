using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kapibara.ConnectSlots
{
    [Serializable]
    public class SlotSprite
    {
        [SerializeField] public SlotType type;
        [SerializeField] public Sprite sprite;
    }
    
    [Serializable]
    public class SlotSprites
    {
        [SerializeField] private List<SlotSprite> _slotSprites;
        
        public Sprite this[SlotType type]
        {
            get => _slotSprites.Find(ss => ss.type == type).sprite;
        }
    }
}