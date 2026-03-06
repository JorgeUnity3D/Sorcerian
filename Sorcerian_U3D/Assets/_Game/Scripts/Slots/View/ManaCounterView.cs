using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kapibara.ConnectSlots
{
    public class ManaCounterView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;
        
        public void UpdateCount(int count)
        {
            _text.text = count.ToString("D2");
        }
    }
}