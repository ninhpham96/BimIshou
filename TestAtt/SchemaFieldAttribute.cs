namespace BimIshou.TestAtt
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SchemaFieldAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
