using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kapibara.ConnectSlots
{
    [Serializable]
    public class ManaPrefab
    {
        [SerializeField] private SlotType _type;
        [SerializeField] private GameObject _prefab;

        public SlotType Type
        {
            get => _type;
        }

        public GameObject Prefab
        {
            get => _prefab;
        }
    }

    [Serializable]
    public class ManaPrefabs
    {
        [SerializeField] private List<ManaPrefab> _slotPrefabs;
        
        public GameObject this[SlotType type]
        {
            get => _slotPrefabs.Find(ss => ss.Type == type).Prefab;
        }
        
    }
    
    
}