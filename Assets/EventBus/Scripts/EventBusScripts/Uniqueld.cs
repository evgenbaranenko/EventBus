using System;

namespace EventBus.Scripts.EventBusScripts
{
    public record UniqueId
    {
        // Guid.NewGuid().ToString() - это вызов статического метода NewGuid() из класса Guid,
        // который генерирует новый уникальный идентификатор типа Guid, а затем преобразует его в строковое представление
        public string Id => _id ??= Guid.NewGuid().ToString();
        private string _id;

        //Таким образом, код выше гарантирует, что свойство Id всегда будет иметь уникальное значение типа string,
        //если _id еще не был инициализирован. При первом обращении к свойству Id, если _id равен null,
        //то он будет инициализирован новым уникальным идентификатором. В дальнейшем, при обращении к свойству Id,
        //будет возвращено уже существующее значение _id.


        // Этот код определяет неявное преобразование типа UniqueId в тип string
        // с помощью оператора неявного преобразования (implicit operator).
        public static implicit operator string(UniqueId uniqueId) => uniqueId.Id;
    }
}