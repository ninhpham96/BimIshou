using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using static BimIshou.Utils.Utils;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
public class DimDoorOrWindowInDoubuchi : ExternalCommand
{
    public override void Execute()
    {
        try
        {
            using (TransactionGroup tranG = new TransactionGroup(Document, "Dim"))
            {
                tranG.Start();
                ReferenceArray dim1 = new ReferenceArray();
                ReferenceArray dim2 = new ReferenceArray();

                var grids = new FilteredElementCollector(Document, ActiveView.Id)
                        .OfClass(typeof(Grid))
                        .WhereElementIsNotElementType()
                        .Cast<Grid>()
                        .OrderBy(x => x.GetCurvesInView(DatumExtentType.Model, ActiveView).FirstOrDefault().GetEndPoint(0).DotProduct(ActiveView.RightDirection));
                var line = grids.First().GetCurvesInView(DatumExtentType.ViewSpecific, ActiveView).First() as Line;
                XYZ point = line.GetEndPoint(0).Z > line.GetEndPoint(0).Z ? line.GetEndPoint(1) : line.GetEndPoint(0);

                Line line0 = Line.CreateBound(point.Add(ActiveView.UpDirection * -700 / 304.8), point.Add(ActiveView.UpDirection * -700 / 304.8).Add(ActiveView.RightDirection * 100));
                Line line1 = Line.CreateBound(point.Add(ActiveView.UpDirection * -1000 / 304.8), point.Add(ActiveView.UpDirection * -1000 / 304.8).Add(ActiveView.RightDirection * 100));
                Line line2 = Line.CreateBound(point.Add(ActiveView.UpDirection * -1300 / 304.8), point.Add(ActiveView.UpDirection * -1300 / 304.8).Add(ActiveView.RightDirection * 100));

                SelectionFilter filter = new SelectionFilter(BuiltInCategory.OST_GenericModel, true);

                var selectedeles = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element,
                                                filter,
                                                "Chọn các đối tượng muốn Dim");
                foreach (Grid g in grids)
                {
                    if (g != null)
                    {
                        Reference gridRef = null;
                        Options opt = new Options();
                        opt.ComputeReferences = true;
                        opt.IncludeNonVisibleObjects = true;
                        opt.View = Document.ActiveView;
                        foreach (GeometryObject obj in g.get_Geometry(opt))
                        {
                            if (obj is Line)
                            {
                                Line l = obj as Line;
                                gridRef = l.Reference;
                                dim2.Append(gridRef);
                            }
                        }
                    }
                }
                foreach (var sel in selectedeles)
                {
                    double left = 0;
                    double top = 0;
                    ReferenceArray dim3 = new ReferenceArray();
                    ReferenceArray dim4 = new ReferenceArray();
                    FamilyInstance ele = Document.GetElement(sel) as FamilyInstance;
                    if (ele != null)
                    {
                        Reference Left = ele.GetReferenceByName("Left");
                        Reference Right = ele.GetReferenceByName("Right");
                        if (left == 0 && top == 0)
                        {
                            left = ele.LookupParameter("開口_左右クリア").AsDouble() + ele.LookupParameter("有効開口_幅").AsDouble() / 2;
                            top = ele.LookupParameter("開口_下部クリア").AsDouble() + ele.LookupParameter("有効開口_高さ").AsDouble() / 2;
                            dim1.Append(Left);
                            dim1.Append(Right);
                        }
                        dim3.Append(Right);
                        dim4.Append(Left);
                        var loca = (ele.Location as LocationPoint).Point;
                        var ele1 = GetElementIn3DView(Get3DView(Document), new XYZ(loca.X, loca.Y, top), ActiveView.RightDirection);
                        var ele2 = GetElementIn3DView(Get3DView(Document), new XYZ(loca.X, loca.Y, top), -ActiveView.RightDirection);

                        foreach (Face item in ele1.GetFacesInstance())
                        {
                            var locatemp = new XYZ(loca.X, loca.Y, top);
                            var temp = item?.Project(locatemp)?.XYZPoint;
                            if (temp == null) continue;
                            var dis = loca.DistanceTo(temp);
                            if (locatemp.DistanceTo(temp).IsAlmostEqual(left, 0.01))
                            {
                                dim3.Append(item.Reference);
                            }
                        }
                        foreach (Face item in ele2.GetFacesInstance())
                        {
                            var locatemp = new XYZ(loca.X, loca.Y, top);
                            var temp = item?.Project(locatemp)?.XYZPoint;
                            if (temp == null) continue;
                            if (locatemp.DistanceTo(temp).IsAlmostEqual(left, 0.001))
                            {
                                dim4.Append(item.Reference);
                            }
                        }
                        using (Transaction tran = new Transaction(Document, "create dim"))
                        {
                            tran.Start();
                            var d1 = Document.Create.NewDimension(ActiveView, line0, dim3);
                            var d2 = Document.Create.NewDimension(ActiveView, line0, dim4);
                            d1.TextPosition = d1.TextPosition.Add(ActiveView.RightDirection * 200 / 304.8);
                            d2.TextPosition = d2.TextPosition.Add(-ActiveView.RightDirection * 200 / 304.8);
                            tran.Commit();
                        }
                        dim3.Clear();
                        dim4.Clear();
                    }
                }

                dim1.Append(dim2.get_Item(0));
                dim1.Append(dim2.get_Item(dim2.Size - 1));

                using (Transaction tran = new Transaction(Document, "create dim"))
                {
                    tran.Start();
                    Document.Create.NewDimension(ActiveView, line1, dim1);
                    Document.Create.NewDimension(ActiveView, line2, dim2);
                    tran.Commit();
                }
                tranG.Assimilate();
            }
        }
        catch (Exception)
        {
        }
    }
}
