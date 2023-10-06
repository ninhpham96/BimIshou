using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands.A14
{
    [Transaction(TransactionMode.Manual)]
    internal class PutFillRegionToA14 : ExternalCommand
    {
        public override void Execute()
        {
            var mainDialog = new TaskDialog("Place region to Sheet")
            {
                EnableMarqueeProgressBar = false,
                TitleAutoPrefix = false,
                CommonButtons = TaskDialogCommonButtons.None,
                MainIcon = TaskDialogIcon.TaskDialogIconNone,
                DefaultButton = TaskDialogResult.None,
            };

            mainDialog.MainInstruction = "Chọn";
            mainDialog.MainContent = "";

            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Trái sang phải");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Phải qua trái");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, "Dưới lên trên");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink4, "Trên xuống dưới");

            var tResult = mainDialog.Show();

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
            }
            ElementId viewportTypeID = null;
            ElementId viewId = null;
            HashSet<string> wallSet = new();
            List<string> sb = new();
            double moveX = 0;
            double moveY = 0;
            if (ActiveView.ViewType is ViewType.FloorPlan)
                viewId = ActiveView.Id;
            else if (ActiveView is ViewSheet viewSheet)
            {
                foreach (var item in viewSheet.GetAllViewports())
                {
                    Viewport viewport = Document.GetElement(item) as Viewport;
                    foreach (var type in viewport.GetValidTypes())
                    {
                        var viewportType = Document.GetElement(type);
                        if (viewportType.Name == "非表示")
                        {
                            viewportTypeID = viewportType.Id;
                            break;
                        }
                    }
                    if (Document.GetElement(viewport.ViewId) is ViewPlan view)
                    {
                        if (Document.GetElement(view.ViewTemplateId).Name.Contains("A14"))
                        {
                            viewId = view.Id;
                            break;
                        }
                    }
                }
            }
            var wallLegends = new FilteredElementCollector(Document)
                .OfClass(typeof(View))
                .WhereElementIsNotElementType()
                .Cast<View>()
                .Where(v => v.ViewType is ViewType.Legend)
                .Where(v => v.Name.StartsWith("L"))
                .ToHashSet();
            var wallType = new FilteredElementCollector(Document, viewId)
                .OfClass(typeof(Wall))
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .Where(w => w.Name.Contains("L"))
                .Select(x => x.Name)
                .ToHashSet();
            foreach (var wall in wallType)
            {
                string wallName = wall.Replace("(L)", "");
                foreach (var wall2 in wallName.Split('/'))
                {
                    if (wall2.StartsWith("-")) continue;
                    if (wallName.Contains("GW"))
                    {
                        if (wallSet.Add("L" + wall2.Remove(2).Replace("-", "") + "-G"))
                            sb.Add("L" + wall2.Remove(2).Replace("-", "") + "-G");
                    }
                    else
                    {
                        if (wallSet.Add("L" + wall2.Remove(2)))
                            sb.Add("L" + wall2.Remove(2));
                    }
                }
            }
            if (wallLegends.Count < sb.Count)
            {
                MessageBox.Show("Bạn cần tạo đủ các Legend cho tất cả các loại tường", "Cảnh báo");
                return;
            }
            using (Transaction tran = new Transaction(Document, "Place View to Sheet"))
            {
                tran.Start();
                XYZ p;
                try
                {
                    p = UiDocument.Selection.PickPoint("Chọn điểm muốn đặt.");
                }
                catch (Exception)
                {
                    return;
                }
                for (int i = 0; i < sb.Count; i++)
                {
                    foreach (var view in wallLegends)
                    {
                        if (moveX == 0)
                        {
                            moveX = new FilteredElementCollector(Document, view.Id)
                                .OfCategory(BuiltInCategory.OST_Lines)
                                .WhereElementIsNotElementType()
                                .Cast<DetailLine>()
                                .Where(l => (l.GeometryCurve as Line).Direction.IsParallel(XYZ.BasisX))
                                .Max(l => l.GeometryCurve.Length) / view.Scale;
                            moveY = new FilteredElementCollector(Document, view.Id)
                                .OfCategory(BuiltInCategory.OST_Lines)
                                .WhereElementIsNotElementType()
                                .Cast<DetailLine>()
                                .Where(l => (l.GeometryCurve as Line).Direction.IsParallel(XYZ.BasisY))
                                .Max(l => l.GeometryCurve.Length) / view.Scale;
                        }
                        if (view.Name.Equals(sb[i]))
                        {
                            Viewport viewport;
                            switch (tResult)
                            {
                                case TaskDialogResult.CommandLink1:
                                    viewport = Viewport.Create(Document, ActiveView.Id, view.Id, new XYZ(p.X + moveX / 2 + i * moveX, p.Y + moveY / 2, 0));
                                    viewport.ChangeTypeId(viewportTypeID);
                                    break;
                                case TaskDialogResult.CommandLink2:
                                    viewport = Viewport.Create(Document, ActiveView.Id, view.Id, new XYZ(p.X - moveX / 2 - i * moveX, p.Y + moveY / 2, 0));
                                    viewport.ChangeTypeId(viewportTypeID);
                                    break;
                                case TaskDialogResult.CommandLink3:
                                    viewport = Viewport.Create(Document, ActiveView.Id, view.Id, new XYZ(p.X - moveX / 2, p.Y + moveY / 2 + moveY * i, 0));
                                    viewport.ChangeTypeId(viewportTypeID);
                                    break;
                                default:
                                    viewport = Viewport.Create(Document, ActiveView.Id, view.Id, new XYZ(p.X - moveX / 2, p.Y - moveY / 2 - moveY * i, 0));
                                    viewport.ChangeTypeId(viewportTypeID);
                                    break;
                            }
                            break;
                        }
                    }
                }
                tran.Commit();
            }
        }
    }
}
