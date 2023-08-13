using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class DimTT : ExternalCommand
    {
        public override void Execute()
        {
            PostCommanAlignedDimension.Start(UiApplication);
            PostCommanAlignedDimension.OnPostableCommandModelLineEnded += PostCommanDetailLine_OnPostableCommandModelLineEnded;
        }
        private void PostCommanDetailLine_OnPostableCommandModelLineEnded(object sender, EventArgs e)
        {
            Dimension dim = Document.GetElement(PostCommanAlignedDimension.AddedElement.FirstOrDefault()) as Dimension;
            try
            {
                using (Transaction tran = new Transaction(Document, "Dim 有効"))
                {
                    tran.Start();
                    var maxValue = double.MinValue;
                    foreach (DimensionSegment item in dim.Segments)
                    {
                        if ((double)item.Value > maxValue)
                            maxValue = (double)item.Value;
                    }
                    foreach (DimensionSegment item in dim.Segments)
                    {
                        if (item.Value.Equals(maxValue))
                            item.Prefix = "有効";
                    }
                    tran.Commit();
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
