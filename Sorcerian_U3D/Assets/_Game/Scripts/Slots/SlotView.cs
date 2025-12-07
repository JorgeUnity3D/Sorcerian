using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Kapibara.ConnectSlots
{
    public class SlotView : MonoBehaviour, IPointerClickHandler
    {
        private Slot _slot;
        private UnityAction<Slot> _onSlotClick;

        private SpriteRenderer _spriteRenderer;

        private bool _initialized = false;
        private float _elapsed;
        private float _wait = 0.25f;

        public void Init(Slot slot, UnityAction<Slot> onSlotClick)
        {
            _slot = slot;
            _onSlotClick = onSlotClick;

            _spriteRenderer = GetComponent<SpriteRenderer>();

            StartCoroutine(CRInitializeAndRandomFlip());
        }

        private IEnumerator CRInitializeAndRandomFlip()
        {
            int randomFlip = Random.Range(0, 2);
            _spriteRenderer.flipX = randomFlip == 0;
            float randomStart = Random.Range(0f, 0.5f);
            yield return new WaitForSeconds(randomStart);
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }
            
            FlipEverySeconds();
        }

        public void FlipEverySeconds()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _wait)
            {
                _elapsed = 0;
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
            }
        }

        public void Highlight(bool enabled)
        {
            if (_spriteRenderer == null) return;

            Color c = _spriteRenderer.color;
            c.a = enabled ? 0.5f : 1.0f;
            _spriteRenderer.color = c;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onSlotClick?.Invoke(_slot);
        }

        public void DestroySlot()
        {
            Destroy(gameObject);
        }
    }
}