using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using System.Windows;

namespace BimIshou.TestCommand
{
    [Transaction(TransactionMode.Manual)]
    public class test : ExternalCommand
    {
        public override void Execute()
        {
            var collector = new FilteredElementCollector(Document)
            .OfClass(typeof(View))
            .Cast<View>()
            .Where(v => v.IsTemplate)
            .Where(v => v.ViewType == ViewType.ThreeD);

            Debug.WriteLine(collector);
        }
    }
}
