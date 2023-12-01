using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.ApplicationServices;
using System.Diagnostics;

namespace RevitAddin
{
    [Transaction(TransactionMode.Manual)]
    public class TestCreateSchema : ExternalCommand
    {
        public override void Execute()
        {
        }    }

    [Transaction(TransactionMode.Manual)]
    public class SplitDuctV1Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            try
            {
                List<Element> element = new List<Element>();
                List<Connector> connectors = new List<Connector>();
                List<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element).ToList();
                foreach (Reference elementrReference in references)
                {
                    Element elementDuct = doc.GetElement(elementrReference);
                    element.Add(elementDuct);
                }
                double Lengthft = 1110 / 304.8;
                Duct duct1 = null;
                Duct duct2 = null;
                Duct ductmain = null;
                foreach (Element ductElement in element)
                {
                    ductmain = ductElement as Duct;
                    Curve curve = ((LocationCurve)ductmain.Location).Curve;
                    using (Transaction t = new Transaction(doc, "Chia Duct test"))
                    {
                        t.Start();
                        while (curve.Length > Lengthft)
                        {
                            XYZ startPoint = curve.GetEndPoint(0);
                            XYZ endPoint = curve.GetEndPoint(1);
                            XYZ direction = endPoint.Subtract(startPoint).Normalize();
                            XYZ breakPoint = startPoint.Add(direction.Multiply(Lengthft));

                            ElementId newductId = MechanicalUtils.BreakCurve(doc, ductmain.Id, breakPoint);
                            duct1 = ductElement as Duct;
                            duct2 = doc.GetElement(newductId) as Duct;
                            curve = ((LocationCurve)ductElement.Location).Curve;
                            if (curve.Length < Lengthft)
                            {
                                break;
                            }
                            doc.Regenerate();

                            var unusedConnectors1 = duct1.ConnectorManager.UnusedConnectors;
                            var unusedConnectors2 = duct2.ConnectorManager.UnusedConnectors;
                            foreach (var connector in unusedConnectors1)
                            {
                                if (connector is Connector con)
                                {
                                    connectors.Add(con);
                                    Debug.WriteLine(con.Origin);
                                }
                            }
                            foreach (Connector connector in unusedConnectors2)
                            {
                                if (connector is Connector con)
                                {
                                    connectors.Add(con);
                                    Debug.WriteLine(con.Origin);
                                }
                            }
                            break;
                            FamilyInstance familyInstance = doc.Create.NewUnionFitting(connectors[1], connectors[2]);

                        }
                        t.Commit();
                    }

                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }

        }
    }
}