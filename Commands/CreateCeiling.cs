using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
public class CreateCeiling : ExternalCommand
{
    RevitCommandEndedMonitor revitCommandEndedMonitor;
    IList<Reference> selectRooms;
    public List<ElementId> AddedElement { get; set; } = new List<ElementId>();
    List<ElementId> Ceilings = new();
    public override void Execute()
    {
        try
        {
            selectRooms = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new SelectionFilter(BuiltInCategory.OST_Rooms, true));
            revitCommandEndedMonitor = new RevitCommandEndedMonitor(UiApplication);
            revitCommandEndedMonitor.CommandEnded += OnCommandEnded;
            Application.DocumentChanged += Application_DocumentChanged;
            UiApplication.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.AutomaticCeiling));
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return;
        }
    }
    private void Application_DocumentChanged(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
    {
        AddedElement.AddRange(e.GetAddedElementIds());
    }
    private void OnCommandEnded(object sender, EventArgs e)
    {
        Application.DocumentChanged -= Application_DocumentChanged;
        revitCommandEndedMonitor.CommandEnded -= OnCommandEnded;
        foreach (var item in AddedElement)
        {
            var ele = Document.GetElement(item);
            if (ele is Ceiling)
            {
                Ceilings.Add(ele.Id);
                break;
            }
        }
        using (TransactionGroup tranG = new TransactionGroup(Document, "Auto Create Ceiling"))
        {
            tranG.Start();
            using (Transaction transs = new Transaction(Document, "test"))
            {
                transs.Start();
                for (int i = 1; i < selectRooms.Count; i++)
                {
                    Ceilings.Add((Document.GetElement(Ceilings.FirstOrDefault())).Copy(XYZ.BasisY * 100 * i).FirstOrDefault());
                }
                transs.Commit();
            }
            for (int i = 0; i < selectRooms.Count; i++)
            {
                ICollection<ElementId> ids;
                List<Curve> polygon = new List<Curve>();
                Room room = Document.GetElement(selectRooms[i]) as Room;
                double heightCeiling = double.Parse(room.GetParameter("仕上表 CH").AsString());
                IList<IList<BoundarySegment>> loops = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                foreach (IList<BoundarySegment> loop in loops)
                {
                    foreach (BoundarySegment seg in loop)
                    {
                        Curve line = seg.GetCurve();
                        polygon.Add(line);
                    }
                }
                using (Transaction transs = new Transaction(Document, "temp"))
                {
                    transs.Start();
                    ids = Document.Delete(Ceilings[i]);
                    transs.RollBack();
                    transs.Start();
                    Document.GetElement(Ceilings[i]).GetParameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM).Set(heightCeiling.FromMillimeters());
                    EditCeiling(ids, polygon);
                    transs.Commit();
                }
            }
            tranG.Assimilate();
        }
    }
    private void EditCeiling(ICollection<ElementId> elementIds, List<Curve> polygon)
    {
        List<ElementId> detailLines = new List<ElementId>();
        foreach (ElementId id in elementIds)
        {
            Element ele = Document.GetElement(id);
            if (ele is ModelLine)
            {
                detailLines.Add(id);
            }
        }
        foreach (var line in polygon)
        {
            Transform transform = Transform.CreateTranslation(new XYZ(0, 0, 1));
            var tf = Transform.Identity;
            var newID = ElementTransformUtils.CopyElement(Document, detailLines.FirstOrDefault(), XYZ.BasisY);
            var newLine = Document.GetElement(newID.First()) as ModelLine;
            var locCurve = newLine.Location as LocationCurve;
            locCurve.Curve = Line.CreateBound(transform.OfPoint(line.GetEndPoint(0)), transform.OfPoint(line.GetEndPoint(1)));
        }
        Document.Delete(detailLines);
    }
}
