using Autodesk.Revit.Attributes;
using BimIshou.Utils;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands.DimDoubuchi;

[Transaction(TransactionMode.Manual)]
public class DimDoubuchi : ExternalCommand
{
    public static int setting { get; set; } = 3040;
    public override void Execute()
    {
        try
        {
            ReferenceArray dim1 = new ReferenceArray();
            ReferenceArray dim2 = new ReferenceArray();

            var grid = new FilteredElementCollector(Document, ActiveView.Id)
                .OfClass(typeof(Grid))
                .WhereElementIsNotElementType()
                .Cast<Grid>()
                .First();
            var line = grid.GetCurvesInView(DatumExtentType.ViewSpecific, ActiveView).First() as Line;
            XYZ point = line.GetEndPoint(0).Z > line.GetEndPoint(0).Z ? line.GetEndPoint(0) : line.GetEndPoint(1);

            List<BuiltInCategory> builtInCategories = new List<BuiltInCategory>() { BuiltInCategory.OST_GenericModel,
                                                           BuiltInCategory.OST_StructuralColumns };
            SelectionFilter filter = new SelectionFilter(builtInCategories, true);

            var selectedeles = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element,
                                            filter,
                                            "Chọn các đối tượng muốn Dim");
            var el = UiDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element,
                                            filter,
                                            "Chọn đối tượng chia tấm");
            var tempCurve = ((Document.GetElement(el) as FamilyInstance).Location as LocationCurve)?.Curve;
            foreach (var sel in selectedeles)
            {
                var ele = Document.GetElement(sel) as FamilyInstance;
                if (ele != null)
                {
                    Reference refe = ele.GetReferenceByName("中心(左/右)");
                    dim1.Append(refe);
                }
            }
            if (tempCurve != null)
            {
                foreach (var sel in selectedeles)
                {
                    var ele = Document.GetElement(sel) as FamilyInstance;
                    var curve = (ele.Location as LocationCurve)?.Curve;
                    if (curve == null) continue;
                    if ((curve.Distance(tempCurve.GetEndPoint(0)) / setting).IsAlmostEqual(0, 0.1))
                    {
                        Reference refe = ele.GetReferenceByName("中心(左/右)");
                        dim2.Append(refe);
                    }
                }
            }
            Line line1 = Line.CreateBound(point.Add(ActiveView.UpDirection * 1000 / 304.8), point.Add(ActiveView.UpDirection * 1000 / 304.8).Add(ActiveView.RightDirection * 100));
            Line line2 = Line.CreateBound(point.Add(ActiveView.UpDirection * 1300 / 304.8), point.Add(ActiveView.UpDirection * 1300 / 304.8).Add(ActiveView.RightDirection * 100));
            using (Transaction tran = new Transaction(Document, "create dim1"))
            {
                tran.Start();
                Document.Create.NewDimension(ActiveView, line1, dim1);
                if (dim2.Size > 1)
                    Document.Create.NewDimension(ActiveView, line2, dim2);
                tran.Commit();
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }

    }
}
