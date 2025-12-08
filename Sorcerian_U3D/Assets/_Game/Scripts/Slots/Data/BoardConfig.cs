using System;
using UnityEngine;

namespace Kapibara.ConnectSlots
{
    [Serializable]
    public class BoardConfig
    {
        //Data
        private int _id;
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;

        //Instantiation
        [SerializeField] private SlotSprites _slotSprites;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform _slotsParent;
        [SerializeField] private float _sizeX;
        [SerializeField] private float _sizeY;

        //Animations
        [SerializeField] private float _moveDuration;
        [SerializeField] private float _stepDelay;

        //Test
        [SerializeField] private bool _testBoard;

        //Data
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int Rows
        {
            get => _rows;
        }

        public int Columns
        {
            get => _columns;
        }

        //Instantiation
        public SlotSprites Sprites
        {
            get => _slotSprites;
        } 

        public GameObject SlotPrefab
        {
            get => _slotPrefab;
            set => _slotPrefab = value;
        }

        public Transform SlotsParent
        {
            get => _slotsParent;
            set => _slotsParent = value;
        }

        public float SizeX
        {
            get => _sizeX;
            set => _sizeX = value;
        }

        public float SizeY
        {
            get => _sizeY;
            set => _sizeY = value;
        }

        //Animations
        public float MoveDuration
        {
            get => _moveDuration;
        }

        public float StepDelay
        {
            get => _stepDelay;
        }

        //Test
        public bool TestBoard
        {
            get => _testBoard;
        }
    }
}