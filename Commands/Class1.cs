using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BimIshou.Commands
{
    internal class Class1
    {
        Document doc;
        private void _editPlanRegion(Element planRegion, List<Line> polygon)
        {
            using Transaction transTemp = new Transaction(doc, "temp");
            transTemp.Start();
            ICollection<ElementId> ids = doc.Delete(planRegion.Id);
            transTemp.RollBack();

            //Get detailines of plan region
            List<DetailLine> detailLines = new List<DetailLine>();
            foreach (ElementId id in ids)
            {
                Element ele = doc.GetElement(id);
                if (ele is DetailLine)
                {
                    detailLines.Add(ele as DetailLine);
                }
            }

            using Transaction trans = new Transaction(doc);
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
                var newID = ElementTransformUtils.CopyElement(doc, firstDetailLine.Id, XYZ.BasisY);
                var newLine = doc.GetElement(newID.First()) as DetailLine;
                var locCurve = newLine.Location as LocationCurve;
                locCurve.Curve = Line.CreateBound(transform.OfPoint(line.GetEndPoint(0)), transform.OfPoint(line.GetEndPoint(1)));
            }

            //Delete old detail lines
            doc.Delete(detailLines.Select(x => x.Id).ToList());

            trans.Commit();
        }
    }
}
