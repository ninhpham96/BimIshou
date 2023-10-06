using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using static BimIshou.Utils.Utils;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class TestFunction : ExternalCommand
    {
        public override void Execute()
        {
            try
            {
                var temp = Document.GetElement(new ElementId(2546));


            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
