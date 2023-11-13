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
            TValue addValue,
            Action<TKey, TValue> updateValueFactory)
            where TKey : notnull
        {
            ref var dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

            if (exists)
            {
                updateValueFactory(key, dictionaryValue!);
            }
            else
            {
                dictionaryValue = addValue;
            }

            return dictionaryValue!;
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

        public static TValue AddOrUpdate<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> addValueFactory,
            Action<TKey, TValue> updateValueFactory)
            where TKey : notnull
        {
            ref var dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

            if (exists)
            {
                updateValueFactory(key, dictionaryValue!);
            }
            else
            {
                dictionaryValue = addValueFactory(key);
            }

            return dictionaryValue!;
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

        public static TValue AddOrUpdate<TKey, TValue, TState>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TState, TValue> addValueFactory,
            Action<TKey, TValue, TState> updateValueFactory,
            TState state)
            where TKey : notnull
        {
            ref var dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

            if (exists)
            {
                updateValueFactory(key, dictionaryValue!, state);
            }
            else
            {
                dictionaryValue = addValueFactory(key, state);
            }

            return dictionaryValue!;
        }

        public static ref TValue? GetOrAddValueRef<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key)
            where TKey : notnull
        => ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out _);

        public static TValue GetOrCalculateIfAbsent<TKey, TValue, TState>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TState, TValue> calculationFunc,
            TState state)
            where TKey : notnull
        {
            ref var dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
            if (exists is false)
            {
                dictionaryValue = calculationFunc(key, state);
            }

            return dictionaryValue!;
        }
    }
}
