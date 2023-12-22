using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<test> listCategory = new List<test>();

        public Viewmodel(Document doc)
        {
            this.doc = doc;
            //ListCategory = new FilteredElementCollector(doc)
            //    .OfClass(typeof(Family))
            //    .OfType<Family>()
            //    .Where(fa => fa.FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
            //    .ToList();
            ObservableCollection<B> children = new ObservableCollection<B>();
            children.Add(new B() { Name = "!" });
            children.Add(new B() { Name = "!" });
            children.Add(new B() { Name = "!" });
            children.Add(new B() { Name = "!" });
            ListCategory.Add(new test() { Header = "Ninh", Children = children });

        }
    }
    public class test()
    {
        public string Header { get; set; }
        public ObservableCollection<B> Children { get; set; }
    }
    public class B()
    {
        public string Name { get; set; }
    }
}
