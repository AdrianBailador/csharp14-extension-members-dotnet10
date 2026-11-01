using ExtensionMembers.Domain;

namespace ExtensionMembers.Tests;

/// <summary>
/// Pins the exact behavior from the article's "What They Don't Tell You" section:
/// when a real instance member and an extension member share a name, the real member
/// wins — silently, with zero compiler warning. This test is the only thing that would
/// catch a regression if that resolution ever changed, because the build itself won't.
/// </summary>
public class ShadowingTests
{
    [Fact]
    public void Real_member_silently_wins_over_the_extension_member_with_the_same_name()
    {
        // 3 line items: the OLD extension rule (EU + >= 50) says eligible.
        // The NEW real member rule (EU + >= 50 + >= 2 items) also says eligible here —
        // so this case alone wouldn't catch the bug. See the next test.
        var order = new OrderV2("1", 75m, "EU", 3);

        Assert.True(order.IsEligibleForFreeShipping);
    }

    [Fact]
    public void Real_member_rule_differs_from_the_stale_extension_for_single_item_orders()
    {
        // 1 line item: the OLD extension rule (EU + >= 50) would say eligible.
        // The call below resolves to OrderV2's REAL member, not OrderV2Extensions —
        // so it correctly returns false under the current (updated) business rule.
        // If resolution ever silently flipped back to the extension, this is the
        // assertion that would catch it.
        var order = new OrderV2("1", 75m, "EU", 1);

        Assert.False(order.IsEligibleForFreeShipping);
    }
}
