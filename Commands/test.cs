using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.UI;
using System.Diagnostics;
using KAutoHelper;
using System.Windows.Forms;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class test : ExternalCommand, IExternalCommandAvailability
    {
        public override void Execute()
        {
        }

        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            // Allow button click if there is no active selection
            if (selectedCategories.IsEmpty)
                return true;
            // Allow button click if there is at least one wall selected
            foreach (Category c in selectedCategories)
            {
                if (c.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
                    return true;
            }
            return false;
        }
    }
}
