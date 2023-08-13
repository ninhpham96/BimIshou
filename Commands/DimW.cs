using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class DimW : ExternalCommand
    {
        public override void Execute()
        {
            PostCommanAlignedDimension.Start(UiApplication);
            PostCommanAlignedDimension.OnPostableCommandModelLineEnded += PostCommanDetailLine_OnPostableCommandModelLineEnded;
        }
        private void PostCommanDetailLine_OnPostableCommandModelLineEnded(object sender, EventArgs e)
        {
            Dimension dim = Document.GetElement(PostCommanAlignedDimension.AddedElement.FirstOrDefault()) as Dimension;
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
                PostCommanAlignedDimension.OnPostableCommandModelLineEnded -= PostCommanDetailLine_OnPostableCommandModelLineEnded;
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
