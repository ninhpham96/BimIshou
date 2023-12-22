using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Microsoft.Office.Interop.Excel;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Windows.ToolBars;
using Autodesk.Windows;
using System.Diagnostics;
using System.Windows;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class test : ExternalCommand
    {
        public override void Execute()
        {
            ComponentManager.Ribbon.MouseDoubleClick += Ribbon_MouseDoubleClick; ;

            using (Transaction tran = new Transaction(Document, "new tran"))
            {
                tran.Start();

                tran.Commit();
            }

        }

        private void Ribbon_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("1");
        }
    }
}
