using ExtensionMembers.Domain;

namespace ExtensionMembers.Tests;

public class ExtensionUsageTests
{
    [Fact]
    public void Instance_extension_property_reads_like_a_real_property()
    {
        var order = new Order("1", 75m, "EU", 3);

        Assert.True(order.IsEligibleForFreeShipping);
    }

    [Fact]
    public void Instance_extension_method_returns_a_transformed_value()
    {
        var order = new Order("1", 100m, "EU", 1);

        var discounted = order.WithDiscount(10);

        Assert.Equal(90m, discounted.Total);
    }

    [Fact]
    public void Static_extension_member_is_called_on_the_type_not_an_instance()
    {
        // This is the call shape C# 14 introduces: Order.Empty(...), exactly as if
        // Order declared a real static factory method — nothing at the call site
        // distinguishes it from one.
        var order = Order.Empty("US");

        Assert.Equal(0m, order.Total);
        Assert.Equal("US", order.Region);
    }
}
