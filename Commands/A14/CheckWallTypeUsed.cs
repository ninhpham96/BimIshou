using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using System.Text;
using System.Windows;

namespace BimIshou.Commands.A14;

[Transaction(TransactionMode.Manual)]
public class CheckWallTypeUsed : ExternalCommand
{
    public override void Execute()
    {
        ElementId viewId = null;
        HashSet<string> wallSet = new();
        StringBuilder sb = new();
        if (ActiveView.ViewType is ViewType.FloorPlan)
            viewId = ActiveView.Id;
        else if (ActiveView is ViewSheet viewSheet)
        {
            foreach (var item in viewSheet.GetAllViewports())
            {
                Viewport viewport = Document.GetElement(item) as Viewport;
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
        IEnumerable<Wall> wallType = new FilteredElementCollector(Document, viewId)
            .OfClass(typeof(Wall))
            .WhereElementIsNotElementType()
            .Cast<Wall>()
            .Where(w => w.Name.Contains("L"))
            .GroupBy(x => x.Name)
            .Select(s => s.FirstOrDefault());
        foreach (var wall in wallType)
        {
            string wallName = wall.Name.Replace("(L)", "");
            foreach (var wall2 in wallName.Split('/'))
            {
                if (wall2.StartsWith("-")) continue;
                if (wallName.Contains("GW"))
                {
                    if (wallSet.Add("L" + wall2.Remove(2).Replace("-", "") + "-G"))
                        sb.AppendLine("L" + wall2.Remove(2).Replace("-", "") + "-G");
                }
                else
                {
                    if (wallSet.Add("L" + wall2.Remove(2)))
                        sb.AppendLine("L" + wall2.Remove(2));
                }
            }
        }
        MessageBox.Show(sb.ToString(), "Các tường đang sử dụng trong View");
    }
}
