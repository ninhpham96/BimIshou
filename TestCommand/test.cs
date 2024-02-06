using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Newtonsoft.Json;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using System.Text;

namespace BimIshou.TestCommand
{
    [Transaction(TransactionMode.Manual)]
    public class test : ExternalCommand
    {
        public override void Execute()
        {
            using Transaction tran = new Transaction(Document, "new tran");
            tran.Start();
            foreach (Category category in Document.Settings.Categories)
            {
                if (category.CategoryType.Equals(CategoryType.Model))
                {
                    if (category.CanAddSubcategory)
                    {
                        try
                        {
                            ActiveView.SetCategoryHidden(category.Id, true);
                        }
                        catch (Exception)
                        {
                        }
                    }

                }
            }
            tran.Commit();
        }
    }
}
