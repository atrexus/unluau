using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Unluau.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void PART_CloseTab_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            TabItem tab = ((border.Parent as StackPanel).Parent as Border).TemplatedParent as TabItem;
            TabControl tabControl = tab.Parent as TabControl;

            tabControl.Items.Remove(tab);
        }
    }
}
