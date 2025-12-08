using UnityEngine;

namespace Kapibara.ConnectSlots
{
    public class BaseScriptableData<T> : ScriptableObject where T : class
    {
        public T Data;
    }
}