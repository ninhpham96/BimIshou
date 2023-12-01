using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands.ChieucaoTB;

[Transaction(TransactionMode.Manual)]
internal class AverageAltitudeCmd : ExternalCommand
{
    public override void Execute()
    {
        AverageAltitudeModel Model = new AverageAltitudeModel(UiDocument);
        Main main = new Main() { DataContext = Model };
        if (Model.itemSource.Any())
            main.ShowDialog();
    }
}
