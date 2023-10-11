using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BimIshou.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Office.Interop.Excel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace BimIshou.Commands.ChieucaoTB
{
    public partial class AverageAltitudeModel : ObservableObject
    {
        private Document doc;
        private UIDocument uidoc;
        private List<XYZ> xyzs = new List<XYZ>();
        ElementId textId = null;
        public ObservableCollection<Custom> itemSource { get; set; }
        public AverageAltitudeModel(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            doc = uidoc.Document;
            textId = new FilteredElementCollector(doc)
               .OfClass(typeof(TextNoteType))
               .WhereElementIsElementType()
               .Cast<TextNoteType>()
               .Where(t => t.Name == "Meiryo 1.5mm")
               .FirstOrDefault().Id;
            itemSource = new ObservableCollection<Custom>();
            xyzs = GetItemSource();
        }
        [RelayCommand]
        private void Ok(object obj)
        {
            (obj as System.Windows.Window).Close();

            using (Transaction tran = new Transaction(doc, "new tran"))
            {
                tran.Start();
                for (int i = 0; i < xyzs.Count; i++)
                {
                    if (itemSource[i].高さA > 0)
                        TextNote.Create(doc, doc.ActiveView.Id, xyzs[i], $"{itemSource[i].Name1}\n+{itemSource[i].高さA}", textId);
                    else
                        TextNote.Create(doc, doc.ActiveView.Id, xyzs[i], $"{itemSource[i].Name1}\n{itemSource[i].高さA}", textId);
                    if (i == xyzs.Count - 1)
                        TextNote.Create(doc, doc.ActiveView.Id, (xyzs[i] + xyzs[0]) / 2, xyzs[i].DistanceTo(xyzs[0]).ToMillimeters().Round(3).ToString(), textId);
                    else
                        TextNote.Create(doc, doc.ActiveView.Id, (xyzs[i] + xyzs[i + 1]) / 2, xyzs[i].DistanceTo(xyzs[i + 1]).ToMillimeters().Round(3).ToString(), textId);
                }
                tran.Commit();
            }
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Workbooks.Add();
            _Worksheet workSheet = (_Worksheet)excelApp.ActiveSheet;
            System.Windows.Controls.DataGrid data = obj as System.Windows.Controls.DataGrid;
            double sum距離 = 0;
            double sum面積 = 0;
            for (var i = 0; i < itemSource.Count; i++)
            {
                sum距離 += itemSource[i].距離;
                sum面積 += itemSource[i].面積;
                workSheet.Cells[i + 2, 1] = itemSource[i].Name1;
                workSheet.Cells[i + 2, 2] = itemSource[i].Name2;
                workSheet.Cells[i + 2, 3] = itemSource[i].距離;
                workSheet.Cells[i + 2, 4] = itemSource[i].高さA;
                workSheet.Cells[i + 2, 5] = itemSource[i].高さB;
                workSheet.Cells[i + 2, 6] = itemSource[i].平均高さ;
                workSheet.Cells[i + 2, 7] = itemSource[i].面積;
            }
            workSheet.Cells[7 + 2, 3] = sum距離;
            workSheet.Cells[7 + 2, 7] = sum面積;
            workSheet.Cells[8 + 2, 7] = sum面積 / sum距離;
            excelApp.Visible = true;
            excelApp.Quit();
        }
        [RelayCommand]
        private void Cancel(object obj)
        {
            System.Windows.Window window = (System.Windows.Window)obj;
            window.Close();
        }
        private List<XYZ> GetItemSource()
        {
            List<ModelLine> lines = new();
            List<XYZ> xyzs = new();

            var Ids = uidoc.Selection.GetElementIds();
            try
            {
                if (Ids.Count > 0)
                    foreach (var i in Ids)
                    {
                        if (doc.GetElement(i) is ModelLine ml)
                            lines.Add(ml as ModelLine);
                    }
                else
                    lines.AddRange(uidoc.Selection.PickElementsByRectangle(new SelectionFilter(BuiltInCategory.OST_AreaSchemeLines)).Select(x => x as ModelLine));

            }
            catch (Exception)
            {

            }
            for (int i = 0; i < lines.Count - 1; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (!xyzs.Any())
                    {
                        xyzs.Add(lines[i].GeometryCurve.GetEndPoint(0));
                        xyzs.Add(lines[i].GeometryCurve.GetEndPoint(1));
                        break;
                    }
                    if (xyzs.Last().IsAlmostEqualTo(lines[j].GeometryCurve.GetEndPoint(0)))
                    {
                        xyzs.Add(lines[j].GeometryCurve.GetEndPoint(1));
                        break;
                    }
                    else
                    {
                        xyzs.Add(lines[j].GeometryCurve.GetEndPoint(0));
                        break;
                    }
                }
            }
            for (int i = 0; i < xyzs.Count; i++)
            {
                if (i == xyzs.Count - 1)
                {
                    double dis = xyzs[i].DistanceTo(xyzs[0]).ToMeters().Round(3);
                    var custom = new Custom() { Id = i, Name1 = "NO." + (i + 1).ToString(), Name2 = "NO." + (1).ToString(), 距離 = dis };
                    itemSource.Add(custom);
                }
                else
                {
                    double dis = xyzs[i].DistanceTo(xyzs[i + 1]).ToMeters().Round(3);
                    var custom = new Custom() { Id = i, Name1 = "NO." + (i + 1).ToString(), Name2 = "NO." + (i + 2).ToString(), 距離 = dis };
                    itemSource.Add(custom);
                }
            }
            return xyzs;
        }
    }
}
