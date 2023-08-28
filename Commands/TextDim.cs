
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class TextDim : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            
            Document doc = uidoc.Document;

            LocDim locDim = new LocDim();
            string prefix = "有効";

            while (true)
            {
                try
                {

                    Reference r = uidoc.Selection.PickObject(ObjectType.Element, locDim, "Chon doi tuong.");
                    Element element = doc.GetElement(r);
                    XYZ p0 = r.GlobalPoint;
                    Dimension dimension = doc.GetElement(r) as Dimension;
                    DimensionSegmentArray segments = dimension.Segments;
                    List<double> toaDo = new List<double>();
                    if (segments.Size > 0)
                    {
                        foreach (DimensionSegment segment in segments)
                        {
                            toaDo.Add(p0.DistanceTo(segment.Origin));
                        }
                        double min = toaDo[0];
                        int vtmin = 0;

                        for (int i = 0; i < toaDo.Count; i++)
                        {
                            if (toaDo[i] < min)
                            {
                                min = toaDo[i];
                                vtmin = i;
                            }
                        }

                        DimensionSegment segment1 = segments.get_Item(vtmin);
                        using (Transaction trans = new Transaction(doc, "Text Dim"))
                        {
                            trans.Start();
                            segment1.Prefix = prefix;
                            trans.Commit();
                        }
                    }
                    else
                    {
                        using (Transaction trans = new Transaction(doc, "Text Dim"))
                        {
                            trans.Start();
                            dimension.Prefix = prefix;
                            trans.Commit();
                        }
                    }

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Succeeded;
                }
            }


        }
    }
    class LocDim : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Name.Equals("Dimensions");
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

