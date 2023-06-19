namespace EventBus.Scripts.EventBusScripts
{
    // Пустой интерфейс для маркировки событий
    public interface IEvent
    {
    }

    // Базовый интерфейс для слушателей событий 
    public interface IBaseEventReceiver
    {
    }

    // Интерфейс для параметризованных событий 
    public interface IEventReceiver<T> : IBaseEventReceiver where T : struct, IEvent
    {
        #region зачем нужен @ перед event

        //Знак @ перед ключевым словом event используется для экранирования,
        //так как event является зарезервированным ключевым словом в C#.

        #endregion
        void OnEvent(T @event);
        string Id { get; set; }
    }
}