using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]

    public class SetWorkPlan : ExternalCommand
    {
        public override void Execute()
        {
            Plane plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 1, 1), XYZ.Zero);

            using (Transaction tran = new Transaction(Document, "new tran"))
            {
                tran.Start();
                SketchPlane sp = SketchPlane.Create(Document, plane);
                Document.ActiveView.SketchPlane = sp;
                Document.ActiveView.ShowActiveWorkPlane();
                tran.Commit();
            }
        }
    }
}
