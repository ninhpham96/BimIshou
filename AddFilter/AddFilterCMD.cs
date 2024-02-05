using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

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
        if (subView.DialogResult == true)
        {
            viewmodel.ShowMainview();
        }
        else
        {
            viewmodel.SubView.Close();
        }
    }
}
