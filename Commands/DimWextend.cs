using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using BimIshou.Utils;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class DimWextend : ExternalCommand
    {
        public override void Execute()
        {
            List<BuiltInCategory> categories = new List<BuiltInCategory>() { BuiltInCategory.OST_Doors,BuiltInCategory.OST_Windows};
            SelectionFilter filter = new SelectionFilter(categories, true);

            var selectedeles = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element,
                                filter,
                                "Chọn các đối tượng muốn Dim");

            Dimension dim = null;
            var ids = new List<ElementId>();
            var refs = dim.References;
            try
            {
                foreach (Reference item in refs)
                {
                    var temp = Document.GetElement(item);
                    if (temp.Category.Id.IntegerValue == (int)BuiltInCategory.OST_CurtainWallMullions)
                    {
                        var hostId = (temp as Mullion).Host.Id;
                        if (!ids.Contains(hostId))
                            ids.Add(hostId);
                    }
                    if (temp.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Doors)
                        || temp.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Windows))
                    {
                        if (!ids.Contains(temp.Id))
                            ids.Add(temp.Id);
                    }
                }
                foreach (ElementId id in ids)
                {
                    XYZ pos;
                    ReferenceArray Aref = new();
                    foreach (Reference item in refs)
                    {
                        var temp = Document.GetElement(item);
                        if (temp.Category.Id.IntegerValue == (int)BuiltInCategory.OST_CurtainWallMullions)
                        {
                            var hostId = (temp as Mullion).Host.Id;
                            if (hostId.IntegerValue == id.IntegerValue)
                                Aref.Append(item);
                        }
                        if (temp.Id.IntegerValue == id.IntegerValue)
                            Aref.Append(item);
                    }
                    using (Transaction tran = new Transaction(Document, "Dim W"))
                    {
                        tran.Start();
                        pos = Document.Create.NewDimension(Document.ActiveView, dim.Curve as Line, Aref).TextPosition;
                        tran.RollBack();
                    }
                    using (Transaction tran = new Transaction(Document, "Dim W"))
                    {
                        tran.Start();
                        foreach (DimensionSegment item in dim.Segments)
                        {
                            if (item.TextPosition.IsAlmostEqualTo(pos, 0.00000001))
                            {
                                item.Prefix = "W";
                            }
                        }
                        tran.Commit();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
