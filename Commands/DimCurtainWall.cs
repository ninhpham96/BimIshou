using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
public class DimCurtainWall : ExternalCommand
{
    public override void Execute()
    {
        ReferenceArray refs = new ReferenceArray();
        XYZ dir = null;

        SelectionFilter filter = new SelectionFilter(BuiltInCategory.OST_CurtainWallMullions);
        var selects = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, filter);
        var point = UiDocument.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);

        foreach (var select in selects)
        {
            var ele = Document.GetElement(select) as Mullion;
            if (ele.HandOrientation.IsParallel(XYZ.BasisZ)) continue;
            var faces = ele.GetFacesSymbol();
            dir ??= ele.HandOrientation;
            foreach (PlanarFace item in faces)
            {
                if (item != null && item.FaceNormal.IsParallel(XYZ.BasisX))
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
