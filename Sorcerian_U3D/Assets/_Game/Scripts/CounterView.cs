using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kapibara.ConnectSlots
{
    public class CounterView : MonoBehaviour
    {
        [SerializeField] private SlotType _slotType;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        public void AddCount(int count)
        {
            _text.text += count.ToString("N2");
        }
    }
}