using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Kapibara.ConnectSlots
{
    public class BoardController : IStartable
    {
        [Inject] private BoardView _boardView;
        [Inject] private BoardGenerator _boardGenerator;
        [Inject] private BoardInputHandler _boardInputHandler;
        [Inject] private BoardSwapper _boardSwapper;
        [Inject] private BoardMatcher _boardMatcher;
        [Inject] private BoardDestroyer _boardDestroyer;
        [Inject] private BoardGravity _boardGravity;
        [Inject] private BoardRefiller _boardRefiller;
        [Inject] private BoardDebugger _boardDebugger;
        
        [Inject] private Board _board;
        [Inject] private BoardConfig _boardConfig;

        private List<Slot> _matches = new List<Slot>();

        public void Start()
        {
            _boardGenerator.GenerateBoardData();
            _boardView.GenerateBoardView(_boardInputHandler.OnSlotClicked);

            _boardInputHandler.OnFirstSlotSelected += _boardView.HighlightSlot; 
            _boardInputHandler.OnSecondSlotSelected += _boardView.HighlightSlot;
            _boardInputHandler.OnSwapRequested += OnSwapRequested;
            _boardInputHandler.OnSwapCancelled += OnSwapCancelled; //;
        }

        #region SWAP HANDLING
        
        private void OnSwapRequested(Slot slotA, Slot slotB)
        {
            _boardDebugger.DebugBoard("OnSwapRequested");
            _boardSwapper.RequestSwap(slotA, slotB, OnSwapComplete);
        }

        private void OnSwapCancelled(Slot slotA, Slot slotB = null)
        {
            _boardView.ShakeSlots(new List<Slot>  {slotA, slotB});
            _boardView.ClearAllHighlights();
        }

        private void OnSwapComplete(Slot slotA, Slot slotB)
        {
            _boardDebugger.DebugBoard("OnSwapComplete");
            _boardView.ClearAllHighlights();
            CheckMatches(slotA, slotB);
        }

        private void CheckMatches(Slot slotA = null, Slot slotB = null)
        {
            _matches = _boardMatcher.FindAllMatches(3);
            if (_matches.Count >= 3)
            {
                _boardDestroyer.DestroySlots(_matches, OnSlotDestroyed, OnAllMatchesDestroyed);
            }
            else
            {
                _matches.Clear();
                if (slotA != null)
                {
                    _boardSwapper.RequestSwap(slotB, slotA, OnSwapReverted);
                }
                else
                {
                    _boardInputHandler.ReleaseInput();
                }
            }
        }

        public void OnSwapReverted(Slot slotA, Slot slotB)
        {
            _boardInputHandler.ReleaseInput();
            _boardView.ClearHighlights(slotA, slotB);
        }

        private void OnSlotDestroyed(Slot slot)
        {
            slot.IsDestroyed = true;
        }

        private void OnAllMatchesDestroyed()
        {
            _boardDebugger.DebugBoard("OnAllMatchesDestroyed");
            _matches.Clear();
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            List<SlotMovement> slotMovements = _boardGravity.CalculateGravityMovements();
            _boardSwapper.ApplyGravityInGrid(slotMovements,
                OnGravityApplied); //, () => { DebugBoard("ApplyGravity"); });
        }

        private void OnGravityApplied()
        {
            _boardView.DestroySlots();
            _boardDebugger.DebugBoard("OnGravityApplied");

            RegenerateBoardPositions();
            _boardDebugger.DebugBoard("RegenerateBoardPositions");
            RefillBoard();
        }

        private void RefillBoard()
        {
            List<SlotMovement> movements = _boardRefiller.RefillBoardMovements();
            _boardDebugger.DebugSlotMovementList("RefillBoardMovements", movements);
            _boardGenerator.RegenerateEmptySlotsData(movements);
            _boardDebugger.DebugSlotMovementList("RegenerateEmptySlotsData", movements);

            _boardView.RegenerateEmptySlots(movements, _boardInputHandler.OnSlotClicked);
            _boardDebugger.DebugBoard("RegenerateEmptySlots");
            _boardDebugger.DebugSlotMovementList("RegenerateEmptySlots", movements);

            _boardSwapper.ApplyGravityInGrid(movements, OnBoardRefilled,
                () => { _boardDebugger.DebugBoard("OnNewSlotsGravityAnimationApplied"); });
        }

        private void OnBoardRefilled()
        {
            _boardDebugger.DebugBoard("OnBoardRefilled");
            CheckMatches();
        }

        private void RegenerateBoardPositions()
        {
            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Columns; c++)
                {
                    if (_board[r, c] != null)
                    {
                        _board[r, c].SetPositionData(r, c);
                    }
                }
            }
        }

        #endregion
    }
}