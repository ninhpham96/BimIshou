using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BimIshou.Utils;
using MathNet.Numerics;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class OpGachSan : ExternalCommand
    {
        List<Element> elements = new();
        List<Element> tempEle = new();
        List<Line> lines = new();
        double kc = 215 / 304.8;
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
                if (!room.IsPointInRoom(newloca) && !CheckDistance(lines, newloca))
                {
                    Document.Delete(element.Id);
                }
                if (CheckDistance(lines, newloca))
                {
                    tempEle.Add(element);
                }
            }

            foreach (var element in elements)
            {
                Solid solid = element.GetSolidsInstance().FirstOrDefault();



            }
            tran.Commit();
        }
        bool CheckDistance(List<Line> lines, XYZ point)
        {
            foreach (var line in lines)
            {
                if (line.Distance(point) <= kc)
                    return true;
            }
            return false;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class SplitRegionsWithLine : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            // Helper functions
            Func<Plane, Plane> mirrorPlane = (Plane plane) =>
            {
                XYZ origin = plane.Origin;
                XYZ normal = plane.Normal;
                XYZ mirroredNormal = new XYZ(-normal.X, -normal.Y, -normal.Z);
                return Plane.CreateByNormalAndOrigin(mirroredNormal, origin);
            };

            Func<Solid, Face> findTopFace = (Solid solid) =>
            {
                foreach (PlanarFace face in solid.Faces)
                {
                    if (face.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ))
                    {
                        return face;
                    }
                }
                return null;
            };

            Func<DetailLine, Plane> createPlaneFromLine = (DetailLine line) =>
            {
                XYZ startPoint = line.GeometryCurve.GetEndPoint(0);
                XYZ endPoint = line.GeometryCurve.GetEndPoint(1);
                XYZ midPoint = (startPoint + endPoint) / 2;
                XYZ midPointWithOffset = new XYZ(midPoint.X, midPoint.Y, midPoint.Z + 10);
                return Plane.CreateByThreePoints(startPoint, endPoint, midPointWithOffset);
            };

            // Select Filled Regions
            List<ElementId> selectedRegionIds = new List<ElementId>();
            try
            {
                ISelectionFilter regionFilter = new ISelectionFilter_Regions();
                IList<Reference> regionReferences = selection.PickObjects(ObjectType.Element, regionFilter);
                selectedRegionIds = regionReferences.Select(refElem => refElem.ElementId).ToList();

                if (selectedRegionIds.Count == 0)
                {
                    TaskDialog.Show("Error", "FilledRegions weren't selected. Please Try Again.");
                    return Result.Cancelled;
                }
            }
            catch
            {
                return Result.Cancelled;
            }

            // Select DetailLine
            ElementId selectedLineId = ElementId.InvalidElementId;
            try
            {
                ISelectionFilter detailLineFilter = new ISelectionFilter_DetailLine();
                Reference lineReference = selection.PickObject(ObjectType.Element, detailLineFilter);
                selectedLineId = lineReference.ElementId;

                if (selectedLineId == ElementId.InvalidElementId)
                {
                    TaskDialog.Show("Error", "Detail Line wasn't selected. Please Try Again.");
                    return Result.Cancelled;
                }
            }
            catch
            {
                return Result.Cancelled;
            }

            // Create Plane from DetailLine
            Element selectedLine = doc.GetElement(selectedLineId);
            DetailLine detailLine = selectedLine as DetailLine;
            Plane plane = createPlaneFromLine(detailLine);

            // Modify Shape
            using (Transaction t = new Transaction(doc, "Split Regions with Line"))
            {
                t.Start();

                foreach (ElementId regionId in selectedRegionIds)
                {
                    try
                    {
                        FilledRegion region = doc.GetElement(regionId) as FilledRegion;

                        // Create Shape from Boundaries (random height)
                        List<CurveLoop> boundaries = (List<CurveLoop>)region.GetBoundaries();
                        Solid shape = GeometryCreationUtilities.CreateExtrusionGeometry(boundaries, XYZ.BasisZ, 10);

                        // Split Solid with Plane (In both directions)
                        Solid newShape1 = BooleanOperationsUtils.CutWithHalfSpace(shape, plane);
                        Solid newShape2 = BooleanOperationsUtils.CutWithHalfSpace(shape, mirrorPlane(plane));

                        // Get Top Faces of new Geometries
                        Face topFace1 = findTopFace(newShape1);
                        Face topFace2 = findTopFace(newShape2);

                        // Get Top Face Outlines
                        List<CurveLoop> outline1 = (List<CurveLoop>)topFace1.GetEdgesAsCurveLoops();
                        List<CurveLoop> outline2 = (List<CurveLoop>)topFace2.GetEdgesAsCurveLoops();

                        // Get Filled Region Type
                        ElementId frId = region.GetTypeId();

                        // Create new FilledRegions
                        FilledRegion filledRegion1 = FilledRegion.Create(doc, frId, doc.ActiveView.Id, outline1);
                        FilledRegion filledRegion2 = FilledRegion.Create(doc, frId, doc.ActiveView.Id, outline2);

                        // Delete Old Filled Region
                        if (filledRegion1 != null && filledRegion2 != null)
                        {
                            doc.Delete(region.Id);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }

    // Define Selection Filters
    public class ISelectionFilter_Regions : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            return (element is FilledRegion);
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }

    public class ISelectionFilter_DetailLine : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            return (element is DetailLine);
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }

}
