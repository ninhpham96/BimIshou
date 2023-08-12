using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using System.Windows;

namespace BimIshou.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class DimFuniture : ExternalCommand
    {
        public override void Execute()
        {
            ReferenceArray referenceArray = new ReferenceArray();
            var filterplumbingFixtures = new SelectionFilter(BuiltInCategory.OST_PlumbingFixtures, true);
            var point = UiDocument.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None);
            var selectedplumbingFixtures = UiDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, filterplumbingFixtures,"Chọn các thiết bị muốn Dim");

            try
            {
                foreach (Reference item in selectedplumbingFixtures)
                {
                    var ele = Document.GetElement(item) as FamilyInstance;
                    var r = ele.GetReferenceByName("中心(左/右)");
                    if (r != null) referenceArray.Append(r);
                }
                var view3d = Get3DView(Document);
                var temp = Document.GetElement(selectedplumbingFixtures.First()) as FamilyInstance;

                var loca = (temp.Location as LocationPoint).Point;
                var linee = Line.CreateBound(point, point.Add(temp.HandOrientation * 100));

                referenceArray.Append(GetCeilingReferenceAbove(view3d, loca.Add(-temp.FacingOrientation * 100 / 304.8), temp.HandOrientation));
                referenceArray.Append(GetCeilingReferenceAbove(view3d, loca.Add(-temp.FacingOrientation * 100 / 304.8), -temp.HandOrientation));

                using (Transaction tran = new Transaction(Document, "new tran"))
                {
                    tran.Start();
                    Document.Create.NewDimension(Document.ActiveView, linee, referenceArray);
                    tran.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private View3D Get3DView(Document doc)
        {
            var collector
                = new FilteredElementCollector(Document);
            collector.OfClass(typeof(View3D));
            foreach (View3D v in collector)
                if (v is { IsTemplate: false, Name: "{3D}" })
                    return v;
            return null;
        }
        private Reference GetCeilingReferenceAbove(View3D view, XYZ p, XYZ dir)
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
