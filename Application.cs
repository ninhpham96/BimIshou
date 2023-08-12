using BimIshou.Commands;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou
{
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            var panel = Application.CreatePanel("Dimmension", "BimIshou");

            var dimCh = panel.AddPushButton<DimCH>("Dim CH");
            dimCh.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            dimCh.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var dimFuniture = panel.AddPushButton<DimFuniture>("Dim Funiture");
            dimFuniture.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            dimFuniture.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DimW = panel.AddPushButton<DimW>("Dim W");
            DimW.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimW.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DimTT = panel.AddPushButton<DimTT>("Dim 有効");
            DimTT.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimTT.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}