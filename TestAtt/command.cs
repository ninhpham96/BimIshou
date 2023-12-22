using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.TestAtt
{
    [Transaction(TransactionMode.Manual)]
    class command : ExternalCommand
    {
        public override void Execute()
        {
            var reference = UiDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            var element = Document.GetElement(reference);
            var mySchema = new CustomSchema { Editor = "Kennan", Age = 26 };
            element.WriteData(mySchema);
        }
    }

    [SchemaCustom(Name = "MySchema", Guid = "09A33462-4979-49F0-A15E-90DFE444C243", VendorId = "22222")]
    public class CustomSchema : SchemaBase
    {
        public string Editor { get; set; }
        public int Age { get; set; }
    }
}
