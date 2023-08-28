using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MultiCutCMD : ExternalCommand
    {
        public override void Execute()
        {
            List<Element> fromlistElement = new List<Element>();
            List<Element> tolistElement = new List<Element>();
            MuliCutFilter genericModelFileter = new MuliCutFilter();

            try
            {
                IList<Reference> fromlistReference = UiDocument.Selection.PickObjects(ObjectType.Element, genericModelFileter, "Chọn đối tượng bị cắt!");
                IList<Reference> tolistReference = UiDocument.Selection.PickObjects(ObjectType.Element, genericModelFileter, "Chọn đối tượng cắt!");
                foreach (var item in fromlistReference)
                {
                    fromlistElement.Add(Document.GetElement(item));
                }
                foreach (var item in tolistReference)
                {
                    tolistElement.Add(Document.GetElement(item));
                }
                using (Transaction transaction = new Transaction(Document))
                {
                    transaction.Start("Try cut via API");
                    foreach (var item in fromlistElement)
                    {
                        foreach (var ite in tolistElement)
                        {
                            SolidSolidCutUtils.AddCutBetweenSolids(Document, item, ite);
                        }
                    }
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
    class MuliCutFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Name == "Generic Models" || elem.Category.Name == "Structural Framing"|| elem.Category.Name == "Structural Columns")
                return true;
            else return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
