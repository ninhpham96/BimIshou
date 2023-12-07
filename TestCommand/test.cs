using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.TestCommand
{
    [Transaction(TransactionMode.Manual)]
    internal class test : ExternalCommand
    {
        public override void Execute()
        {
        }
    }
}
