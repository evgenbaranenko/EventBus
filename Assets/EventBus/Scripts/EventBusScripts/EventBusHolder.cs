// тут создаем шину событий в методе Awake в этом классе заглушке 

using UnityEngine;

namespace EventBus.Scripts.EventBusScripts
{
    public class EventBusHolder : MonoBehaviour
    {
        public EventBus EventBus { get; private set; }

        private void Awake() => EventBus = new EventBus();
    }
}