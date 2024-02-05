using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class OpGachSan : ExternalCommand
    {
        List<Element> elements = new();
        List<Element> tempEle = new();
        List<Line> lines = new();
        double Max = int.MinValue;
        public override void Execute()
        {
            double delta = (300 + 2) / 304.8;
            Room room = Document.GetElement(new ElementId(3096)) as Room;
            var option = new SpatialElementBoundaryOptions();
            var bound = room.GetBoundarySegments(option);
            FamilyInstance viendinhvi = Document.GetElement(new ElementId(8828)) as FamilyInstance;
            XYZ loca = (viendinhvi.Location as LocationPoint).Point;

            foreach (var Segments in bound)
            {
                foreach (var seg in Segments)
                {
                    Curve curve = seg.GetCurve();
                    lines.Add(curve as Line);
                    if (curve.GetEndPoint(0).DistanceTo(loca) > Max)
                        Max = curve.GetEndPoint(0).DistanceTo(loca);
                }
            }
            Transaction tran = new Transaction(Document, "test");
            tran.Start();
            for (int i = 1; i < Max / delta; i++)
            {
                var newEle1 = viendinhvi.Copy(viendinhvi.HandOrientation * i * delta).FirstOrDefault().ToElement(Document);
                var newEle2 = viendinhvi.Copy(-viendinhvi.HandOrientation * i * delta).FirstOrDefault().ToElement(Document);
                var newEle3 = viendinhvi.Copy(viendinhvi.FacingOrientation * i * delta).FirstOrDefault().ToElement(Document);
                var newEle4 = viendinhvi.Copy(-viendinhvi.FacingOrientation * i * delta).FirstOrDefault().ToElement(Document);
                elements.Add(newEle1);
                elements.Add(newEle2);
                elements.Add(newEle3);
                elements.Add(newEle4);
                for (int j = 1; j < Max / delta; j++)
                {
                    var newEle5 = newEle1.Copy(viendinhvi.FacingOrientation * j * delta).FirstOrDefault().ToElement(Document);
                    var newEle6 = newEle1.Copy(-viendinhvi.FacingOrientation * j * delta).FirstOrDefault().ToElement(Document);
                    var newEle7 = newEle2.Copy(viendinhvi.FacingOrientation * j * delta).FirstOrDefault().ToElement(Document);
                    var newEle8 = newEle2.Copy(-viendinhvi.FacingOrientation * j * delta).FirstOrDefault().ToElement(Document);
                    elements.Add(newEle5);
                    elements.Add(newEle6);
                    elements.Add(newEle7);
                    elements.Add(newEle8);
                }
            }
            foreach (var element in elements)
            {
                XYZ newloca = (element.Location as LocationPoint).Point;
                if (!room.IsPointInRoom(newloca) && !CheckCollision(lines, element.GetSolidsInstance().FirstOrDefault()))
                {
                    Document.Delete(element.Id);
                    continue;
                }
                if (CheckCollision(lines, element.GetSolidsInstance().FirstOrDefault()))
                {
                    tempEle.Add(element);
                }
            }

            foreach (var element in tempEle)
            {
                Solid solid = element.GetSolidsInstance().FirstOrDefault();
                Solid Shape = null;
                bool checkDelete = false;
                foreach (var line in lines)
                {
                    if (solid.IntersectWithCurve(line, new SolidCurveIntersectionOptions()).SegmentCount == 1)
                    {
                        checkDelete = true;
                        Plane plane = CreatePlaneFromLine(line);
                        Shape = BooleanOperationsUtils.CutWithHalfSpace(solid, MirrorPlane(plane));
                        if (Shape == null) continue;
                        solid = Shape;
                    }
                }
                if (Shape != null)
                {
                    DirectShape ds = DirectShape.CreateElement(Document, new ElementId(BuiltInCategory.OST_GenericModel));
                    ds.ApplicationId = "Application id";
                    ds.ApplicationDataId = "Geometry object id";
                    ds.SetShape(new GeometryObject[] { Shape });
                }
                if (checkDelete)
                {
                    Document.Delete(element.Id);
                    checkDelete = false;
                }
            }
            tran.Commit();
        }
        bool CheckCollision(List<Line> lines, Solid solid)
        {
            foreach (var line in lines)
            {
                if (solid.IntersectWithCurve(line, new SolidCurveIntersectionOptions()).SegmentCount == 1)
                    return true;
            }
            return false;
        }
        Plane CreatePlaneFromLine(Line line)
        {
            XYZ startPoint = line.GetEndPoint(0);
            XYZ endPoint = line.GetEndPoint(1);
            XYZ midPoint = (startPoint + endPoint) / 2;
            XYZ midPointWithOffset = new XYZ(midPoint.X, midPoint.Y, midPoint.Z + 10);
            return Plane.CreateByThreePoints(startPoint, endPoint, midPointWithOffset);
        }
        Plane MirrorPlane(Plane plane)
        {
            XYZ origin = plane.Origin;
            XYZ normal = plane.Normal;
            XYZ mirroredNormal = new XYZ(-normal.X, -normal.Y, -normal.Z);
            return Plane.CreateByNormalAndOrigin(mirroredNormal, origin);
        }
    }
}