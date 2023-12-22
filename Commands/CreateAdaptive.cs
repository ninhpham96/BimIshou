using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using BimIshou.Utils;

namespace RevitAddin
{
    [Transaction(TransactionMode.Manual)]
    public class CreateAdaptive : ExternalCommand
    {
        public override void Execute()
        {
            var pickob = UiDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new SelectionFilter(BuiltInCategory.OST_Roofs));
            var symbol = Document.GetElement(new ElementId(305481)) as FamilySymbol;
            var roof = Document.GetElement(pickob) as FootPrintRoof;
            var solid = roof.GetSolidsInstance();
            foreach (var sol in solid)
            {
                var faces = GetTopFaces(sol);
                using (Transaction tran = new Transaction(Document, "new tran"))
                {
                    DiscardWarning(tran);
                    tran.Start();
                    foreach (Face face in faces)
                    {
                        List<XYZ> points = new List<XYZ>();
                        foreach (var edgearr in face.GetEdgesAsCurveLoops())
                            foreach (Curve edge in edgearr)
                                points.Add(edge.GetEndPoint(0));
                        CreateAdaptiveComponentInstance(Document, symbol, points);
                    }
                    tran.Commit();
                }
            }
        }
        public static List<Face> GetTopFaces(Solid solid)
        {
            var faces = new List<Face>();
            for (int i = 0; i < solid.Faces.Size; i++)
            {
                faces.Add(solid.Faces.get_Item(i));
            }
            return faces.Where(x => NormalOnMidPoint(x).DotProduct(XYZ.BasisZ) > 0).ToList();
        }
        public static XYZ NormalOnMidPoint(Face face)
        {
            return face.ComputeNormal(face.GetBoundingBox().Min / 2 + face.GetBoundingBox().Max / 2);
        }
        private void CreateAdaptiveComponentInstance(Document document, FamilySymbol symbol, List<XYZ> xyzs)
        {
            FamilyInstance instance = AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance(document, symbol);
            IList<ElementId> placePointIds = new List<ElementId>();
            placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(instance);
            int index = 0;
            foreach (ElementId id in placePointIds)
            {
                ReferencePoint point = document.GetElement(id) as ReferencePoint;
                point.Position = xyzs[index];
                ++index;
            }
        }
        public static void DiscardWarning(Transaction tr)
        {
            var op = tr.GetFailureHandlingOptions();
            var preproccessor = new DiscardAndResolveAllWarning();
            op.SetFailuresPreprocessor(preproccessor);
            tr.SetFailureHandlingOptions(op);
        }
        public class DiscardAndResolveAllWarning : IFailuresPreprocessor
        {
            public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();
                foreach (FailureMessageAccessor fma in fmas)
                {
                    if (fma.GetSeverity() == FailureSeverity.Error)
                    {
                        failuresAccessor.ResolveFailure(fma);
                        return FailureProcessingResult.ProceedWithCommit;
                    }
                    else if (fma.GetSeverity() == FailureSeverity.Warning)
                    {
                        failuresAccessor.DeleteWarning(fma);
                    }
                }
                return FailureProcessingResult.Continue;
            }
        }
    }
}