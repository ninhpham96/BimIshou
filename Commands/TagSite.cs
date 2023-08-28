
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
using System.Security.Policy;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Nice3point.Revit.Toolkit.External;

#endregion

namespace BimIshou.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class TagSite : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            
            Document doc = uidoc.Document;

            TagMode tMode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tOrien = TagOrientation.Horizontal;

            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance));

            foreach (Element element in collector)
            {
                RevitLinkInstance instance = element as RevitLinkInstance;
                Document linkDoc = instance.GetLinkDocument();
                //RevitLinkType type = doc.GetElement(instance.GetTypeId()) as RevitLinkType;
                FilteredElementCollector sites = new FilteredElementCollector(linkDoc)
                    .OfCategory(BuiltInCategory.OST_Site)
                    .WhereElementIsNotElementType();
                IList<Element> list = new List<Element>();

                foreach (Element site in sites)
                {

                    ElementType elementType = instance.GetLinkDocument().GetElement(site.GetTypeId()) as ElementType;

                    if (elementType.FamilyName == "d外構_レベル標_隅" || elementType.FamilyName == "d外構_レベル標_中心")
                    {
                        list.Add(site);
                    }

                }
                try
                {
                    using (Transaction trans = new Transaction(doc, "Creat Tag"))
                    {
                        trans.Start();
                        foreach (Element ele in list)
                        {
                            Reference reference = new Reference(ele).CreateLinkReference(instance);
                            LocationPoint loc = ele.Location as LocationPoint;
                            XYZ pos = loc.Point;
                            IndependentTag tag = IndependentTag.Create(doc, doc.ActiveView.Id, reference, true, tMode, tOrien, pos);


                            LinkElementId linkId = tag.TaggedElementId;
                            ElementId linkInsancetId = linkId.LinkInstanceId;
                            ElementId linkedElementId = linkId.LinkedElementId;
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Failed;
                }

            }
            return Result.Succeeded;
        }
    }
}
