using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace BimIshou.AddFilter;

public partial class AddFilterVM : ObservableObject
{
    public MainView MainView { get; }
    public SubView SubView { get; }
    public Document Doc { get; }
    [ObservableProperty]
    private List<View> viewtemplates;
    [ObservableProperty]
    private List<Element> viewFilters;
    public Element viewSelected { get; set; }
    public AddFilterVM(MainView mainView, SubView subView, Document doc)
    {
        Doc = doc;
        Viewtemplates = GetViewTemplates();
        MainView = mainView;
        SubView = subView;
        subView.cbViewSource.SelectedItem = Viewtemplates.FirstOrDefault();
        SubView.DataContext = this;
        MainView.DataContext = this;
    }
    public void ShowMainview()
    {
        MainView.ShowDialog();
    }
    public void ShowSubview()
    {
        SubView.ShowDialog();
    }
    private List<View> GetViewTemplates()
    {
        return new FilteredElementCollector(Doc)
            .OfClass(typeof(View))
            .Cast<View>()
            .Where(v => v.IsTemplate)
            .ToList();
    }
    private List<Element> GetFilter(View view)
    {
        List<Element> result = new();
        var filters = view.GetFilters();
        foreach (var id in filters)
        {
            result.Add(id.ToElement(Doc));
        }
        return result;
    }

    [RelayCommand]
    private void OkSub()
    {
        if(viewSelected is null)
        {
            MessageBox.Show("bạn chưa chọn view nào!");
            return;
        }
        ViewFilters = GetFilter(viewSelected as View);
        SubView.DialogResult = true;
    }
}
