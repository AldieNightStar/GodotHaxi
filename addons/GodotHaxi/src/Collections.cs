using System;
using System.Collections.Generic;

namespace GodotHaxi;

public class CollectionUtil
{
    public static Dictionary<K, T> Assoc<T, K>(IEnumerable<T> items, Func<T, K> func)
    {
        Dictionary<K, T> dict = new();
        foreach (var item in items)
        {
            var key = func(item);
            if (key == null) continue;

            dict[key] = item;
        }

        return dict;
    }

    public static ISet<K> SetOf<T, K>(IEnumerable<T> items, Func<T, K> func)
    {
        HashSet<K> set = new();
        foreach (var item in items)
        {
            var value = func(item);
            if (value == null) continue;
            set.Add(value);
        }
        return set;
    }
}