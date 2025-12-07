using System;
using System.Collections;
using UnityEngine;

namespace Kapibara.ConnectSlots
{
    [Serializable]
    public enum SlotType
    {
        SLOT_EARTH = 0,
        SLOT_WIND = 1,
        SLOT_FIRE = 2,
        SLOT_WATER = 3,
        SLOT_FOREST = 4,
        SLOT_SWAMP = 5,
    }

    [Serializable]
    public class Slot
    {
        private int _id;
        private int _row;
        private int _column;
        private SlotType _slotType;
        private Sprite _sprite;
        private GameObject _slotInstance;
        private SlotView _slotView;
        private bool _isDestroyed;
        private bool _isEmpty;

        #region PROPERTIES

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int Row
        {
            get => _row;
        }

        public int Column
        {
            get => _column;
        }

        public SlotType SlotType
        {
            get => _slotType;
            set => _slotType = value;
        }

        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public GameObject SlotInstance
        {
            get => _slotInstance;
            set => _slotInstance = value;
        }

        public SlotView SlotView
        {
            get => _slotInstance.GetComponent<SlotView>();
        }

        public bool IsDestroyed
        {
            get => _isDestroyed;
            set => _isDestroyed = value;
        }

        public bool isEmpty
        {
            get => _isEmpty;
            set => _isEmpty = value;
        }

        #endregion

        public Slot(int id, int row, int column, SlotType slotType, Sprite sprite)
        {
            _id = id;
            _row = row;
            _column = column;
            _slotType = slotType;
            _sprite = sprite;
            _isDestroyed = false;
            _isEmpty = false;
        }

        public Slot(int row, int column)
        {
            _id = -1;
            _row = row;
            _column = column;
            _isDestroyed = false;
            _isEmpty = true;
        }

        public void SetPositionData(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public override string ToString()
        {
            return
                $"<color=#2C65DD>ID: {_id} <color=#55FFFF>({_row}, {_column})</color> - Type: {_slotType} {(_sprite != null ? ($"- Sprite:  {_sprite}") : "")} {(_slotInstance != null ? ($"- SlotInstance:  {_slotInstance}") : "")}</color> {(_isDestroyed ? "<color=#FF5555>X</color>" : "<color=#88FF88>V</color>")}";
        }
    }
}