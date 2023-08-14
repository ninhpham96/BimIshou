using Autodesk.Revit.UI;
using BimIshou.AutoTag;
using BimIshou.Commands;
using BimIshou.Commands.DimDoubuchi;
using BimIshou.DuplicateSheet;
using Nice3point.Revit.Extensions;
using Nice3point.Revit.Toolkit.External;
using System.Reflection;

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

            var MuliCutFilter = panel.AddPushButton<MultiCutCMD>("Multi Cut");
            MuliCutFilter.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            MuliCutFilter.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var AutoTagRoomCMD = panel.AddPushButton<AutoTagRoomCMD>("Tag Room");
            AutoTagRoomCMD.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            AutoTagRoomCMD.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DuplicateSheetCMD = panel.AddPushButton<DuplicateSheetCMD>("Duplicate Sheets");
            DuplicateSheetCMD.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DuplicateSheetCMD.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DimDoubuchi = panel.AddSplitButton("dim", "dimdouchi");
            DimDoubuchi.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimDoubuchi.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var button1 = DimDoubuchi.AddPushButton<DimDoubuchi>("DimDoubuchi 1");
            button1.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            button1.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var button2 = DimDoubuchi.AddPushButton<SettingDim>("Setting");
            button2.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            button2.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var button3 = DimDoubuchi.AddPushButton<DimDoorOrWindowInDoubuchi>("DimDoubuchi 2");
            button3.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            button3.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}