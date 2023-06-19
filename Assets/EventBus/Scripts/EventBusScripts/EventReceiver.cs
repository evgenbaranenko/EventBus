// класс приемщик 
// именно его вешаем на префаб 

using UnityEngine;

namespace EventBus.Scripts.EventBusScripts
{
    [RequireComponent(typeof(MeshRenderer))]
    public class EventReceiver : MonoBehaviour, IEventReceiver<RedEvent>, IEventReceiver<GreenEvent>,
        IEventReceiver<BlueEvent>
    {
        #region fields

        [SerializeField] private EventBusHolder _eventBusHolder;
        private MeshRenderer _meshRenderer;

        #endregion

        #region engine methods

        // в методе Start регестрируем события 
        // так делается только для примера 
        private void OnEnable()
        {
            _eventBusHolder.EventBus.Register(this as IEventReceiver<RedEvent>);
            _eventBusHolder.EventBus.Register(this as IEventReceiver<GreenEvent>);
            _eventBusHolder.EventBus.Register(this as IEventReceiver<BlueEvent>);

            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnDestroy() => _meshRenderer.sharedMaterial.color = Color.white;

        private void OnDisable()
        {
            _eventBusHolder.EventBus.Unregister(this as IEventReceiver<RedEvent>);
            _eventBusHolder.EventBus.Unregister(this as IEventReceiver<GreenEvent>);
            _eventBusHolder.EventBus.Unregister(this as IEventReceiver<BlueEvent>);
        }

        #endregion

        #region IEventReciver

        public string Id { get; set; } = new UniqueId();

        // подписываем на событие 
        public void OnEvent(RedEvent @event)
        {
            transform.position += @event.MoveDelta;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        // подписываем на событие 
        public void OnEvent(GreenEvent @event) => transform.localScale += @event.Scale;

        // подписываем на событие 
        public void OnEvent(BlueEvent @event) => _meshRenderer.sharedMaterial.color = @event.Color;

        #endregion
    }
}