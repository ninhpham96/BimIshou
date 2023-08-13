using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
public class DimDoorOrWindowInDoubuchi : ExternalCommand
{
    public override void Execute()
    {
        ReferenceArray dim1 = new ReferenceArray();
        ReferenceArray dim2 = new ReferenceArray();
        var grids = new FilteredElementCollector(Document, ActiveView.Id)
                .OfClass(typeof(Grid))
                .WhereElementIsNotElementType()
                .Cast<Grid>();
        var line = grids.First().GetCurvesInView(DatumExtentType.ViewSpecific, ActiveView).First() as Line;
        XYZ point = line.GetEndPoint(0).Z > line.GetEndPoint(0).Z ? line.GetEndPoint(1) : line.GetEndPoint(0);

        SelectionFilter filter = new SelectionFilter(BuiltInCategory.OST_GenericModel, true);

        var selectedeles = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element,
                                        filter,
                                        "Chọn các đối tượng muốn Dim");
        foreach ( var grid in grids)
        {
            dim2.Append(grid.Curve.Reference);
        }
        MessageBox.Show(dim2.Size.ToString());
        foreach (var sel in selectedeles)
        {
            FamilyInstance ele = Document.GetElement(sel) as FamilyInstance;
            if (ele != null)
            {
                Reference Left = ele.GetReferenceByName("Left");
                Reference Right = ele.GetReferenceByName("Right");

                dim1.Append(Left);
                dim1.Append(Right);
            }
        }

        Line line1 = Line.CreateBound(point.Add(ActiveView.UpDirection * -1000 / 304.8), point.Add(ActiveView.UpDirection * -1000 / 304.8).Add(ActiveView.RightDirection * 100));
        Line line2 = Line.CreateBound(point.Add(ActiveView.UpDirection * -1300 / 304.8), point.Add(ActiveView.UpDirection * -1300 / 304.8).Add(ActiveView.RightDirection * 100));
        using (Transaction tran = new Transaction(Document, "create dim"))
        {
            tran.Start();
            Document.Create.NewDimension(ActiveView, line1, dim1);
            Document.Create.NewDimension(ActiveView, line2, dim2);
            tran.Commit();
        }
    }
}
