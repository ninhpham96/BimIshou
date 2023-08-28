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
            #region Dimensions
            var panel1 = Application.CreatePanel("Dimensions", "BimIshou");
            
            var dimCh = panel1.AddPushButton<DimCH>("Dim CH");
            dimCh.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            dimCh.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var dimFuniture = panel1.AddPushButton<DimFuniture>("Dim Funiture");
            dimFuniture.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            dimFuniture.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DimW = panel1.AddPushButton<DimW>("Dim W");
            DimW.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimW.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DimDoorAndWindow = panel1.AddPushButton<AutoDimDoor>("Dim DoorAndWindow");
            DimDoorAndWindow.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimDoorAndWindow.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DimTT = panel1.AddPushButton<DimTT>("Dim 有効");
            DimTT.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DimTT.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var MuliCutFilter = panel1.AddPushButton<MultiCutCMD>("Multi Cut");
            MuliCutFilter.SetImage("/BimIshou;component/Resources/Icons/MultiCut16.png");
            MuliCutFilter.SetLargeImage("/BimIshou;component/Resources/Icons/MultiCut32.png");

            var AutoTagRoomCMD = panel1.AddPushButton<AutoTagRoomCMD>("Tag Room");
            AutoTagRoomCMD.SetImage("/BimIshou;component/Resources/Icons/AutoTag16.png");
            AutoTagRoomCMD.SetLargeImage("/BimIshou;component/Resources/Icons/AutoTag32.png");

            var DuplicateSheetCMD = panel1.AddPushButton<DuplicateSheetCMD>("Duplicate Sheets");
            DuplicateSheetCMD.SetImage("/BimIshou;component/Resources/Icons/RibbonIcon16.png");
            DuplicateSheetCMD.SetLargeImage("/BimIshou;component/Resources/Icons/RibbonIcon32.png");

            var DimDoubuchi = panel1.AddSplitButton("dim", "dimdouchi");
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
            #endregion

            #region Tool Vo Tri
            var panel2 = Application.CreatePanel("Tool Vô Tri", "BimIshou");

            var tagSite = panel2.AddPushButton<TagSite>("Tag Level Site");
            tagSite.ToolTip = "Tag các mốc cao độ từ file link Gaiko!";
            tagSite.SetLargeImage("/BimIshou;component/Resources/Icons/TagSite32.png");
            var tagDim = panel2.AddPushButton<TextDim>("Text 有効");
            tagDim.ToolTip = "Text thêm chữ 有効 vào phía trước đoạn Dimension được chọn!";
            tagDim.SetLargeImage("/BimIshou;component/Resources/Icons/TextDim32.png");



            #endregion
        }
    }
}