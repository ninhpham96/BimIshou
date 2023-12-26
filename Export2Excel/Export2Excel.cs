using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Office.Interop.Excel;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;

namespace BimIshou;

[Transaction(TransactionMode.Manual)]
public partial class Export2Excel : ExternalCommand
{
    public List<ViewSchedule> _source { get; private set; }
    public HashSet<ViewSchedule> selecteds { get; set; }
    public ViewSchedule Selected { get; set; }
    public override void Execute()
    {
        selecteds = new();
        _source = GetAllViewSchedules();
        _View view = new _View() { DataContext = this };
        view.ShowDialog();
    }

    [RelayCommand]
    private void Ok(object obj)
    {
        //Export(allSheet, allRevision);
        Debug.WriteLine(Selected.Name);
        System.Windows.Window window = (System.Windows.Window)obj;
        window.Close();
    }

    [RelayCommand]
    private void Cancel(object obj)
    {
        System.Windows.Window window = (System.Windows.Window)obj;
        window.Close();
    }

    private void Export()
    {
        var allSheet = GetAllSheets();
        var allRevision = Revision.GetAllRevisionIds(Document);
        string path = @"C:\Users\My PC\Desktop";
        ViewSchedule viewSchedule = Document.GetElement(new ElementId(2831)) as ViewSchedule;

        ViewScheduleExportOptions exportOptions = new ViewScheduleExportOptions();
        exportOptions.TextQualifier = ExportTextQualifier.DoubleQuote;
        exportOptions.FieldDelimiter = ",";
        exportOptions.Title = false;
        exportOptions.HeadersFootersBlanks = false;

        viewSchedule.Export(path, "test.csv", exportOptions);
        Excel.Application excelApp = new Excel.Application();
        Workbook workbook = excelApp.Workbooks.Open(path + "\\test.csv");
        try
        {
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
            var rng = worksheet.UsedRange;
            int colCount = rng.Columns.Count;
            int rowCount = rng.Rows.Count;

            rng = (Range)worksheet.Cells[rowCount, colCount];

            for (int i = 0; i < allRevision.Count; i++)
            {
                Revision revision = Document.GetElement(allRevision[i]) as Revision;
                worksheet.Cells[1, colCount + 1 + i] = revision.Description;
            }

            for (int i = 2; i <= rowCount; i++)
            {
                string sheetNumber = (worksheet.Cells[i, 2] as Range).Value.ToString();
                var sheet = allSheet.Where(sheet => sheet.SheetNumber.Equals(sheetNumber)).FirstOrDefault();
                var allrevisionofSheet = sheet.GetAllRevisionIds();
                foreach (var item in allrevisionofSheet)
                {
                    Revision revision = Document.GetElement(item) as Revision;
                    for (int j = colCount + 1; j < colCount + 1 + allRevision.Count; j++)
                    {
                        string value = (worksheet.Cells[1, j] as Range).Value.ToString();
                        if (revision.Description.Equals(value))
                        {
                            worksheet.Cells[i, j] = revision.RevisionDate;
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
