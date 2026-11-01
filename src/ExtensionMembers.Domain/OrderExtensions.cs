namespace ExtensionMembers.Domain;

/// <summary>
/// Extension members for <see cref="Order"/>: the full C# 14 surface —
/// an instance property, an instance method, and a static factory method,
/// all declared on a type this class doesn't own.
/// </summary>
public static class OrderExtensions
{
    /// <summary>Defines extensions for <see cref="Order"/>.</summary>
    /// <param name="order">The order used as a receiver.</param>
    extension(Order order)
    {
        /// <summary>
        /// Whether this order qualifies for free shipping under the current promotion rule.
        /// </summary>
        public bool IsEligibleForFreeShipping => order.Total >= 50m && order.Region == "EU";

        /// <summary>Applies a percentage discount and returns a new order.</summary>
        public Order WithDiscount(decimal percent) =>
            order with { Total = order.Total * (1 - percent / 100m) };
    }

    /// <summary>Static extension members — called as <c>Order.Empty(...)</c>, not on an instance.</summary>
    extension(Order)
    {
        /// <summary>Creates a zero-value order for the given region, useful as a test fixture default.</summary>
        public static Order Empty(string region) => new("empty", 0m, region, 0);
    }
}
