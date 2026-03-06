using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Kapibara.ConnectSlots
{
    public class BoardView : MonoBehaviour
    {
        [Inject] private Board _board;
        [Inject] private BoardConfig _boardConfig;
        [Inject] private ManaPrefabs _manaPrefabs;

        #region BOARD BUILDING

        public void GenerateBoardView(UnityAction<Slot> onSlotClick)
        {
            int rows = _board.Rows;
            int cols = _board.Columns;

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
            GameObject instance = Instantiate(_boardConfig.SlotPrefab, _boardConfig.SlotsParent);
            instance.transform.localScale = new Vector3(_boardConfig.SizeX, _boardConfig.SizeY, 1f);

            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            sr.sprite = slot.Sprite;

            // Calculate sprite size
            Vector2 spriteSize = sr.sprite.bounds.size;
            Vector2 scaledSize = new Vector2(spriteSize.x * _boardConfig.SizeX, spriteSize.y * _boardConfig.SizeY);

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

            int rows = _board.Rows;

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
                    Vector2 scaledSize = new Vector2(sprite.bounds.size.x * _boardConfig.SizeX,
                        sprite.bounds.size.y * _boardConfig.SizeY);
                    Vector3 nextPos = CalculatePosition(nextRow, slot.Column, scaledSize);

                    sequence.Append(
                        slot.SlotInstance.transform.DOLocalMove(nextPos, _boardConfig.MoveDuration)
                            .SetEase(Ease.OutQuad)
                    );

                    if (i < steps - 1)
                        sequence.AppendInterval(_boardConfig.StepDelay);

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
            for (var r = 0; r < _board.Rows; r++)
            for (var c = 0; c < _board.Columns; c++)
            {
                Slot slot = _board[r, c];
                if (slot != null)
                {
                    slot.SlotView.Highlight(false);
                }
            }
        }

        #endregion

        #region ANIMATIONS

        public void ShakeSlots(List<Slot> slots, int repetitions = 2)
        {
            foreach (var slot in slots)
            {
                if (slot?.SlotInstance == null)
                    continue;

                var t = slot.SlotInstance.transform;
                Sequence seq = DOTween.Sequence();

                float shakeDistance = 0.12f;
                float durationTotal = 0.35f;  // la duración total que quieras
                float stepDuration = durationTotal / (repetitions * 3f);

                Vector3 originalPos = t.localPosition;

                Sequence gesture = DOTween.Sequence();
                for (int i = 0; i < repetitions; i++)
                {
                    gesture.Append(t.DOLocalMoveX(originalPos.x + shakeDistance, stepDuration).SetEase(Ease.OutQuad))
                        .Append(t.DOLocalMoveX(originalPos.x - shakeDistance, stepDuration).SetEase(Ease.OutQuad))
                        .Append(t.DOLocalMoveX(originalPos.x, stepDuration * 0.5f).SetEase(Ease.OutQuad));
                }

                gesture.Join(t.DOLocalRotate(new Vector3(0, 0, 10f), durationTotal).SetEase(Ease.OutQuad))
                    .Append(t.DOLocalRotate(new Vector3(0, 0, -10f), durationTotal).SetEase(Ease.OutQuad))
                    .Append(t.DOLocalRotate(Vector3.zero, durationTotal * 0.5f).SetEase(Ease.OutQuad));

                seq.Append(gesture)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        t.localPosition = originalPos;
                        t.localRotation = Quaternion.identity;
                    });
            }
        }

        public void PlayManaArcAnimation(Slot slot, ManaCounterView manaCounterView, Transform parent)
        {
            if (slot?.SlotInstance == null || manaCounterView == null)
                return;

            // WORLD POS start
            Vector3 startPos = slot.SlotInstance.transform.position;

            // Convert UI rect to world (Screen Space Overlay compatible)
            RectTransform targetRect = manaCounterView.GetComponent<RectTransform>();
            Vector3 endPos = targetRect.ConvertToWorldspacePositionCamera(manaCounterView.GetComponentInParent<Canvas>());

            // Instantiate ORB PREFAB from ScriptableObject
            GameObject prefab = _manaPrefabs[slot.SlotType];
            if (prefab == null)
            {
                Debug.LogWarning($"[Mana Orb] No prefab found for {slot.SlotType}");
                return;
            }

            GameObject orb = Instantiate(prefab, startPos, Quaternion.identity);
            orb.transform.SetParent(parent);
            
            // ARC Path
            Vector3 mid = (startPos + endPos) / 2f;
            mid.y += 1.75f; // visible arc

            Vector3[] path = { startPos, mid, endPos };

            orb.transform.DOPath(path, 0.45f, PathType.CatmullRom)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    // OPTIONAL: pop animation on arrival
                    orb.transform.DOScale(0f, 0.15f).SetEase(Ease.InBack);
                    Destroy(orb, 0.15f);
                });
        }

        #endregion

        #region DESTRUCTIONS

        public void DestroySlotAnimation(Slot slot, int totalSlots, UnityAction<Slot> OnDestroyAnimationComplete)
        {
            // slot.SlotInstance.transform.DOShakeScale(0.35f, Random.Range(3, 5)).SetEase(Ease.OutQuad);
            // slot.SlotInstance.transform.DOScale(0, 0.35f).SetEase(Ease.OutQuad)
            //     .OnComplete(() => OnDestroyAnimationComplete?.Invoke(slot));
            if (slot?.SlotInstance == null)
                return;

            Transform t = slot.SlotInstance.transform;

            float baseDuration = 0.35f;
            float intensity = Mathf.Clamp01((totalSlots - 3) / 7f); // 0 → suave, 1 → potente

            float squashAmount = Mathf.Lerp(0.15f, 0.35f, intensity);
            float shakePower = Mathf.Lerp(2f, 8f, intensity);
            float preEffectDuration = Mathf.Lerp(0.12f, 0.22f, intensity);

            Sequence seq = DOTween.Sequence();

            // 1) Squash & Stretch inicial (impacto cartoon)
            seq.Append(
                t.DOScale(new Vector3(1 + squashAmount, 1 - squashAmount, 1), preEffectDuration)
                    .SetEase(Ease.OutQuad)
            );

            // 2) Pequeña vibración horizontal según intensidad
            seq.Join(
                t.DOShakePosition(preEffectDuration, strength: shakePower * 0.05f, vibrato: 10, randomness: 90)
            );

            // 3) Vuelta suave antes de desaparecer
            seq.Append(
                t.DOScale(new Vector3(1 - squashAmount * 0.5f, 1 + squashAmount * 0.5f, 1), preEffectDuration)
                    .SetEase(Ease.OutQuad)
            );

            // 4) Efecto final: scale to 0 (tu efecto original mejorado)
            seq.Append(
                t.DOScale(0, baseDuration)
                    .SetEase(Ease.InBack) // más elegante que OutQuad para desaparecer
            );

            seq.OnComplete(() =>
            {
                OnDestroyAnimationComplete?.Invoke(slot);
            });
        }

        public void DestroySlots()
        {
            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Columns; c++)
                {
                    if (_board[r, c].IsDestroyed)
                    {
                        Destroy(_board[r, c].SlotInstance);
                        _board[r, c] = null;
                    }
                }
            }
        }

        #endregion
    }
}