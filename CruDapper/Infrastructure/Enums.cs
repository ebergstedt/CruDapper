namespace CruDapper
{
    public enum Operator
    {
        Equals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Like,
        In
    }

    public enum Provider
    {
        Postgres,
        MsSql,
        Default
    }
}