using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace RevitAddin
{
    [Transaction(TransactionMode.Manual)]
    public class CommandMoveEnd : ExternalCommand
    {
        RevitCommandEndedMonitor revitCommandEndedMonitor;
        IList<Reference> selectRooms;
        public List<ElementId> AddedElement { get; set; } = new List<ElementId>();
        List<ElementId> Ceillings = new();
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
                    Ceillings.Add(ele.Id);
                    break;
                }
            }
            using (TransactionGroup tranG = new TransactionGroup(Document, "Auto Create Ceilling"))
            {
                tranG.Start();
                using (Transaction transs = new Transaction(Document, "test"))
                {
                    transs.Start();
                    var opt = new SpatialElementBoundaryOptions();
                    for (int i = 1; i < selectRooms.Count; i++)
                    {
                        var tempCeilling = (Document.GetElement(Ceillings.FirstOrDefault())).Copy(XYZ.BasisY * 100 * i).FirstOrDefault();
                        Ceillings.Add(tempCeilling);
                    }
                    transs.Commit();
                }
                for (int i = 0; i < selectRooms.Count; i++)
                {
                    ICollection<ElementId> ids;
                    List<Line> polygon = new List<Line>();
                    Room room = Document.GetElement(selectRooms[i]) as Room;
                    IList<IList<BoundarySegment>> loops = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                    foreach (IList<BoundarySegment> loop in loops)
                    {
                        foreach (BoundarySegment seg in loop)
                        {
                            Line line = seg.GetCurve() as Line;
                            polygon.Add(line);
                        }
                    }
                    using (Transaction transs = new Transaction(Document, "temp"))
                    {
                        transs.Start();
                        ids = Document.Delete(Ceillings[i]);
                        transs.RollBack();
                    }
                    using (Transaction trans = new Transaction(Document, "Change Sketch Ceilling"))
                    {
                        trans.Start();
                        EditCeilling(ids, polygon);
                        trans.Commit();
                    }
                }
                tranG.Assimilate();
            }
        }
        private void EditCeilling(ICollection<ElementId> elementIds, List<Line> polygon)
        {
            List<ModelLine> detailLines = new List<ModelLine>();
            foreach (ElementId id in elementIds)
            {
                Element ele = Document.GetElement(id);
                if (ele is ModelLine)
                {
                    detailLines.Add(ele as ModelLine);
                }
            }
            var firstDetailLine = detailLines.FirstOrDefault();
            foreach (var line in polygon)
            {
                Transform transform = Transform.CreateTranslation(new XYZ(0, 0, 1));
                var tf = Transform.Identity;
                var newID = ElementTransformUtils.CopyElement(Document, firstDetailLine.Id, XYZ.BasisY);
                var newLine = Document.GetElement(newID.First()) as ModelLine;
                var locCurve = newLine.Location as LocationCurve;
                locCurve.Curve = Line.CreateBound(transform.OfPoint(line.GetEndPoint(0)), transform.OfPoint(line.GetEndPoint(1)));
            }
            Document.Delete(detailLines.Select(x => x.Id).ToList());
        }
    }

    public class RevitCommandEndedMonitor
    {
        private readonly UIApplication _revitUiApplication;

        private bool _initializingCommandMonitor;

        public event EventHandler CommandEnded;

        public RevitCommandEndedMonitor(UIApplication revituiApplication)
        {
            _revitUiApplication = revituiApplication;
            _initializingCommandMonitor = true;
            _revitUiApplication.Idling += OnRevitUiApplicationIdling;
        }
        private void OnRevitUiApplicationIdling(object sender, IdlingEventArgs idlingEventArgs)
        {
            if (_initializingCommandMonitor)
            {
                _initializingCommandMonitor = false;
                return;
            }
            _revitUiApplication.Idling -= OnRevitUiApplicationIdling;
            OnCommandEnded();
        }
        protected virtual void OnCommandEnded()
        {
            CommandEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}