using System.Collections.Generic;

namespace Utilities
{
    /// <summary>
    /// 内存数据存储
    /// </summary>
    public static class Store
    {
        private static Dictionary<object, object> _store = new Dictionary<object, object>();

        public static bool Set<T>(object key, T data)
        {
            if (_store.ContainsKey(key)) _store[key] = data;
            else _store.Add(key, data);
            return true;
        }

        public static T Get<T>(object name)
        {
            if (_store.ContainsKey(name))
                if (_store[name] is T data)
                    return data;
            return default;
        }

        public static bool Remove(object name) => _store.Remove(name);

        public static void Clear() => _store.Clear();
    }
}