using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class PostCommanAlignedDimension : ExternalCommand
    {
        //public static bool ReadyToAdd;
        public static List<ElementId> AddedElement;
        public static UIDocument UIDoc;
        public static UIApplication uiapp;
        public static Autodesk.Revit.ApplicationServices.Application app;
        public static event EventHandler OnPostableCommandModelLineEnded;
        public override void Execute()
        {
            //ReadyToAdd = true;
            AddedElement = new List<ElementId>();
            OnPostableCommandModelLineEnded = null;
            RevitCommandEndedMonitor revitCommandEndedMonitor = new RevitCommandEndedMonitor(uiapp);
            revitCommandEndedMonitor.CommandEnded += RevitCommandEndedMonitor_CommandEnded;
            app.DocumentChanged += Application_DocumentChanged;

            RevitCommandId cmdModelLine_id = RevitCommandId.LookupPostableCommandId(PostableCommand.AlignedDimension);
            uiapp.PostCommand(cmdModelLine_id);
        }
        /// <summary>
        /// Register OnPostableCommandModelLineEnded after start
        /// Start on any Cmd
        /// Finist => List<DetailLine> AddedDetailLines
        /// </summary>
        /// <param name="uidoc">UIDocument</param>
        public static void Start(UIApplication uiap)
        {
            uiapp = uiap;
            UIDoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            new PostCommanAlignedDimension().Execute();
        }
        private void RevitCommandEndedMonitor_CommandEnded(object sender, EventArgs e)
        {
            app.DocumentChanged -= Application_DocumentChanged;
            OnPostableCommandModelLineEnded?.Invoke(this, EventArgs.Empty);
        }
        private void Application_DocumentChanged(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
        {
            AddedElement.AddRange(e.GetAddedElementIds());            
        }
    }

    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class PostCommanCreateCeiling : ExternalCommand
    {
        //public static bool ReadyToAdd;
        public static List<ElementId> AddedElement;
        public static UIDocument UIDoc;
        public static UIApplication uiapp;
        public static Autodesk.Revit.ApplicationServices.Application app;
        public static event EventHandler OnPostableCommandModelLineEnded;
        public override void Execute()
        {
            //ReadyToAdd = true;
            AddedElement = new List<ElementId>();
            OnPostableCommandModelLineEnded = null;
            RevitCommandEndedMonitor revitCommandEndedMonitor = new RevitCommandEndedMonitor(uiapp);
            revitCommandEndedMonitor.CommandEnded += RevitCommandEndedMonitor_CommandEnded;
            app.DocumentChanged += Application_DocumentChanged;

            RevitCommandId cmdModelLine_id = RevitCommandId.LookupPostableCommandId(PostableCommand.AutomaticCeiling);
            uiapp.PostCommand(cmdModelLine_id);
        }
        /// <summary>
        /// Register OnPostableCommandModelLineEnded after start
        /// Start on any Cmd
        /// Finist => List<DetailLine> AddedDetailLines
        /// </summary>
        /// <param name="uidoc">UIDocument</param>
        public static void Start(UIApplication uiap)
        {
            uiapp = uiap;
            UIDoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            new PostCommanCreateCeiling().Execute();
        }
        private void RevitCommandEndedMonitor_CommandEnded(object sender, EventArgs e)
        {
            app.DocumentChanged -= Application_DocumentChanged;
            OnPostableCommandModelLineEnded?.Invoke(this, EventArgs.Empty);
        }
        private void Application_DocumentChanged(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
        {
            AddedElement.AddRange(e.GetAddedElementIds());
        }
    }
    /// <summary>
    /// https://forums.autodesk.com/t5/revit-api-forum/how-do-i-continue-after-postcommand/td-p/10981164
    /// https://gist.github.com/ricaun/cc4f0a39b36006883f091bc7f0fc3d35
    /// </summary>
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