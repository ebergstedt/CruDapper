using System;

namespace CruDapper
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
}