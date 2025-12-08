using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace Kapibara.ConnectSlots
{
    public class BoardDebugger
    {
        [Inject] private Board _board;
        
        #region DEBUG

        public void DebugBoard(string title)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"--- <b><color=#FFD700>{title}</color></b> ---");

            // ============================================================
            // 1. PREPARAR TABLA DE TIPOS (sin color, para medir correctamente)
            // ============================================================

            string[,] typeTable = new string[_board.Rows, _board.Columns];
            int[] typeColWidths = new int[_board.Columns];

            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Columns; c++)
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

            string[,] posTable = new string[_board.Rows, _board.Columns];
            int[] posColWidths = new int[_board.Columns];

            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Columns; c++)
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

            int totalTypeWidth = typeColWidths.Sum() + _board.Columns * 3 + 1;
            sb.AppendLine("+" + new string('-', totalTypeWidth) + "+");

            for (int r = 0; r < _board.Rows; r++)
            {
                sb.Append("|");
                for (int c = 0; c < _board.Columns; c++)
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

            int totalPosWidth = posColWidths.Sum() + _board.Columns * 3 + 1;
            sb.AppendLine("+" + new string('-', totalPosWidth) + "+");

            for (int r = 0; r < _board.Rows; r++)
            {
                sb.Append("|");
                for (int c = 0; c < _board.Columns; c++)
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

        public void DebugSlotList(string title, List<Slot> slotList)
        {
            string result = $"· {title}\n";
            for (int i = 0; i < slotList.Count; i++)
            {
                result += $"    ({slotList[i]})\n";
            }

            Debug.Log(result);
        }

        public void DebugSlotMovementList(string title, List<SlotMovement> slotMovementList)
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
    }
}