using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Renamefamilytype
{
    [Transaction(TransactionMode.Manual)]
    internal class Cmd : ExternalCommand
    {
        public override void Execute()
        {
            Viewmodel viewmodel = new Viewmodel(Document);
            View view = new View() { DataContext = viewmodel };
            view.ShowDialog();
        }
    }
}
