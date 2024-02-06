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
        viewSelected = Viewtemplates.FirstOrDefault(v => v.Name == "00.VIEWTEMPLATE(ORIGIN)") ?? Viewtemplates.FirstOrDefault();
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
            .Where(v => v.ViewType == ViewType.ThreeD
            || v.ViewType == ViewType.FloorPlan
            || v.ViewType == ViewType.Walkthrough
            || v.ViewType == ViewType.AreaPlan
            || v.ViewType == ViewType.CeilingPlan
            || v.ViewType == ViewType.Section)
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
        if (viewSelected is null)
        {
            MessageBox.Show("bạn chưa chọn view nào!");
            return;
        }
        ViewFilters = GetFilter(viewSelected as View);
        SubView.DialogResult = true;
    }
    [RelayCommand]
    private void CancelSub()
    {
        SubView.DialogResult = false;
    }
    [RelayCommand]
    private void OkMain()
    {
        var views = MainView.lsvViewtemplate.SelectedItems;
        var viewFilter = MainView.lsvFilter.SelectedItems;
        using (Transaction tran = new Transaction(Doc, "Add"))
        {
            tran.Start();
            foreach (View view in views)
            {
                var _filter = view.GetFilters();
                foreach (Element filter in viewFilter)
                {
                    if (!_filter.Any(f => f.IntegerValue.Equals(filter.Id.IntegerValue)))
                    {
                        var overrides = (viewSelected as View).GetFilterOverrides(filter.Id);
                        view.AddFilter(filter.Id);
                        view.SetFilterOverrides(filter.Id, overrides);
                    }
                }
            }
            tran.Commit();
        }
        MainView.DialogResult = true;
    }
    [RelayCommand]
    private void CancelMain()
    {
        MainView.DialogResult = false;
    }
    [RelayCommand]
    private void SelectAll(object obj)
    {
        if (obj.ToString().Equals("ViewFilter"))
            MainView.lsvFilter.SelectAll();
        else
            MainView.lsvViewtemplate.SelectAll();
    }
    [RelayCommand]
    private void UnSelectAll(object obj)
    {
        if (obj.ToString().Equals("ViewFilter"))
            MainView.lsvFilter.UnselectAll();
        else
            MainView.lsvViewtemplate.UnselectAll();
    }
}
