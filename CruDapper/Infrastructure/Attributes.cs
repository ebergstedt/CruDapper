using System;

namespace CruDapper.Infrastructure
{
    public class AutoIncrementAttribute : Attribute
    {
    }

    public class FunctionAttribute : Attribute
    {
        public FunctionAttribute(string Name)
        {
            this.Name = Name;
        }

        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IgnorePropertyAttribute : Attribute
    {
        public IgnorePropertyAttribute(bool ignore)
        {
            Value = ignore;
        }

        public bool Value { get; set; }
    }
}