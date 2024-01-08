using Autodesk.Revit.Attributes;
using BimIshou.Commands.DimDoubuchi.View;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou
{
    [Transaction(TransactionMode.Manual)]
    internal class SettingDim : ExternalCommand
    {
        public override void Execute()
        {
            Setting view = new Setting();
            view.ShowDialog();
        }
    }
}
