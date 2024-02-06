using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou
{
    [Transaction(TransactionMode.Manual)]
    public class Test : ExternalCommand
    {
        public override void Execute()
        {
            var allSheet = new FilteredElementCollector(Document)
                .OfClass(typeof(ViewSheet))
                .WhereElementIsNotElementType()
                .Cast<ViewSheet>()
                .ToList();

            using (Transaction tran = new Transaction(Document, "Set revision"))
            {
                tran.Start();
                foreach (var viewSheet in allSheet)
                {
                    var allRevisionofSheet = viewSheet.GetAllRevisionIds();
                    for (int i = 0; i < allRevisionofSheet.Count; i++)
                    {
                        var revision = Document.GetElement(allRevisionofSheet[i]) as Revision;
                        if (revision.SequenceNumber == 1)
                        {
                            var para = viewSheet.LookupParameter("test1");
                            para.Set(i.ToString());
                        }
                        else if (revision.SequenceNumber == 2)
                        {
                            var para = viewSheet.LookupParameter("test2");
                            para.Set(i.ToString());
                        }
                        else if (revision.SequenceNumber == 3)
                        {
                            var para = viewSheet.LookupParameter("test3");
                            para.Set(i.ToString());
                        }
                        else
                        {
                            var para = viewSheet.LookupParameter("test4");
                            para.Set(i.ToString());
                        }
                    }
                    tran.Commit();
                }
            }
        }
    }
}
