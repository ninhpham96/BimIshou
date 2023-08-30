using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
internal class AutoDimDoor1 : ExternalCommand
{
    public override void Execute()
    {
        ReferenceArray refss = new ReferenceArray();
        List<BuiltInCategory> categories = new List<BuiltInCategory>() { BuiltInCategory.OST_Doors, BuiltInCategory.OST_Windows };
        var filterWall = new SelectionFilter(BuiltInCategory.OST_Walls, true);
        var filterDoorOrWindow = new SelectionFilter(categories, true);
        try
        {
            var ele = UiDocument.Selection.PickObjects(ObjectType.Element, filterWall);
            var doororWindow = UiDocument.Selection.PickObjects(ObjectType.Element, filterDoorOrWindow);
            var p = UiDocument.Selection.PickPoint();
            XYZ dir = null;
            if (doororWindow.Count != 0)
            {
                foreach (Reference obj in doororWindow)
                {
                    if (obj == null) break;
                    var familyinstance = Document.GetElement(obj) as FamilyInstance;
                    dir ??= familyinstance.FacingOrientation;
                    var left = familyinstance.GetReferenceByName("左") ?? familyinstance.GetReferences(FamilyInstanceReferenceType.Left).First();
                    var right = familyinstance.GetReferenceByName("右") ?? familyinstance.GetReferences(FamilyInstanceReferenceType.Right).First();
                    refss.Append(left);
                    refss.Append(right);
                }
            }
            using (TransactionGroup tranG = new TransactionGroup(Document, "AutoDim"))
            {
                tranG.Start();
                foreach (Reference obj in ele)
                {
                    var wall = Document.GetElement(obj) as Wall;
                    Line line = (wall.Location as LocationCurve).Curve as Line;
                    dir ??= wall.Orientation;
                    if (doororWindow.Count != 0)
                        if (!line.Direction.IsParallel(dir)) continue;
                    using (Transaction tran = new Transaction(Document, "new Line"))
                    {
                        tran.Start();
                        var newLine = CreateModelLine.OnVerticalPlane(Document, Line.CreateBound(line.GetEndPoint(0), line.GetEndPoint(0).Add(line.Direction * 10)));
                        refss.Append(newLine.GeometryCurve.Reference);
                        tran.Commit();
                    }
                }
                Dimension dim;
                using (Transaction tran = new Transaction(Document, "new tran"))
                {
                    tran.Start();
                    dim = Document.Create.NewDimension(ActiveView, Line.CreateBound(p, p.Add(dir)), refss);
                    tran.Commit();
                }
                var ids = new List<ElementId>();
                var refs = dim.References;
                foreach (Reference item in refs)
                {
                    var temp = Document.GetElement(item);
                    if (temp.Category.Id.IntegerValue == (int)BuiltInCategory.OST_CurtainWallMullions)
                    {
                        var hostId = (temp as Mullion).Host.Id;
                        if (!ids.Contains(hostId))
                            ids.Add(hostId);
                    }
                    if (temp.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Doors)
                        || temp.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Windows))
                    {
                        if (!ids.Contains(temp.Id))
                            ids.Add(temp.Id);
                    }
                }
                foreach (ElementId id in ids)
                {
                    XYZ pos;
                    ReferenceArray Aref = new();
                    foreach (Reference item in refs)
                    {
                        var temp = Document.GetElement(item);
                        if (temp.Category.Id.IntegerValue == (int)BuiltInCategory.OST_CurtainWallMullions)
                        {
                            var hostId = (temp as Mullion).Host.Id;
                            if (hostId.IntegerValue == id.IntegerValue)
                                Aref.Append(item);
                        }
                        if (temp.Id.IntegerValue == id.IntegerValue)
                            Aref.Append(item);
                    }
                    using (Transaction tran = new Transaction(Document, "Dim W"))
                    {
                        tran.Start();
                        pos = Document.Create.NewDimension(Document.ActiveView, dim.Curve as Line, Aref).TextPosition;
                        tran.RollBack();
                    }
                    using (Transaction tran = new Transaction(Document, "Dim W"))
                    {
                        tran.Start();
                        foreach (DimensionSegment item in dim.Segments)
                        {
                            if (item.TextPosition.IsAlmostEqualTo(pos, 0.00000001))
                                item.Prefix = "W";
                        }
                        tran.Commit();
                    }
                }
                tranG.Assimilate();
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }
}
