namespace UI.Template.Models;

/// <summary>
/// Simple model that represents a store product with name, price and stock status.
/// Provides equality/comparison helpers intended for assertions in tests.
/// </summary>
public sealed class Product(string name, decimal price, int stock) : IEquatable<Product>
{
    public string Name { get; init; } = name ?? string.Empty;
    public decimal Price { get; init; } = price;
    public int Stock { get; init; } = stock;

    /// <summary>
    /// Determines whether the specified <see cref="Product"/> is equal to the current instance.
    /// Equality compares normalized <see cref="Name"/>, exact <see cref="Price"/>, and <see cref="Stock"/>.
    /// </summary>
    /// <param name="other">The other product to compare with this instance.</param>
    /// <returns><c>true</c> if the products are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(Product? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return string.Equals(Normalize(Name), Normalize(other.Name), StringComparison.OrdinalIgnoreCase)
               && Price == other.Price
               && Stock == other.Stock;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to the current instance.
    /// Overrides <see cref="object.Equals(object)"/> and delegates to <see cref="Equals(Product?)"/> when possible.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => Equals(obj as Product);

    /// <summary>
    /// Returns a hash code for this instance.
    /// Combines normalized name (lowercase), price and stock to produce a stable hash code suitable for hashing collections.
    /// </summary>
    /// <returns>An integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Normalize(Name)?.ToLowerInvariant(), Price, Stock);
    }

    /// <summary>
    /// Checks equality between two <see cref="Product"/> instances.
    /// </summary>
    public static bool operator ==(Product? left, Product? right) => EqualityComparer<Product>.Default.Equals(left, right);

    /// <summary>
    /// Checks inequality between two <see cref="Product"/> instances.
    /// </summary>
    public static bool operator !=(Product? left, Product? right) => !(left == right);

    /// <summary>
    /// Returns a human-readable string representation of the product.
    /// </summary>
    /// <returns>A string representation.</returns>
    public override string ToString()
    {
        return $"Product(Name='{Name}', Price={Price}, Stock='{Stock}')";
    }

    /// <summary>
    /// Determines whether the specified product is equivalent to this instance.
    /// This is a convenience wrapper around <see cref="Equals(Product?)"/> to make intent explicit in tests.
    /// </summary>
    /// <param name="other">The other product to compare.</param>
    /// <returns><c>true</c> if the products are equivalent; otherwise, <c>false</c>.</returns>
    public bool IsEquivalent(Product other) => Equals(other);

    /// <summary>
    /// Produces a sequence of textual differences between this product and <paramref name="other"/>.
    /// Each yielded string describes a field that differs and its values. Useful for assertion messages.
    /// </summary>
    /// <param name="other">The other product to compare to.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of difference messages. Empty if no differences are found.</returns>
    public IEnumerable<string> GetDifferences(Product other)
    {
        if (other is null)
        {
            yield return "Other product is null";
            yield break;
        }

        if (!string.Equals(Normalize(Name), Normalize(other.Name), StringComparison.OrdinalIgnoreCase))
            yield return $"Name: '{Name}' != '{other.Name}'";

        if (Price != other.Price)
            yield return $"Price: {Price} != {other.Price}";

        if (Stock != other.Stock)
            yield return $"Stock: {Stock} != {other.Stock}";
    }

    /// <summary>
    /// Normalizes a string for comparison by treating null as empty and trimming whitespace.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>A trimmed, non-null string.</returns>
    private static string Normalize(string? s) => (s ?? string.Empty).Trim();
}
