using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using System.Windows;

namespace BimIshou.Commands.A14;

[Transaction(TransactionMode.Manual)]
public class CreateKeyPlaneForA14 : ExternalCommand
{
    public override void Execute()
    {
        View a14View = null;
        ElementId viewportTypeID = null;

        if (ActiveView is not ViewSheet)
        {
            MessageBox.Show("Bạn cần chuyển về Sheet A14.");
            return;
        }
        if (ActiveView is ViewSheet viewsheet)
        {
            if (!viewsheet.SheetNumber.StartsWith("A14"))
            {
                MessageBox.Show("Bạn cần chuyển về Sheet A14.");
                return;
            }
            foreach (var vs in viewsheet.GetAllViewports())
            {
                var vp = Document.GetElement(vs) as Viewport;
                foreach (var type in vp.GetValidTypes())
                {
                    var viewportType = Document.GetElement(type);
                    if (viewportType.Name == "非表示")
                    {
                        viewportTypeID = viewportType.Id;
                        break;
                    }
                }
                if (Document.GetElement(vp.ViewId) is ViewPlan vpPlan)
                {
                    a14View = vpPlan;
                    break;
                }
            }
            if (a14View == null)
            {
                MessageBox.Show("Bạn cần đặt view mặt bằng chi tiết vào sheet trước khi chạy tool.");
                return;
            }
        }
        //get bounding of crop view
        var cropShape = a14View.GetCropRegionShapeManager().GetCropShape();
        //Get view template
        var viewTemplates = new FilteredElementCollector(Document)
            .OfClass(typeof(View))
            .WhereElementIsNotElementType()
            .Cast<View>()
            .Where(v => v.IsTemplate == true)
            .Where(v => v.Name.StartsWith("A17 展開 キープラン"))
            .ToHashSet();
        //Get type of FilledRegion
        var filledRegion = new FilteredElementCollector(Document)
            .OfClass(typeof(FilledRegionType))
            .WhereElementIsElementType()
            .Cast<FilledRegionType>()
            .Where(f => f.Name == "ハッチ_平行45°0.8mm red")
            .FirstOrDefault();
        //create keyplane
        XYZ point = UiDocument.Selection.PickPoint("Pick điểm đặt.");
        using (Transaction tran = new Transaction(Document, "Create Key Plane"))
        {
            tran.Start();
            var primaryView = Document.GetElement(a14View.GetPrimaryViewId()) as View;
            var keyPlaneId = primaryView.Duplicate(ViewDuplicateOption.Duplicate);
            View keyplane = Document.GetElement(keyPlaneId) as View;
            keyplane.Name = a14View.Name.Replace("平面詳細図", "平面詳細図キープラン");
            keyplane.ViewTemplateId = viewTemplates.FirstOrDefault().Id;
            keyplane.Scale = 500;
            keyplane.CropBoxVisible = false;

            FilledRegion.Create(Document, filledRegion.Id, keyPlaneId, cropShape);

            var viewport = Viewport.Create(Document, ActiveView.Id, keyPlaneId, point);
            viewport.ChangeTypeId(viewportTypeID);
            tran.Commit();
        }
    }
}
