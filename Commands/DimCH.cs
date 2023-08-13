using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class DimCH : ExternalCommand
    {
        public override void Execute()
        {
            PostCommanAlignedDimension.Start(UiApplication);
            PostCommanAlignedDimension.OnPostableCommandModelLineEnded += PostCommanDetailLine_OnPostableCommandModelLineEnded;
        }
        private void PostCommanDetailLine_OnPostableCommandModelLineEnded(object sender, EventArgs e)
        {
            try
            {
                Dimension dim = Document.GetElement(PostCommanAlignedDimension.AddedElement.FirstOrDefault()) as Dimension;
                using (Transaction tran = new Transaction(Document, "Dim CH"))
                {
                    tran.Start();
                    if (dim.Segments.Size == 0)
                    {
                        dim.Prefix = "CH";
                        tran.Commit();
                    }
                    else
                    {
                        dim.Segments.get_Item(dim.Segments.Size - 1).Prefix = "CH";
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
