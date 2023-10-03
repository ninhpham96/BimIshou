using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using static BimIshou.Utils.Utils;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
public class DimCurtainWall : ExternalCommand
{
    public override void Execute()
    {
        ReferenceArray refs = new ReferenceArray();
        ReferenceArray refs1 = new ReferenceArray();
        ReferenceArray refs2 = new ReferenceArray();
        List<Mullion> elements = new List<Mullion>();
        List<Mullion> elements2 = new List<Mullion>();
        FamilyInstance door = null;
        Wall host = null;
        XYZ dir = null;
        Curve line = null;
        List<BuiltInCategory> categories = new List<BuiltInCategory>() { BuiltInCategory.OST_CurtainWallMullions, BuiltInCategory.OST_Doors };
        SelectionFilter filter = new SelectionFilter(categories);
        var selects = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, filter);
        var point = UiDocument.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);

        foreach (var select in selects)
        {
            var ele = Document.GetElement(select) as Mullion;
            if (ele == null)
            {
                door = Document.GetElement(select) as FamilyInstance;
                host ??= door.Host as Wall;
                continue;
            }
            if (ele.HandOrientation.IsParallel(XYZ.BasisZ)) continue;
            dir ??= ele.HandOrientation;
            line ??= ((ele.Host as Wall).Location as LocationCurve).Curve;
            elements.Add(ele);
        }
        if(door != null)
        {
            //var left = door.GetReferenceByName("左") ?? door.GetReferences(FamilyInstanceReferenceType.Left).First();
            //var right = door.GetReferenceByName("右") ?? door.GetReferences(FamilyInstanceReferenceType.Right).First();
            //refs.Append(left);
            //refs.Append(right);
            refs.Append(door.GetReferenceByName("ref1"));
            refs.Append(door.GetReferenceByName("ref2"));
        }
        if (dir.IsParallel(XYZ.BasisX))
        {
            var temp1 = elements.OrderBy(x => (x.Location as LocationPoint).Point.X).First();
            var temp2 = elements.OrderBy(x => (x.Location as LocationPoint).Point.X).Last();
            elements2.Add(temp1);
            elements2.Add(temp2);
            elements.Remove(temp1);
            elements.Remove(temp2);
        }
        else
        {
            var temp1 = elements.OrderBy(x => (x.Location as LocationPoint).Point.Y).First();
            var temp2 = elements.OrderBy(x => (x.Location as LocationPoint).Point.Y).Last();
            elements2.Add(temp1);
            elements2.Add(temp2);
            elements.Remove(temp1);
            elements.Remove(temp2);
        }
        foreach (var ele in elements)
        {
            var faces = ele.GetFacesSymbol();
            foreach (PlanarFace item in faces)
            {
                if (item != null && item.FaceNormal.IsParallel(XYZ.BasisX))
                {
                    refs.Append(item.Reference);
                }
            }
        }
        foreach (var ele in elements2)
        {
            var faces = ele.GetFacesSymbol();
            foreach (PlanarFace item in faces)
            {
                if (item != null && item.FaceNormal.X == -1)
                {
                    refs.Append(item.Reference);
                    refs1.Append(item.Reference);
                }
            }
        }
        //foreach (Reference item in door?.GetReferences(FamilyInstanceReferenceType.WeakReference))
        //{
        //    refs2.Append(item);
        //}

        var tempDir = Line.CreateBound(line.Project(point).XYZPoint, point).Direction;
        using (Transaction tran = new Transaction(Document, "Dim"))
        {
            tran.Start();
            Document.Create.NewDimension(ActiveView, Line.CreateBound(point, point.Add(dir * 100)), refs);
            Document.Create.NewDimension(ActiveView, Line.CreateBound(point.Add(tempDir * 300 / 304.8), point.Add(tempDir * 300 / 304.8).Add(dir * 100)), refs1).Prefix = "W";
            tran.Commit();
        }
    }
}
