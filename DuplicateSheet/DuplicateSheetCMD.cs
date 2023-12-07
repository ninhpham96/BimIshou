using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.DuplicateSheet.ViewModel;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.DuplicateSheet
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DuplicateSheetCMD : ExternalCommand
    {
        public override void Execute()
        {

            using (TransactionGroup tranG = new TransactionGroup(Document))
            {
                tranG.Start();
                try
                {
                    DupSheetViewModel dupSheetViewModel = new DupSheetViewModel(UiApplication);
                    dupSheetViewModel.DupSheetView.ShowDialog();
                    tranG.Assimilate();
                }
                catch (Exception e)
                {
                    tranG.RollBack();
                    MessageBox.Show(e.Message);
                }
            }

        }
    }
}
