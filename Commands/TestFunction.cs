using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class TestFunction : ExternalCommand
    {
        public override void Execute()
        {
            var line = Document.GetElement(new ElementId(57771951)) as DetailLine;

            var p1 = UiDocument.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);
            var p2 = UiDocument.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);

            bool b = IsSameSide(line.GeometryCurve as Line, p1, p2);
            MessageBox.Show(b.ToString());
        }
        bool IsSameSide(Line line, XYZ a, XYZ b)
        {
            XYZ cp1 = (line.GetEndPoint(0) - a).CrossProduct(b - a);
            XYZ cp2 = (line.GetEndPoint(1) - a).CrossProduct(b - a);
            return cp1.DotProduct(cp2) >= 0;
        }
    }
}
