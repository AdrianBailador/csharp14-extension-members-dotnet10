namespace ExtensionMembers.Domain;

/// <summary>
/// The "after" shape: a teammate, working in a completely unrelated PR, promoted
/// <c>IsEligibleForFreeShipping</c> into a real member directly on the type — because the
/// business rule grew a third condition (<see cref="LineItemCount"/>) that the original
/// extension property in <see cref="OrderV2Extensions"/> was never updated to include.
/// Nobody touched or even looked at <see cref="OrderV2Extensions"/> while doing this.
/// </summary>
public sealed record OrderV2(string Id, decimal Total, string Region, int LineItemCount)
{
    /// <summary>The real, current business rule: EU, over the threshold, AND at least 2 line items.</summary>
    public bool IsEligibleForFreeShipping => Total >= 50m && Region == "EU" && LineItemCount >= 2;
}

/// <summary>
/// The original extension — written before <see cref="OrderV2"/> grew a real member with
/// the same name. Left in place, unchanged, because nothing ever told anyone it needed updating.
/// </summary>
public static class OrderV2Extensions
{
    extension(OrderV2 order)
    {
        /// <summary>The old rule: EU and over the threshold. Missing the line-item condition.</summary>
        public bool IsEligibleForFreeShipping => order.Total >= 50m && order.Region == "EU";
    }
}
