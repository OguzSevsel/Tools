using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.Core.ServiceLocator
{
    public static class ServiceLocator
    {
        public static Dictionary<Type, object> Services { get; private set; } = new();

        public static void Register<T>(T service)
        {
            var type = typeof(T);

            if (IsRegistered<T>(type))
            {
                Debug.LogWarning($"{type.Name} already registered. Overwriting.");
                Services[type] = service;
                return;
            }

            Services.Add(type, service);
        }

        public static T Get<T>()
        {
            var type = typeof(T);

            if (!Services.TryGetValue(type, out var service))
            {
                throw new Exception($"Service of type {type.Name} is not registered");
            }

            return (T)service;
        }

        public static bool IsRegistered<T>(Type t)
        {
            return Services.ContainsKey(t);
        }

        public static void Unregister<T>()
        {
            Services.Remove(typeof(T));
        }

        public static void Clear()
        {
            Services.Clear();
        }
    }
}
