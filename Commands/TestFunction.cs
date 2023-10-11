using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using static BimIshou.Utils.Utils;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class TestFunction : ExternalCommand
    {
        public override void Execute()
        {
            var xxx = GetIntersectOfPipeWithFloor(Get3DView(Document), new XYZ(90.717410681, 123.157338301, 100), -XYZ.BasisZ);
            foreach (var x in xxx)
            {
                Debug.WriteLine(x);
            }
        }
    }
}
