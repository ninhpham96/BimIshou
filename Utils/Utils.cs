using Autodesk.Revit.DB;
using System.Windows;

namespace BimIshou.Utils
{
    public static class Utils
    {
        public static View3D Get3DView(Document doc)
        {
            var collector
                = new FilteredElementCollector(doc);
            collector.OfClass(typeof(View3D));
            foreach (View3D v in collector)
                if (v is { IsTemplate: false, Name: "{3D}" })
                    return v;
            return null;
        }
        public static Reference GetCeilingReferenceAbove(View3D view, XYZ p, XYZ dir)
        {
            var filter = new ElementClassFilter(
                typeof(Wall));
            var refIntersector
                = new ReferenceIntersector(filter,
                    FindReferenceTarget.Face, view);
            refIntersector.FindReferencesInRevitLinks = false;

            var rwc = refIntersector.FindNearest(
                p, dir);
            var r = null == rwc
                ? null
                : rwc.GetReference();
            if (null == r) MessageBox.Show("no intersecting geometry");
            return r;
        }
    }
    public class SelectionFilter : Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        private BuiltInCategory _builtInCategory;

        private List<BuiltInCategory> _builtInCategoryList;

        private bool _allowReference;


        private bool multiCategories = false;

        public SelectionFilter(BuiltInCategory builtInCategory, bool allowReference = true)
        {
            this._builtInCategory = builtInCategory;
            this._allowReference = allowReference;
            this.multiCategories = false;
        }

        public SelectionFilter(List<BuiltInCategory> builtInCategoryList, bool allowReference = true)
        {
            this._builtInCategoryList = builtInCategoryList;
            this._allowReference = allowReference;
            this.multiCategories = true;
        }
        public bool AllowElement(Element elem)
        {
            if (this.multiCategories)
            {
                foreach (BuiltInCategory builtInCategory in this._builtInCategoryList)
                {
                    if (MatchCategory(elem, builtInCategory)) return true;
                }
            }
            else
            {
                if (MatchCategory(elem, this._builtInCategory)) return true;
            }

            return false;

        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return _allowReference;
        }
        public bool MatchCategory(Element element, BuiltInCategory builtInCategory)
        {
            if (element != null)
            {
                try
                {
                    if (element.Category.Id.IntegerValue == (int)builtInCategory)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    if (element.Id.IntegerValue == (int)builtInCategory)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}