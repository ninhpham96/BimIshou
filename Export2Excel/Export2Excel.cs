using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Office.Interop.Excel;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace BimIshou;

[Transaction(TransactionMode.Manual)]
public partial class Export2Excel : ExternalCommand
{
    public List<ViewSchedule> _source { get; private set; }
    public List<ViewSchedule> SelectedItems { get; set; }
    public override void Execute()
    {

        SelectedItems ??= new();
        _source = GetAllViewSchedules();
        _View view = new _View() { DataContext = this };
        view.ShowDialog();
    }
    [RelayCommand]
    private void Ok(object obj)
    {
        string path = Browser();
        var allSheet = GetAllSheets();
        var allRevision = Revision.GetAllRevisionIds(Document);
        foreach (var item in SelectedItems)
        {
            Export(path, item, allSheet, allRevision);
        }
        CloseWin(obj);
    }
    [RelayCommand]
    private void Cancel(object obj)
    {
        CloseWin(obj);
    }
    private void Export(string path, ViewSchedule viewSchedule, List<ViewSheet> allSheet, IList<ElementId> allRevision)
    {
        ViewScheduleExportOptions exportOptions = new ViewScheduleExportOptions();
        exportOptions.TextQualifier = ExportTextQualifier.DoubleQuote;
        exportOptions.FieldDelimiter = ",";
        exportOptions.Title = false;
        exportOptions.HeadersFootersBlanks = false;

        ProjectInfo projectInfo = Document.ProjectInformation;
        string TRSDC_Discipline = projectInfo.GetParameter("TRSDC_Discipline").AsValueString();
        string TRSDC_Originator_Code = projectInfo.GetParameter("TRSDC_Originator Code").AsValueString();
        string TRSDC_Project_Code = projectInfo.GetParameter("TRSDC_Project Code").AsValueString();
        string TRSDC_Project_Number = projectInfo.GetParameter("TRSDC_Project_Number").AsValueString();
        string TRSDC_Volumn_System = projectInfo.GetParameter("TRSDC_Volume/System").AsValueString();
        viewSchedule.Export(path, $"{viewSchedule.Name}.csv", exportOptions);

        Excel.Application excelApp = new Excel.Application();
        Workbook workbook = excelApp.Workbooks.Open(path + $"\\{viewSchedule.Name}.csv");
        try
        {
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
            Range range = worksheet.get_Range("B1", "Z100");
            range.ClearContents();
            var rng = worksheet.UsedRange;
            int colCount = rng.Columns.Count;
            int rowCount = rng.Rows.Count;

            worksheet.Cells[1, 2] = "Sheet Number";
            for (int i = 0; i < allRevision.Count; i++)
            {
                Revision revision = Document.GetElement(allRevision[i]) as Revision;
                worksheet.Cells[1, 3 + i] = revision.Description;
            }
            for (int i = 2; i <= rowCount; i++)
            {
                string sheetName = (worksheet.Cells[i, 1] as Range).Value.ToString();
                ViewSheet sheet = allSheet.Where(sheet => sheet.Name == sheetName).FirstOrDefault();
                if (sheet is null) continue;
                string TRSDC_Building_Level = sheet.GetParameter("TRSDC_Building Level").AsValueString();
                worksheet.Cells[i, 2] = $"{TRSDC_Project_Number}-{TRSDC_Project_Code}-{TRSDC_Originator_Code}-{TRSDC_Volumn_System}-{TRSDC_Building_Level}-DWG-{TRSDC_Discipline}-{sheet.SheetNumber}";

                foreach (var item in sheet.GetAllRevisionIds())
                {
                    Revision revision = Document.GetElement(item) as Revision;
                    for (int j = 3; j < 3 + allRevision.Count; j++)
                    {
                        string value = (worksheet.Cells[1, j] as Range).Value.ToString();
                        if (revision.Description.Equals(value))
                        {
                            worksheet.Cells[i, j] = sheet.GetRevisionNumberOnSheet(item);
                        }
                    }
                }
            }
            workbook.Save();
            workbook.Close();
            excelApp.Quit();
        }
        catch (Exception)
        {
            workbook.Close();
            excelApp.Quit();
            throw;
        }
    }
    private string Browser()
    {
        using (FolderBrowserDialog fbd = new FolderBrowserDialog())
        {
            if (fbd.ShowDialog() == DialogResult.OK)
                return fbd.SelectedPath;
            else
                return string.Empty;
        }
    }
    private static void CloseWin(object obj)
    {
        System.Windows.Window window = (System.Windows.Window)obj;
        window.Close();
    }
    private List<ViewSheet> GetAllSheets()
    {
        return new FilteredElementCollector(Document)
                .OfClass(typeof(ViewSheet))
                .WhereElementIsNotElementType()
                .Cast<ViewSheet>()
                .ToList();
    }
    private List<ViewSchedule> GetAllViewSchedules()
    {
        return new FilteredElementCollector(Document)
                .OfClass(typeof(ViewSchedule))
                .WhereElementIsNotElementType()
                .Cast<ViewSchedule>()
                .ToList();
    }
}
