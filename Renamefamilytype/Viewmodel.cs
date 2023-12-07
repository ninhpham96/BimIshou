using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BimIshou.Renamefamilytype
{
    public partial class Viewmodel : ObservableObject
    {
        private Document doc;

        [ObservableProperty]
        private List<Family> listCategory = new List<Family>();

        public Viewmodel(Document doc)
        {
            this.doc = doc;
            ListCategory = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .OfType<Family>()
                .Where(fa => fa.FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                .ToList();
        }
    }
}
