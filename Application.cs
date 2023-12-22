using Autodesk.Revit.UI;
using BimIshou.AutoTag;
using BimIshou.Commands;
using BimIshou.Commands.A14;
using BimIshou.Commands.ChieucaoTB;
using BimIshou.Commands.DimDoubuchi;
using BimIshou.DuplicateSheet;
using BimIshou.TestAtt;

namespace BimIshou
{
    [UsedImplicitly]
    public class Application : ApplicationBase
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

            var DimDoorAndWindow = panel.AddPushButton<AutoDimDoor1>("Dim DoorAndWindow 1");
            DimDoorAndWindow.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimDoorAndWindow.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DimDoorAndWindow1 = panel.AddPushButton<AutoDimDoor2>("Dim DoorAndWindow 2");
            DimDoorAndWindow1.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimDoorAndWindow1.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

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

            var CreateWallRegion = panel.AddSplitButton("A14", "Wall Region");
            DimDoubuchi.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimDoubuchi.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var button4 = CreateWallRegion.AddPushButton<CheckWallTypeUsed>("Check Wall Type Used");
            button4.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            button4.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var button5 = CreateWallRegion.AddPushButton<CreateViewLegend>("Create View Legend");
            button5.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            button5.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var button6 = CreateWallRegion.AddPushButton<PutFillRegionToA14>("Put Fill Region ToA14");
            button6.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            button6.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var button7 = CreateWallRegion.AddPushButton<CreateKeyPlaneForA14>("Create KeyPlane For A14");
            button7.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            button7.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var CreateCeiling = panel.AddPushButton<CreateCeiling>("CreateCeiling");
            CreateCeiling.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            CreateCeiling.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var CreateCaodoTB = panel.AddPushButton<AverageAltitudeCmd>("Cao do trung binh");
            CreateCaodoTB.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            CreateCaodoTB.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}