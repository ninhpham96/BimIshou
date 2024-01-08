using Autodesk.Revit.DB;
using System.Diagnostics;
using System.Windows;

namespace BimIshou
{
    /// <summary>
    /// Interaction logic for View.xaml
    /// </summary>
    public partial class _View : Window
    {
        public _View()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var viewmodel = (Export2Excel)DataContext;
            viewmodel.SelectedItems = listview.SelectedItems
                .Cast<ViewSchedule>()
                .ToList();
        }
    }
}
