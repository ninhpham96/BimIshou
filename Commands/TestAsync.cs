using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;

namespace BimIshou.Commands;

[Transaction(TransactionMode.Manual)]
public class TestAsync : ExternalCommand
{
    public override async void Execute()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var t1 = Test1();
        var t2 = Test2();

        stopwatch.Stop();
        await Task.WhenAll(t1, t2);
        var t3 = Test3();

        Debug.WriteLine(stopwatch.Elapsed);
    }
    public async Task<int> Test1()
    {
        Task<int> t = new Task<int>(
            () =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Debug.Write($"test1 {i}");
                }
                return 0;
            }
        );
        t.Start();
        var res = await t;
        return res;
    }
    public async Task<int> Test2()
    {
        Task<int> t = new Task<int>(
            () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Debug.Write($"test2 {i}");
                }
                return 0;
            }
        );
        t.Start();
        var res = await t;
        return res;
    }
    public async Task<int> Test3()
    {
        Task<int> t = new Task<int>(
            () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Debug.Write($"test3 {i}");
                }
                return 0;
            }
        );
        t.Start();
        var res = await t;
        return res;
    }
    public IEnumerable<Grid> GetGridX()
    {
        return new FilteredElementCollector(Document)
                .OfClass(typeof(Grid))
                .WhereElementIsNotElementType()
                .Cast<Grid>()
                .Where(grid => grid.Name.Contains("X"))
                .OrderBy(grid => int.Parse(grid.Name.Replace("X", "")));
    }
    public IEnumerable<Grid> GetGridY()
    {
        return new FilteredElementCollector(Document)
                .OfClass(typeof(Grid))
                .WhereElementIsNotElementType()
                .Cast<Grid>()
                .Where(grid => grid.Name.Contains("Y"))
                .OrderBy(grid => int.Parse(grid.Name.Replace("Y", "")));
    }
    public HashSet<List<XYZ>> GetAreaCanPutPipeForSlope()
    {
        var gridsX = GetGridX().ToList();
        var gridsY = GetGridY().ToList();
        HashSet<List<XYZ>> result = new HashSet<List<XYZ>>();
        HashSet<List<XYZ>> result1 = new HashSet<List<XYZ>>();

        for (var j = 0; j < gridsY.Count - 1; j++)
        {
            List<XYZ> xyzs1 = new List<XYZ>();
            XYZ p1 = gridsX[0].Curve.Intersection(gridsY[j].Curve);
            XYZ p2 = gridsX[0].Curve.Intersection(gridsY[j + 1].Curve);
            XYZ p3 = gridsX[1].Curve.Intersection(gridsY[j + 1].Curve);
            XYZ p4 = gridsX[1].Curve.Intersection(gridsY[j].Curve);
            xyzs1.Add(p1);
            xyzs1.Add(p2);
            xyzs1.Add(p3);
            xyzs1.Add(p4);

            List<XYZ> xyzs2 = new List<XYZ>();
            XYZ p5 = gridsX[gridsX.Count - 2].Curve.Intersection(gridsY[j].Curve);
            XYZ p6 = gridsX[gridsX.Count - 2].Curve.Intersection(gridsY[j + 1].Curve);
            XYZ p7 = gridsX[gridsX.Count - 1].Curve.Intersection(gridsY[j + 1].Curve);
            XYZ p8 = gridsX[gridsX.Count - 1].Curve.Intersection(gridsY[j].Curve);
            xyzs2.Add(p5);
            xyzs2.Add(p6);
            xyzs2.Add(p7);
            xyzs2.Add(p8);

            result.Add(xyzs1);
            result.Add(xyzs2);
        }
        for (var j = 0; j < gridsX.Count - 1; j++)
        {
            List<XYZ> xyzs1 = new List<XYZ>();
            XYZ p1 = gridsY[0].Curve.Intersection(gridsX[j].Curve);
            XYZ p2 = gridsY[0].Curve.Intersection(gridsX[j + 1].Curve);
            XYZ p3 = gridsY[1].Curve.Intersection(gridsX[j + 1].Curve);
            XYZ p4 = gridsY[1].Curve.Intersection(gridsX[j].Curve);
            xyzs1.Add(p1);
            xyzs1.Add(p2);
            xyzs1.Add(p3);
            xyzs1.Add(p4);

            List<XYZ> xyzs2 = new List<XYZ>();
            XYZ p5 = gridsY[gridsY.Count - 2].Curve.Intersection(gridsX[j].Curve);
            XYZ p6 = gridsY[gridsY.Count - 2].Curve.Intersection(gridsX[j + 1].Curve);
            XYZ p7 = gridsY[gridsY.Count - 1].Curve.Intersection(gridsX[j + 1].Curve);
            XYZ p8 = gridsY[gridsY.Count - 1].Curve.Intersection(gridsX[j].Curve);
            xyzs2.Add(p5);
            xyzs2.Add(p6);
            xyzs2.Add(p7);
            xyzs2.Add(p8);

            result.Add(xyzs1);
            result.Add(xyzs2);
        }

        //foreach (List<XYZ> res in result)
        //{
        //    if (CheckParkingInsidePolygon(res))
        //    {
        //        result1.Add(res);
        //    }
        //}
        Debug.WriteLine(result1.Count);
        return result1;
    }
}
