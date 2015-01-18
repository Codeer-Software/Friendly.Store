using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            _grid.Background = new SolidColorBrush(Colors.Green);
        }

        string Func(int value)
        {
            return value.ToString();
        }
    }
}
