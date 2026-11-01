namespace ExtensionMembers.Domain;

/// <summary>
/// The "before" shape: a plain data record with no domain behavior.
/// Mirrors a type you might not fully own — a DTO generated from a contract,
/// or an entity another team maintains.
/// </summary>
public sealed record Order(string Id, decimal Total, string Region, int LineItemCount);
