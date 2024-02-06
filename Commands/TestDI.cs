using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage;
using BimIshou.Services;
using Nice3point.Revit.Toolkit.External;
using ricaun.Revit.DI;
using System.Diagnostics;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class TestDI : ExternalCommand, IHost
    {
        public override void Execute()
        {
            Host.Container.AddRevitSingleton(UiApplication);
            Host.Container.AddSingleton<IProjectInfoService, ProjectInfoService>();

            var xxx = Host.Container.Resolve<IProjectInfoService>();
            var format = xxx.GetProjectInfo(ConstantGUID.SchemaGUID_LoggerParking, nameof(LoggerParkingEnum.Format));
            var RowNumber = xxx.GetProjectInfo(ConstantGUID.SchemaGUID_LoggerParking, nameof(LoggerParkingEnum.RowNumber));


            Debug.WriteLine(RowNumber);
        }
    }
    public enum LoggerParkingEnum
    {
        Format,
        Type,
        ProductType,
        RowNumber,
        SpanX,
        SpanY,
        SpotType,
        SpotCount,
        IndexEntry,
        SlopeType,
        SlopePos,
        FloorCount,
        SpecClimateArea,
        TotalAcreageContruction,
        AcreageAFloor,
        ElevatorCount,
        StairCount,
        GroupProductName
    }
    public static class ConstantGUID
    {
        public static Schema GetGuid(this string stringGuid)
        {
            return Schema.Lookup(new Guid(stringGuid));
        }
        public const string SchemaGUID_GDSettings = "C3DB751D-5032-44A2-BBAE-AFCDB8ABA7CF";
        public const string SchemaGUID_Input6Storage = "50DFBF8C-E182-4909-B9EC-A8FC9643DC13";
        public const string SchemaGUID_Input5Settings = "A67646E2-5EC2-4745-9B6C-47706EDF6585";
        public const string SchemaGUID_Input4BoxStorage = "C70DD2ED-78D4-44BC-8941-A299798AE1EF";
        public const string SchemaGUID_LoggerParking = "96190439-48E7-4C1C-A537-6F95EB281C78";
        public const string SchemaGUID_OutputParking = "4B9E3E1B-8D1B-4A64-8B55-A068F01ADB4E";
        public const string SchemaGUID_OutputGD = "533E9DB4-F1F8-443F-BD5B-19AA03D423D2";
        public const string SchemaGUID_CustomEntry = "0dc622fc-cdd7-4bec-8243-250d1d41ea44";
    }
}
