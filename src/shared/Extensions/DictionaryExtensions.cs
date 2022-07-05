using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static TValue AddOrUpdate<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue addValue,
            Func<TKey, TValue, TValue> updateValueFactory)
            where TKey : notnull
        {
            ref var dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

            if (exists)
            {
                dictionaryValue = updateValueFactory(key, dictionaryValue!);
            }
            else
            {
                dictionaryValue = addValue;
            }

            return dictionaryValue;
        }
    }
}
