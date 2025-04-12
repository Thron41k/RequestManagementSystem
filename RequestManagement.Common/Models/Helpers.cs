namespace RequestManagement.Common.Models;

public static class Helpers
{
    public record QuantityFilter(decimal Value, ComparisonOperator Operator)
    {
        private static QuantityFilter GreaterThan(decimal value) => new(value, ComparisonOperator.GreaterThan);
        private static QuantityFilter EqualTo(decimal value) => new(value, ComparisonOperator.EqualTo);
        private static QuantityFilter LessThan(decimal value) => new(value, ComparisonOperator.LessThan);
        public static QuantityFilter? GetQuantityFilter(decimal value, int type) => type switch { 1 => EqualTo(value), 2 => GreaterThan(value), 3 => LessThan(value), _ => null };
    }

    public enum ComparisonOperator
    {
        GreaterThan,
        EqualTo,
        LessThan
    }
}