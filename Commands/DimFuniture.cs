using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using static BimIshou.Utils.Utils;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
internal class DimFuniture : ExternalCommand
{
    public override void Execute()
    {
        ReferenceArray referenceArray = new ReferenceArray();
        try
        {
            var filterplumbingFixtures = new SelectionFilter(BuiltInCategory.OST_PlumbingFixtures, true);
            var point = UiDocument.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);
            var selectedplumbingFixtures = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, filterplumbingFixtures, "Chọn các thiết bị muốn Dim");

            foreach (Reference item in selectedplumbingFixtures)
            {
                var ele = Document.GetElement(item) as FamilyInstance;
                var r = ele.GetReferenceByName("中心(左/右)");
                if (r != null) referenceArray.Append(r);
            }
            var view3d = Get3DView(Document);
            var temp = Document.GetElement(selectedplumbingFixtures.First()) as FamilyInstance;

            var loca = (temp.Location as LocationPoint).Point;
            var linee = Line.CreateBound(point, point.Add(temp.HandOrientation * 100));

            referenceArray.Append(GetCeilingReferenceAbove(view3d, loca.Add(-temp.FacingOrientation * 100 / 304.8), temp.HandOrientation));
            referenceArray.Append(GetCeilingReferenceAbove(view3d, loca.Add(-temp.FacingOrientation * 100 / 304.8), -temp.HandOrientation));

            using (Transaction tran = new Transaction(Document, "new tran"))
            {
                tran.Start();
                Document.Create.NewDimension(Document.ActiveView, linee, referenceArray);
                tran.Commit();
            }
        }
        catch (Exception)
        {
        }
    }
}

