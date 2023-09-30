using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using BimIshou.Views;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CreateCeiling : ExternalCommand
    {
        public override void Execute()
        {
            Ceiling ceiling = null;
            PostCommanCreateCeiling.Start(UiApplication);
            PostCommanCreateCeiling.OnPostableCommandModelLineEnded += PostCommanDetailLine_OnPostableCommandModelLineEnded;


            //_editPlanRegion(new ElementId(2551), lines);

        }
        private void PostCommanDetailLine_OnPostableCommandModelLineEnded(object sender, EventArgs e)
        {
            Ceiling ceiling = null;
            foreach (ElementId id in PostCommanCreateCeiling.AddedElement)
            {
                Element ele = Document.GetElement(id);
                if (ele is Ceiling)
                {
                    ceiling = (Ceiling)ele;
                    break;
                }
            }
            using (Transaction transs = new Transaction(Document, "test"))
            {
                transs.Start();


                ceiling.Copy(XYZ.BasisY * 100);


                transs.Commit();
            }
            //XYZ p1 = new XYZ(0, 0, 0);
            //XYZ p2 = new XYZ(1000, 0, 0);
            //XYZ p3 = new XYZ(1000, 1000, 0);
            //XYZ p4 = new XYZ(0, 1000, 0);

            //Line line1 = Line.CreateBound(p1, p2);
            //Line line2 = Line.CreateBound(p2, p3);
            //Line line3 = Line.CreateBound(p3, p4);
            //Line line4 = Line.CreateBound(p4, p1);

            //List<Line> lines = new List<Line>() { line1, line2, line3, line4 };

            //var xxx = PostCommanCreateCeiling.AddedElement;
            //_editPlanRegion(xxx, lines);
            PostCommanCreateCeiling.OnPostableCommandModelLineEnded -= PostCommanDetailLine_OnPostableCommandModelLineEnded;
        }

        private void Application_DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            e.GetDeletedElementIds();
        }
        private void _editPlanRegion(ElementId planRegion, List<Line> polygon)
        {
            using Transaction transTemp = new Transaction(Document, "temp");
            transTemp.Start();
            ICollection<ElementId> ids = Document.Delete(planRegion);
            transTemp.RollBack();

            //Get detailines of plan region
            List<ModelLine> detailLines = new List<ModelLine>();
            foreach (ElementId id in ids)
            {
                Element ele = Document.GetElement(id);
                if (ele is ModelLine)
                {
                    detailLines.Add(ele as ModelLine);
                }
            }

            using Transaction trans = new Transaction(Document);
            trans.Start("Change Plane Region");

            //Get 1 detailine
            var firstDetailLine = detailLines.FirstOrDefault();

            //Get z
            var z = firstDetailLine.GeometryCurve.GetEndPoint(0).Z;

            //Create new detail line
            foreach (var line in polygon)
            {
                Transform transform = Transform.CreateTranslation(new XYZ(0, 0, 1));
                var tf = Transform.Identity;
                var newID = ElementTransformUtils.CopyElement(Document, firstDetailLine.Id, XYZ.BasisY);
                var newLine = Document.GetElement(newID.First()) as ModelLine;
                var locCurve = newLine.Location as LocationCurve;
                locCurve.Curve = Line.CreateBound(transform.OfPoint(line.GetEndPoint(0)), transform.OfPoint(line.GetEndPoint(1)));
            }

            //Delete old detail lines
            Document.Delete(detailLines.Select(x => x.Id).ToList());

            trans.Commit();
        }
        private void _editPlanRegion(List<ElementId> elementIds, List<Line> polygon)
        {
            ICollection<ElementId> ids = elementIds;

            //Get detailines of plan region
            List<ModelLine> detailLines = new List<ModelLine>();
            foreach (ElementId id in ids)
            {
                Element ele = Document.GetElement(id);
                if (ele is ModelLine)
                {
                    detailLines.Add(ele as ModelLine);
                }
            }

            using Transaction trans = new Transaction(Document);
            trans.Start("Change Plane Region");

            //Get 1 detailine
            var firstDetailLine = detailLines.FirstOrDefault();

            //Get z
            var z = firstDetailLine.GeometryCurve.GetEndPoint(0).Z;

            //Create new detail line
            foreach (var line in polygon)
            {
                Transform transform = Transform.CreateTranslation(new XYZ(0, 0, 1));
                var tf = Transform.Identity;
                var newID = ElementTransformUtils.CopyElement(Document, firstDetailLine.Id, XYZ.BasisY);
                var newLine = Document.GetElement(newID.First()) as ModelLine;
                var locCurve = newLine.Location as LocationCurve;
                locCurve.Curve = Line.CreateBound(transform.OfPoint(line.GetEndPoint(0)), transform.OfPoint(line.GetEndPoint(1)));
            }

            //Delete old detail lines
            Document.Delete(detailLines.Select(x => x.Id).ToList());

            trans.Commit();
        }

    }
}
