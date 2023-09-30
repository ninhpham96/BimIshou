using BimIshou.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BimIshou.Views
{
    public partial class BimIshouView
    {
        public BimIshouView()
        {
            InitializeComponent();
            Rectangle rectangle = new Rectangle() { Fill = Brushes.Red, Height = 50, Width = 50 };
            Rectangle rectangle1 = new Rectangle() { Fill = Brushes.Red, Height = 50, Width = 50 };
            Rectangle rectangle2 = new Rectangle() { Fill = Brushes.Red, Height = 50, Width = 50 };

            Canvas.SetTop(rectangle, 100);
            Canvas.SetTop(rectangle1, 200);


            rectangle.Tag = "1";
            rectangle1.Tag = "2";

            rectangle.PreviewMouseMove += Rectangle_PreviewMouseMove;
            rectangle1.PreviewMouseMove += Rectangle_PreviewMouseMove;
            rectangle2.PreviewMouseMove += Rectangle_PreviewMouseMove;

            canvas.Children.Add(rectangle);
            canvas.Children.Add(rectangle1);
            canvas.Children.Add(rectangle2);
        }

        private void Rectangle_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ob = sender as Rectangle;
            if(ob.Tag is string)
            {
                MessageBox.Show("!");
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }
    }
}