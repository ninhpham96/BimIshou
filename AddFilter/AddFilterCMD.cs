using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.AddFilter;

[Transaction(TransactionMode.Manual)]
public class AddFilterCMD : ExternalCommand
{
    public override void Execute()
    {
        var mainView = new MainView();
        var subView = new SubView();
        AddFilterVM viewmodel = new AddFilterVM(mainView, subView, Document);
        viewmodel.ShowSubview();
        using (TransactionGroup tranG = new TransactionGroup(Document, "Add filter"))
        {
            tranG.Start();
            if (subView.DialogResult == true)
            {
                viewmodel.ShowMainview();
            }
            else
            {
                viewmodel.SubView.Close();
                tranG.RollBack();
                return;
            }
            if (mainView.DialogResult == true)
            {
                viewmodel.MainView.Close();
                tranG.Assimilate();
                MessageBox.Show("Add Success");
                return;
            }
            else
            {
                viewmodel.SubView.Close();
                tranG.RollBack();
                return;
            }
        }
    }
}
