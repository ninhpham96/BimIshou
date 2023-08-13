using System.Windows;

namespace BimIshou.Commands.DimDoubuchi.View
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DimDoubuchi.setting = int.Parse(settingDim.Text);
            this.Close();
        }
    }
}
