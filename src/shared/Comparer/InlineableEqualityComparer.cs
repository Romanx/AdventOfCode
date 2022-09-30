using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Inline capable generic equality comparer
/// </summary>
/// <typeparam name="T">The type to compare for equality</typeparam>
public class InlineableEqualityComparer<T> : EqualityComparer<T>
{
    /// <summary>
    /// Type specific equality function
    /// </summary>
    private readonly Func<T?, T?, bool> equalityFunction;

    /// <summary>
    /// Type specific hash function
    /// </summary>
    private readonly Func<T?, int> hashFunction;

    /// <summary>
    /// Creates a new instance of an equality comparer using the provided functions
    /// </summary>
    /// <param name="equalityFunction">The equality function</param>
    /// <param name="hashFunction">The hash function</param>
    public InlineableEqualityComparer(Func<T?, T?, bool> equalityFunction, Func<T?, int> hashFunction)
    {
        this.equalityFunction = equalityFunction ?? throw new ArgumentNullException(nameof(equalityFunction));
        this.hashFunction = hashFunction ?? throw new ArgumentNullException(nameof(hashFunction));
    }

    /// <inheritdoc/>
    public override bool Equals(T? x, T? y) => this.equalityFunction(x, y);

    /// <inheritdoc/>
    public override int GetHashCode(T? obj) => this.hashFunction(obj);


    /// <summary>
    /// Creates an equality comparer using the provided functions
    /// </summary>
    /// <param name="equalityFunction">The equality function</param>
    /// <param name="hashFunction">The hash function</param>
    /// <returns>The new comparer</returns>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Same pattern as .NET's Comparer<T>.Create(Comparison<T>) Method")]
    public static IEqualityComparer<T> Create(Func<T?, T?, bool> equalityFunction, Func<T?, int> hashFunction) =>
        new InlineableEqualityComparer<T>(equalityFunction, hashFunction);
}
