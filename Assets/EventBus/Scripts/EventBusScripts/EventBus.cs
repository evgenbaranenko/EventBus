using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Scripts.EventBusScripts
{
    // шина событий 
    
    public class EventBus
    {
        #region fields

        // создаем словарь ключом для которого будут типы событий (разные события),
        // а в качестве значения список подписчиков 

        #region что такое WeakReference

        // WeakReference в C# представляет класс, который позволяет создавать слабые ссылки на 
        // Слабая ссылка (weak reference) представляет ссылку на объект, которая не увеличивает счетчик ссылок этого объекта.
        // То есть, объект, на который указывает слабая ссылка, может быть собранным сборщиком мусора,
        // если больше нет сильных ссылок на него.

        #endregion

        // самих подписчиков мы будем представлять базовым интерфейсом обернутым в WeakReference

        private readonly Dictionary<Type, List<WeakReference<IBaseEventReceiver>>> _receivers;
        private readonly Dictionary<string, WeakReference<IBaseEventReceiver>> _receiverHashToReference;

        public EventBus() // инициализация словарей. Обертываем в WeakReference
        {
            _receivers = new Dictionary<Type, List<WeakReference<IBaseEventReceiver>>>();
            _receiverHashToReference = new Dictionary<string, WeakReference<IBaseEventReceiver>>();
        }

        #endregion

        #region public metods

        // метод регистрации 
        public void Register<T>(IEventReceiver<T> receiver) where T : struct, IEvent
        {
            Type eventType = typeof(T); // получаем тип события

            #region что такое ContainsKey

            // ContainsKey - это метод, используемый для проверки наличия указанного ключа в коллекции,
            // реализующей интерфейс IDictionary<TKey, TValue>
            // Он возвращает булевое значение true, если ключ присутствует в коллекции, false - ключ отсутствует

            #endregion

            // если мы еще не создали то добавляем в список пользователей по данному типу
            if (!_receivers.ContainsKey(eventType))
                _receivers[eventType] = new List<WeakReference<IBaseEventReceiver>>();

            #region что такое TryGetValue

            // TryGetValue - это метод, используемый для получения значения, связанного с указанным ключом,
            // из коллекции, реализующей интерфейс IDictionary<TKey, TValue>
            // Он возвращает булевое значение, указывающее, удалось ли успешно получить значение по ключу

            #endregion

            if (!_receiverHashToReference.TryGetValue(receiver.Id, out WeakReference<IBaseEventReceiver> reference))
            {
                reference = new WeakReference<IBaseEventReceiver>(receiver);
                _receiverHashToReference[receiver.Id] = reference;
            }

            _receivers[eventType].Add(reference); // добавляем в словарь 

            #region старое

            // // создаем экземпляр интерфейса обернутого в слабую ссылку
            // WeakReference<IBaseEventReceiver> reference = new WeakReference<IBaseEventReceiver>(receiver);
            //
            //
            // // добавляем в словарь ссылку на лист по хешу объекта пользователя
            // _receiverHashToReference[receiver.GetHashCode()] = reference;

            #endregion
        }

        // метод отписки
        public void Unregister<T>(IEventReceiver<T> receiver) where T : struct, IEvent
        {
            // получаем тип события 
            Type eventType = typeof(T);

            // проверяем наличие подписчиков в словаре
            if (!_receivers.ContainsKey(eventType) || !_receiverHashToReference.ContainsKey(receiver.Id)) return;

            // тут нужно было бы получить ссылки
            // но что бы не перебирать все ссылки мы заранее можем задать их по хешу объекта слушателя 
            WeakReference<IBaseEventReceiver> reference = _receiverHashToReference[receiver.Id];

            _receivers[eventType].Remove(reference); // удаляем со словаря List нужного типа 

            // выполняется подсчет количества объектов, на которые ссылаются слабые ссылки _receivers
            // _receivers.SelectMany(x => x.Value) объединяет все списки слабых ссылок в один плоский список
            // Затем .Count(x => x == reference) применяется к полученному списку и считает количество элементов, которые равны reference
            // Здесь reference представляет собой некоторую слабую ссылку, с которой выполняется сравнение

            int weakRefCount =
                _receivers.SelectMany(x => x.Value).Count(x => x == reference);

            if (weakRefCount == 0)
                _receiverHashToReference.Remove(receiver.Id); // удаляем со словаря номер (инт) хеша объекта слушателя
        }

        // метод вызова события 
        public void Raise<T>(T @event) where T : struct, IEvent
        {
            Type eventType = typeof(T); // получаем тип

            if (!_receivers.ContainsKey(eventType)) return; // если в словаре нету выходим из метода

            // пробегаем по списку подписчиков и если объект еще не удален то вызываем событие с ним 

            #region способ с циклом foreach

            /*// способ с циклом foreach
            foreach (WeakReference<IBaseEventReceiver> reference in _receivers[eventType])
            {
                //TryGetTarget - это метод, используемый для получения целевого объекта из слабой ссылки (WeakReference).
                //Он позволяет безопасно получить объект из слабой ссылки,
                //не вызывая исключение, даже если объект уже был собран сборщиком мусора.

                if (reference.TryGetTarget(out IBaseEventReceiver receiver))
                {
                    ((IEventReceiver<T>)receiver).OnEvent(@event); // вызываем событие
                }
            }*/

            #endregion

            #region comments: почему тут используется цикл for

            // альтернативный способ с циклом for  
            // потому что: При отписке или удалении объекта (способ с циклом foreach) во время вызова Raise будет ошибка,
            // потому-что foreach не позволяет изменять размер списка.
            // Без обратного направления ошибки не будет, но будет выходить из цикла если объект удалился.
            // Из-за этого вызов евента будет срабатывать только на одном объекте из списка

            #endregion

            #region что такое TryGetTarget

            //TryGetTarget - это метод, используемый для попытки получения целевого объекта из слабой ссылки (WeakReference).
            //Он проверяет, существует ли еще объект, на который ссылается слабая ссылка, и возвращает результат в виде булевого значения.

            #endregion

            List<WeakReference<IBaseEventReceiver>> references = _receivers[eventType];

            for (int i = references.Count - 1; i >= 0; i--)
            {
                if (references[i].TryGetTarget(out IBaseEventReceiver receiver))
                    ((IEventReceiver<T>)receiver).OnEvent(@event); // вызываем событие
            }
        }

        #endregion
    }
}