using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.DuplicateSheet.ViewModel;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.DuplicateSheet
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DuplicateSheetCMD : ExternalCommand
    {
        public override void Execute()
        {
            DupSheetViewModel dupSheetViewModel = new DupSheetViewModel(UiApplication);
            dupSheetViewModel.DupSheetView.ShowDialog();
        }
    }
}
