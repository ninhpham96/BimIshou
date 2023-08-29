using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
public class DimCurtainWall : ExternalCommand
{
    public override void Execute()
    {
        ReferenceArray refs = new ReferenceArray();
        List<Mullion> elements = new List<Mullion>();
        XYZ dir = null;
        Mullion max = null;
        Mullion min = null;
        SelectionFilter filter = new SelectionFilter(BuiltInCategory.OST_CurtainWallMullions);
        //var selects = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, filter);
        var selects = UiDocument.Selection.GetElementIds();
        var point = UiDocument.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);

        foreach (var select in selects)
        {
            var ele = Document.GetElement(select) as Mullion;
            if (ele.HandOrientation.IsParallel(XYZ.BasisZ)) continue;
            dir ??= ele.HandOrientation;
            elements.Add(ele);
        }
        if (dir.IsParallel(XYZ.BasisX))
        {
            max = elements.OrderBy(x => (x.Location as LocationPoint).Point.X).First();
            min = elements.OrderBy(x => (x.Location as LocationPoint).Point.X).Last();
            elements.Remove(max);
            elements.Remove(min);
        }
        else
        {
            max = elements.OrderBy(x => (x.Location as LocationPoint).Point.Y).First();
            min = elements.OrderBy(x => (x.Location as LocationPoint).Point.Y).Last();
            elements.Remove(max);
            elements.Remove(min);
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
        foreach (PlanarFace item in max.GetFacesSymbol())
        {
            XYZ p = (elements.First().Location as LocationPoint).Point;
            XYZ p1 = (max.Location as LocationPoint).Point;
            if (item != null && item.FaceNormal.IsParallel(XYZ.BasisX))
            {
                var xxx = item.Project(new XYZ(p.X, p.Y, 1000));
                var p2 = xxx.XYZPoint;
                if (p2.DistanceTo(p) < p1.DistanceTo(p))
                {
                    refs.Append(item.Reference);
                }
            }
        }





        using (Transaction tran = new Transaction(Document, "Dim"))
        {
            tran.Start();

            Document.Create.NewDimension(ActiveView, Line.CreateBound(point, point.Add(dir * 100)), refs);

            tran.Commit();
        }
    }
}
