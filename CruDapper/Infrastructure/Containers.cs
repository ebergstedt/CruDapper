namespace CruDapper
{
    public class WhereArgument
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public Operator? Operator { get; set; }
        public bool? Not { get; set; }
    }
}