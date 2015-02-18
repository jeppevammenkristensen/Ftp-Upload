using System;
using System.Collections.Generic;
using System.Windows.Documents.DocumentStructures;
using Upload.Annotations;

namespace Upload.Messages
{
    public class MessageBus
    {
        private static readonly Dictionary<Type, List<Action<Object>>> _subscribers = new Dictionary<Type,List<Action<Object>>>();

        public void Publish<T>(T message) where T : class
        {
            if (!_subscribers.ContainsKey(typeof (T)))
                return;

            foreach (var action in _subscribers[typeof(T)])
            {
                action(message);
            }
        }

        public void Subscribe<T>(Action<T> callback) where T : class 
        {
            var type = typeof (T);
            var list = new List<Action<Object>>();

            if (!_subscribers.ContainsKey(type))
            {
                _subscribers.Add(type,list);
            }

            list.Add(obj => callback(obj as T));
        }

        public void Unsubscribe<T>(Action<T> callback) where T : class 
        {
            throw new NotImplementedException();
        }
    }
}