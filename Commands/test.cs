using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class test : ExternalCommand
    {
        public override void Execute()
        {
            List<XYZ> points = new List<XYZ>();
            List<TextNote> texts = new List<TextNote>();
            List<Data> datas = new List<Data>();

            List<ElementId> elementsIds = UiDocument.Selection.GetElementIds()?.ToList() ?? new List<ElementId>();
            List<Element> selectedElements = elementsIds.Count > 0 ? elementsIds.Select(x => Document.GetElement(x)).ToList() : new List<Element>();

            foreach (Element _line in selectedElements)
            {
                var line = _line as ModelLine;
                if (line == null) continue;
                XYZ p1 = line.GeometryCurve.GetEndPoint(0);
                XYZ p2 = line.GeometryCurve.GetEndPoint(1);
                if (!points.Exists(x => x.IsAlmostEqualTo(p1))) points.Add(p1);
                if (!points.Exists(x => x.IsAlmostEqualTo(p2))) points.Add(p2);
            }
            foreach (Element _text in selectedElements)
            {
                var text = _text as TextNote;
                if (text == null) continue;
                texts.Add(text);
            }
            foreach (TextNote text in texts)
            {
                var value = text.Text.Split('+');
                var loca = text.GetLeaders().First().End;
                foreach (var p in points)
                {
                    if (loca.DistanceTo(p) < 10)
                    {
                        datas.Add(new Data(text.Id.IntegerValue, loca, value[0], value[1]));
                    }
                }
            }
            datas.OrderByDescending(x => x.id);
            MessageBox.Show(datas[0].s1);
        }
        public record Data(int id, XYZ loca, string s1, string s2)
        {
            public int id { get; set; } = id;
            public XYZ loca { get; set; } = loca;
            public string s1 { get; set; } = s1;
            public string s2 { get; set; } = s2;
        }
    }
}
