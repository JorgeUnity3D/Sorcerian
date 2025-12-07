using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kapibara.ConnectSlots
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private bool _testBoard;
        [SerializeField] private int _rows = 3;
        [SerializeField] private int _columns = 3;
        [SerializeField] private Sprite[] _slotSprites;

        private BoardGenerator _boardGenerator;
        private BoardInputHandler _boardInputHandler;
        [SerializeField] private BoardView _boardView;
        private BoardSwapper _boardSwapper;
        private BoardMatcher _boardMatcher;
        private BoardDestroyer _boardDestroyer;
        private BoardGravity _boardGravity;
        private BoardRefiller _boardRefiller;

        private Slot[,] _board;
        private List<Slot> _matches = new List<Slot>();
        private int _destroyCount;

        private void Start()
        {
            _boardGenerator = new BoardGenerator(_board, _rows, _columns, _slotSprites);
            if (_testBoard)
            {
                _board = _boardGenerator.GenerateTestBoardData();
            }
            else
            {
                _board = _boardGenerator.GenerateBoardData();
            }

            _boardInputHandler = new BoardInputHandler();
            _boardView.GenerateBoardView(_board, _boardInputHandler.OnSlotClicked);
            _boardSwapper = new BoardSwapper(_board, _boardView);
            _boardMatcher = new BoardMatcher(_board, _rows, _columns);
            _boardDestroyer = new BoardDestroyer(_boardView);
            _boardGravity = new BoardGravity(_board, _rows, _columns);
            _boardRefiller = new BoardRefiller(_board, _rows, _columns);

            // Subscribe to input events
            _boardInputHandler.OnFirstSlotSelected += _boardView.HighlightSlot;
            _boardInputHandler.OnSecondSlotSelected += _boardView.HighlightSlot;
            _boardInputHandler.OnSwapRequested += OnSwapRequested;
            _boardInputHandler.OnSwapCancelled += _boardView.ClearAllHighlights;
        }

        #region SWAP HANDLING

        private void OnSwapRequested(Slot slotA, Slot slotB)
        {
            DebugBoard("OnSwapRequested");

            _boardSwapper.RequestSwap(slotA, slotB, OnSwapComplete);
        }

        private void OnSwapComplete(Slot slotA, Slot slotB)
        {
            DebugBoard("OnSwapComplete");
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
            //_boardView.ClearAllHighlights();
            _boardView.ClearHighlights(slotA, slotB);
        }

        private void OnSlotDestroyed(Slot slot)
        {
            slot.IsDestroyed = true;
        }

        private void OnAllMatchesDestroyed()
        {
            DebugBoard("OnAllMatchesDestroyed");
            _boardView.ClearAllHighlights();
            _matches.Clear();
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            List<SlotMovement> slotMovements = _boardGravity.CalculateGravityMovements();
            _boardSwapper.ApplyGravityInGrid(slotMovements, OnGravityApplied); //, () => { DebugBoard("ApplyGravity"); });
        }

        private void OnGravityApplied()
        {
            CleanDestroyedSlots();
            DebugBoard("OnGravityApplied");
            
            RegenerateBoardPositions();
            DebugBoard("RegenerateBoardPositions");
            RefillBoard();
        }

        private void RefillBoard()
        {
            List<SlotMovement> movements = _boardRefiller.RefillBoardMovements();
            DebugSlotMovementList("RefillBoardMovements", movements);
            _boardGenerator.RegenerateEmptySlotsData(movements);
            DebugSlotMovementList("RegenerateEmptySlotsData", movements);
            
            _boardView.RegenerateEmptySlots(movements, _boardInputHandler.OnSlotClicked);
            DebugBoard("RegenerateEmptySlots");
            DebugSlotMovementList("RegenerateEmptySlots", movements);
            
            _boardSwapper.ApplyGravityInGrid(movements, OnBoardRefilled, () => { DebugBoard("OnNewSlotsGravityAnimationApplied"); });
        }

        private void OnBoardRefilled()
        {
            DebugBoard("OnBoardRefilled");
            CheckMatches();
        }

        private void CleanDestroyedSlots()
        {
            for (int r = 0; r < _board.GetLength(0); r++)
            {
                for (int c = 0; c < _board.GetLength(1); c++)
                {
                    if (_board[r, c].IsDestroyed)
                    {
                        Destroy(_board[r, c].SlotInstance);
                        _board[r, c] = null;
                    }
                }
            }
        }

        private void RegenerateBoardPositions()
        {
            for (int r = 0; r < _board.GetLength(0); r++)
            {
                for (int c = 0; c < _board.GetLength(1); c++)
                {
                    if (_board[r, c] != null)
                    {
                        _board[r, c].SetPositionData(r, c);
                    }
                }
            }
        }

        #endregion

        #region TESTS

        private void DebugBoard(string title)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"--- <b><color=#FFD700>{title}</color></b> ---");

            // ============================================================
            // 1. PREPARAR TABLA DE TIPOS (sin color, para medir correctamente)
            // ============================================================

            string[,] typeTable = new string[_rows, _columns];
            int[] typeColWidths = new int[_columns];

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    string raw =
                        _board[r, c] == null ? "-" :
                        _board[r, c].IsDestroyed ? "0" :
                        (((int)_board[r, c].SlotType) + 1).ToString();

                    typeTable[r, c] = raw;

                    if (raw.Length > typeColWidths[c])
                        typeColWidths[c] = raw.Length;
                }
            }

            // ============================================================
            // 2. PREPARAR TABLA DE POSICIONES (también sin color)
            // ============================================================

            string[,] posTable = new string[_rows, _columns];
            int[] posColWidths = new int[_columns];

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    string raw =
                        _board[r, c] == null ? "(--,--)" :
                        _board[r, c].IsDestroyed ? $"({_board[r, c].Row:N2},{_board[r, c].Column:N2})" :
                        $"({_board[r, c].Row:D2},{_board[r, c].Column:D2})";

                    posTable[r, c] = raw;

                    if (raw.Length > posColWidths[c])
                        posColWidths[c] = raw.Length;
                }
            }

            // ============================================================
            // 3. IMPRIMIR TABLA DE TIPOS CON COLORES, DEBAJO
            // ============================================================

            sb.AppendLine("\n<b><color=#8888FF>TYPE TABLE</color></b>");

            int totalTypeWidth = typeColWidths.Sum() + _columns * 3 + 1;
            sb.AppendLine("+" + new string('-', totalTypeWidth) + "+");

            for (int r = 0; r < _rows; r++)
            {
                sb.Append("|");
                for (int c = 0; c < _columns; c++)
                {
                    string raw = typeTable[r, c].PadLeft(typeColWidths[c]);

                    string colored =
                        _board[r, c] == null ? $"<color=#888888>{raw}</color>" :
                        _board[r, c].IsDestroyed ? $"<color=#FF5555>{raw}</color>" :
                        $"<color=#55FFFF>{raw}</color>";

                    sb.Append(" " + colored + " |");
                }

                sb.AppendLine();
            }

            sb.AppendLine("+" + new string('-', totalTypeWidth) + "+");

            // ============================================================
            // 4. IMPRIMIR TABLA DE POSICIONES DEBAJO DE LA DE TIPOS
            // ============================================================

            sb.AppendLine("\n<b><color=#88FF88>POSITION TABLE</color></b>");

            int totalPosWidth = posColWidths.Sum() + _columns * 3 + 1;
            sb.AppendLine("+" + new string('-', totalPosWidth) + "+");

            for (int r = 0; r < _rows; r++)
            {
                sb.Append("|");
                for (int c = 0; c < _columns; c++)
                {
                    string raw = posTable[r, c].PadLeft(posColWidths[c]);

                    string colored =
                        _board[r, c] == null ? $"<color=#888888>{raw}</color>" :
                        _board[r, c].IsDestroyed ? $"<color=#FF5555>{raw}</color>" :
                        $"<color=#55FFFF>{raw}</color>";

                    sb.Append(" " + colored + " |");
                }

                sb.AppendLine();
            }

            sb.AppendLine("+" + new string('-', totalPosWidth) + "+");

            Debug.Log(sb.ToString());
        }

        private void DebugSlotList(string title, List<Slot> slotList)
        {
            string result = $"· {title}\n";
            for (int i = 0; i < slotList.Count; i++)
            {
                result += $"    ({slotList[i]})\n";
            }

            Debug.Log(result);
        }

        private void DebugSlotMovementList(string title, List<SlotMovement> slotMovementList)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"--- <b><color=#8888FF>{title}</color></b> ---");
            foreach (SlotMovement slotMovement in slotMovementList)
            {
                sb.AppendLine($"{slotMovement}");
            }
            Debug.Log(sb.ToString());
        }
        
        #endregion
        
        #region TESTS
        
        [SerializeField] private int _testRow;
        [SerializeField] private int _testColumn;
        [SerializeField] private int _testDistance;

        private void TestHorizontal()
        {
            List<Slot> horizontal = BoardUtils.GetNeighborsHorizontal(_board, _testRow, _testColumn, _testDistance);
            DebugSlotList("Horizontal", horizontal);
            _boardView.HighlightSlotList(horizontal);
        }

        private void TestVertical()
        {
            List<Slot> vertical = BoardUtils.GetNeighborsVertical(_board, _testRow, _testColumn, _testDistance);
            DebugSlotList("Vertical", vertical);
            _boardView.HighlightSlotList(vertical);
        }

        private void TestOrthogonal()
        {
            List<Slot> orthogonal = BoardUtils.GetNeighborsOrthogonal(_board, _testRow, _testColumn, _testDistance);
            DebugSlotList("Orthogonal", orthogonal);
            _boardView.HighlightSlotList(orthogonal);
        }

        private void TestDiagonal()
        {
            List<Slot> diagonal = BoardUtils.GetNeighborsDiagonal(_board, _testRow, _testColumn, _testDistance);
            DebugSlotList("Diagonal", diagonal);
            _boardView.HighlightSlotList(diagonal);
        }

        private void TestNeighbor()
        {
            List<Slot> neighbors = BoardUtils.GetNeighbors(_board, _testRow, _testColumn, _testDistance);
            DebugSlotList("Neighbor", neighbors);
            _boardView.HighlightSlotList(neighbors);
        }

        #endregion
    }
}