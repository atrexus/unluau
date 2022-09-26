using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Unluau.App.Controls
{
    /// <summary>
    /// Interaction logic for MenuBarOption.xaml
    /// </summary>
    public partial class MenuBarOption : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("IconName2", typeof(string), typeof(TitleBarButton));

        private AnimationLibrary animationLibrary;

        public string IconName2 {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public MenuBarOption()
        {
            InitializeComponent();
            DataContext = this;
            animationLibrary = new AnimationLibrary();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Color fromColor = (Color)Application.Current.Resources["LightCharchoal-Color"];
            Color toColor = (Color)Application.Current.Resources["SpenishGray-Color"];

            //animationLibrary.SolidColorBrushAnimation(Main.Background, fromColor, toColor);

            Icon.Foreground = new SolidColorBrush(toColor);
            //await Task.Delay(1000);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Color fromColor = (Color)Application.Current.Resources["SpenishGray-Color"];
            Color toColor = (Color)Application.Current.Resources["LightCharchoal-Color"];

            //animationLibrary.SolidColorBrushAnimation(Main.Background, fromColor, toColor);

            Icon.Foreground = new SolidColorBrush(toColor);
            //await Task.Delay(1000);
        }
    }
}
