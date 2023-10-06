using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace BimIshou.Commands.A14;

[Transaction(TransactionMode.Manual)]
public class CreateViewLegend : ExternalCommand
{
    public override void Execute()
    {
        HashSet<string> wallSet = new();
        StringBuilder sb = new();
        var wallType = new FilteredElementCollector(Document)
            .OfClass(typeof(Wall))
            .WhereElementIsNotElementType()
            .Cast<Wall>()
            .Where(w => w.Name.Contains("L"))
            .GroupBy(x => x.Name)
            .Select(s => s.FirstOrDefault());
        var wallLegends = new FilteredElementCollector(Document)
            .OfClass(typeof(View))
            .WhereElementIsNotElementType()
            .Cast<View>()
            .Where(v => v.ViewType is ViewType.Legend)
            .Where(v => v.Name.StartsWith("L")).ToHashSet();
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
        Debug.WriteLine(wallSet.Count);
        Debug.WriteLine(wallLegends.Count());
        foreach (var wall in wallLegends)
        {
            if (!wallSet.Add(wall.Name))
                wallSet.Remove(wall.Name);
        }

        using (Transaction tran = new(Document, "Create Wall Legend"))
        {
            tran.Start();
            foreach (var wall in wallSet)
            {
                var legend = wallLegends.FirstOrDefault().Duplicate(ViewDuplicateOption.WithDetailing);
                var ele = Document.GetElement(legend) as View;
                ele.Name = wall;
            }
            tran.Commit();
        }
        MessageBox.Show("Đã tạo thành công!", "Notification");
    }
}
