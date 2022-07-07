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

            dictionaryValue = exists
                ? updateValueFactory(key, dictionaryValue!)
                : addValue;

            return dictionaryValue;
        }

        public static TValue AddOrUpdate<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Func<TKey, TValue, TValue> updateValueFactory)
            where TKey : notnull
        {
            ref var dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

            dictionaryValue = exists
                ? updateValueFactory(key, dictionaryValue!)
                : addValueFactory(key);

            return dictionaryValue;
        }

        public static TValue AddOrUpdate<TKey, TValue, TState>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TState, TValue> addValueFactory,
            Func<TKey, TValue, TState, TValue> updateValueFactory,
            TState state)
            where TKey : notnull
        {
            ref var dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

            dictionaryValue = exists
                ? updateValueFactory(key, dictionaryValue!, state)
                : addValueFactory(key, state);

            return dictionaryValue;
        }
    }
}
