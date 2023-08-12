using BimIshou.ViewModels;

namespace BimIshou.Views
{
    public partial class BimIshouView
    {
        public BimIshouView(BimIshouViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}