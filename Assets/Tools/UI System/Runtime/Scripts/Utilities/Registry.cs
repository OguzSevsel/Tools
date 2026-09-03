using System.Collections.Generic;
using Elements;

namespace Utilities
{
    public static class Registry
    {
        private static Dictionary<string, Element> elements
            = new Dictionary<string, Element>();

        public static void Subscribe(string id, Element element)
        {
            elements.Add(id, element);
        }

        public static T Get<T>(string id) where T : Element
        {
            elements.TryGetValue(id, out var element);
                return element as T;
        }

        public static void Unsubscribe(string id)
        {
            elements.Remove(id);
        }
    }
}