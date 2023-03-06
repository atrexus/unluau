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
    /// Interaction logic for TitleBarButton.xaml
    /// </summary>
    public partial class TitleBarButton : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("IconName", typeof(string), typeof(TitleBarButton));

        private AnimationLibrary animationLibrary;

        public string IconName {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public TitleBarButton()               
        {
            InitializeComponent();
            DataContext = this;
            animationLibrary = new AnimationLibrary();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Color fromColor = (Color)Application.Current.Resources["Charchoal-Color"];
            Color toColor = (Color)Application.Current.Resources["DarkCharchoal-Color"];

            //animationLibrary.SolidColorBrushAnimation(Main.Background, fromColor, toColor);

            Main.Background = new SolidColorBrush(toColor);
            //await Task.Delay(1000);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Color fromColor = (Color)Application.Current.Resources["DarkCharchoal-Color"];
            Color toColor = (Color)Application.Current.Resources["Charchoal-Color"];

            //animationLibrary.SolidColorBrushAnimation(Main.Background, fromColor, toColor);
            
            Main.Background = new SolidColorBrush(toColor);
            //await Task.Delay(1000);
        }
    }
}
