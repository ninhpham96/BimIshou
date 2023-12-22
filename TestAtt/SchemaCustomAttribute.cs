namespace BimIshou.TestAtt
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaCustomAttribute : Attribute
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string VendorId {  get; set; }
    }
}
