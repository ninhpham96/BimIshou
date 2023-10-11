using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;

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
