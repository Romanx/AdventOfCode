using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Shared
{
    public static class EnumHelpers
    {
        private static readonly Dictionary<Type, ImmutableDictionary<char, object>> _cache = new();

        public static T FromDisplayName<T>(char c)
            where T : struct
        {
            if (!typeof(T).IsEnum) { throw new InvalidOperationException("Type must be an enum"); }

            if (_cache.TryGetValue(typeof(T), out var items) is false)
            {
                items = AddNewEntry<T>();
            }

            return items.TryGetValue(c, out var e)
                ? (T)e
                : throw new InvalidOperationException($"Unable to find entry for {typeof(T).Name} with the display name of '{c}'");
        }

        public static char ToDisplayName<T>(T item)
            where T : struct
        {
            if (!typeof(T).IsEnum) { throw new InvalidOperationException("Type must be an enum"); }

            if (_cache.TryGetValue(typeof(T), out var items) is false)
            {
                items = AddNewEntry<T>();
            }

            var cachedItem = items
                .Where(i => ((T)i.Value).Equals(item))
                .ToArray();

            return cachedItem.Length == 1
                ? cachedItem[0].Key
                : throw new InvalidOperationException($"Unable to find entry for {typeof(T).Name} with the value of '{item}'");
        }

        private static ImmutableDictionary<char, object> AddNewEntry<T>() where T : struct
        {
            var members = typeof(T).GetMembers()
                .Select(m => (m.Name, Attribute: m.GetCustomAttribute<DisplayAttribute>()))
                .Where(m => m.Attribute is not null && m.Attribute.Name is not null);

            var builder = ImmutableDictionary.CreateBuilder<char, object>();
            foreach (var (name, attribute) in members)
            {
                builder.Add(attribute.Name![0], Enum.Parse(typeof(T), name));
            }

            var items = builder.ToImmutable();
            _cache.Add(typeof(T), items);
            return items;
        }
    }
}
