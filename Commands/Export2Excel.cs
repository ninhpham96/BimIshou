using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Microsoft.Office.Interop.Excel;
using Nice3point.Revit.Toolkit.External;
using NPOI.SS.UserModel;
using Excel = Microsoft.Office.Interop.Excel;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Export2Excel : ExternalCommand
    {
        public override void Execute()
        {
            string path = @"C:\Users\HC-07\Desktop\";
            ViewSchedule viewSchedule = Document.GetElement(new ElementId(3348)) as ViewSchedule;

            ViewScheduleExportOptions exportOptions = new ViewScheduleExportOptions();
            exportOptions.TextQualifier = ExportTextQualifier.DoubleQuote;
            exportOptions.FieldDelimiter = ",";
            exportOptions.Title = false;
            exportOptions.HeadersFootersBlanks = false;

            viewSchedule.Export(path, "test.csv", exportOptions);

            Excel.Application excelApp = new Excel.Application();

            // Mở một workbook từ file
            Excel.Workbook workbook = excelApp.Workbooks.Open(path + "test.csv");

            // Lấy ra worksheet đầu tiên (hoặc theo tên)
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets[1];

            // Tạo dòng mới tại dòng tiếp theo
            Excel.Range newDataRow = (Range)worksheet.Rows[0];

            // Đặt giá trị cho các ô trong dòng mới
            //newDataRow.Cells[1, 1] = "New Value 1";
            //newDataRow.Cells[1, 2] = "New Value 2";
            // ... Đặt giá trị cho các ô khác tùy thuộc vào nhu cầu của bạn

            // Lưu workbook
            workbook.Save();

            // Đóng workbook và ứng dụng Excel
            workbook.Close();
            excelApp.Quit();

            //ViewSheet viewSheet = Document.GetElement(new ElementId(3041)) as ViewSheet;
            //var xxxx = viewSheet.GetAllRevisionIds();
            //foreach ( var x in xxxx )
            //{
            //    var revision = x.ToElement(Document) as Revision;
            //}
        }
    }
}
