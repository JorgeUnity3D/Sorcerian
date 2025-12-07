using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Kapibara.ConnectSlots
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform _parent;

        [SerializeField] private float _sizeX = 1f;
        [SerializeField] private float _sizeY = 1f;

        [SerializeField] private float _moveDuration = 0.5f;
        [SerializeField] private float _stepDelay = 0.5f;

        private Slot[,] _board;

        #region BOARD BUILDING

        public void GenerateBoardView(Slot[,] board, UnityAction<Slot> onSlotClick)
        {
            _board = board;
            int rows = _board.GetLength(0);
            int cols = _board.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    InstantiateSlot(_board[r, c], onSlotClick);
                }
            }
        }

        private void InstantiateSlot(Slot slot, UnityAction<Slot> onSlotClick)
        {
            GameObject instance = Instantiate(_slotPrefab, _parent);
            instance.transform.localScale = new Vector3(_sizeX, _sizeY, 1f);

            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            sr.sprite = slot.Sprite;

            // Calculate sprite size
            Vector2 spriteSize = sr.sprite.bounds.size;
            Vector2 scaledSize = new Vector2(spriteSize.x * _sizeX, spriteSize.y * _sizeY);

            // Position in grid
            instance.transform.localPosition = CalculatePosition(slot.Row, slot.Column, scaledSize);

            // Collider
            BoxCollider2D col = instance.GetComponent<BoxCollider2D>();
            if (col == null) col = instance.AddComponent<BoxCollider2D>();
            col.size = scaledSize;
            col.offset = Vector2.zero;
            col.isTrigger = true;

            // SlotView
            SlotView view = instance.GetComponent<SlotView>();
            view.Init(slot, onSlotClick);

            slot.SlotInstance = instance;
        }

        private Vector3 CalculatePosition(int row, int column, Vector2 scaledSize)
        {
            //Vector3 nextPos = _board[nextRow, slot.Column].SlotInstance.transform.localPosition;
            // Position in grid
            return new Vector3(column * scaledSize.x, -row * scaledSize.y, 0);
        }

        public void RegenerateEmptySlots(List<SlotMovement> movements, UnityAction<Slot> onSlotClick)
        {
            foreach (SlotMovement movement in movements)
            {
                InstantiateSlot(movement.Slot, onSlotClick);
            }
        }

        #endregion

        #region MOVEMENT

        public void SwapPositions(Slot slotA, Slot slotB, UnityAction<Slot, Slot> OnSwapComplete)
        {
            Vector3 positionA = slotA.SlotInstance.transform.localPosition;
            Vector3 positionB = slotB.SlotInstance.transform.localPosition;

            slotA.SlotInstance.transform.DOLocalMove(positionB, 0.5f)
                .SetEase(Ease.OutQuad);
            slotB.SlotInstance.transform.DOLocalMove(positionA, 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => { OnSwapComplete?.Invoke(slotA, slotB); });
        }

        public void ApplyMovementList(List<SlotMovement> slotMovements, UnityAction OnComplete = null)
        {
            StartCoroutine(CRApplyMovementList(slotMovements, OnComplete));
        }

        private IEnumerator CRApplyMovementList(List<SlotMovement> slotMovements, UnityAction OnComplete)
        {
            List<Sequence> sequences = new List<Sequence>();

            int rows = _board.GetLength(0);

            foreach (SlotMovement movement in slotMovements)
            {
                Slot slot = movement.Slot;

                int currentRow = slot.Row;
                int targetRow = movement.TargetRow;

                if (currentRow == targetRow)
                    continue;

                int stepDir = targetRow > currentRow ? 1 : -1;
                int steps = Mathf.Abs(targetRow - currentRow);

                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < steps; i++)
                {
                    int nextRow = currentRow + stepDir;

                    Sprite sprite = slot.SlotInstance.GetComponent<SpriteRenderer>().sprite;
                    Vector2 scaledSize = new Vector2(sprite.bounds.size.x * _sizeX, sprite.bounds.size.y * _sizeY);
                    Vector3 nextPos = CalculatePosition(nextRow, slot.Column, scaledSize);

                    sequence.Append(
                        slot.SlotInstance.transform.DOLocalMove(nextPos, _moveDuration).SetEase(Ease.OutQuad)
                    );

                    if (i < steps - 1)
                        sequence.AppendInterval(_stepDelay);

                    currentRow = nextRow;
                }

                sequences.Add(sequence);
            }

            foreach (Sequence seq in sequences)
            {
                seq.Play();
            }

            foreach (Sequence seq in sequences)
            {
                yield return seq.WaitForCompletion();
            }

            OnComplete?.Invoke();
        }

        #endregion

        #region HIGHLIGHTS

        public void HighlightSlot(Slot slot)
        {
            slot.SlotView.Highlight(true);
        }

        public void HighlightSlotList(List<Slot> slots)
        {
            ClearAllHighlights();
            foreach (Slot slot in slots)
            {
                slot.SlotView.Highlight(true);
            }
        }

        public void ClearHighlight(Slot slot)
        {
            slot.SlotView.Highlight(false);
        }

        public void ClearHighlights(Slot slotA, Slot slotB)
        {
            slotA.SlotView.Highlight(false);
            slotB.SlotView.Highlight(false);
        }

        public void ClearHighlights(List<Slot> slots)
        {
            foreach (Slot slot in slots)
            {
                slot.SlotView.Highlight(false);
            }
        }

        public void ClearAllHighlights()
        {
            for (var r = 0; r < _board.GetLength(0); r++)
            for (var c = 0; c < _board.GetLength(1); c++)
            {
                Slot slot = _board[r, c];
                if (slot != null)
                {
                    slot.SlotView.Highlight(false);
                }
            }
        }

        #endregion

        #region DESTRUCTIONS

        public void DestroySlot(Slot slot, UnityAction<Slot> OnDestroyAnimationComplete)
        {
            slot.SlotInstance.transform.DOShakeScale(0.35f, Random.Range(1, 3)).SetEase(Ease.OutQuad);
            slot.SlotInstance.transform.DOScale(0, 0.35f).SetEase(Ease.OutQuad)
                .OnComplete(() => OnDestroyAnimationComplete?.Invoke(slot));
        }

        #endregion
    }
}