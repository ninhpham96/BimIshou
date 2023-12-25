using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Microsoft.Office.Interop.Excel;
using Nice3point.Revit.Extensions;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Export2Excel : ExternalCommand
    {
        public override void Execute()
        {
            var allSheet = GetAllSheet();
            var allRevision = Revision.GetAllRevisionIds(Document);

            string path = @"C:\Users\My PC\Desktop";
            ViewSchedule viewSchedule = Document.GetElement(new ElementId(7920)) as ViewSchedule;

            ViewScheduleExportOptions exportOptions = new ViewScheduleExportOptions();
            exportOptions.TextQualifier = ExportTextQualifier.DoubleQuote;
            exportOptions.FieldDelimiter = ",";
            exportOptions.Title = false;
            exportOptions.HeadersFootersBlanks = false;

            viewSchedule.Export(path, "test.csv", exportOptions);
            Excel.Application excelApp = new Excel.Application();

            // Mở một workbook từ file
            Workbook workbook = excelApp.Workbooks.Open(path + "\\test.csv");
            try
            {
                // Lấy ra worksheet đầu tiên (hoặc theo tên)
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

                // Lưu workbook
                workbook.Save();
                // Đóng workbook và ứng dụng Excel
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
        private List<ViewSheet> GetAllSheet()
        {
            return new FilteredElementCollector(Document)
                .OfClass(typeof(ViewSheet))
                .WhereElementIsNotElementType()
                .Cast<ViewSheet>()
                .ToList();
        }
    }
}
