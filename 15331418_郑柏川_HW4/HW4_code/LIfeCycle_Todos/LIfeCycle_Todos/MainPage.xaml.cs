using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace LIfeCycle_Todos
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewPage), "");
        }

        private void CheckBox1Click(object sender, RoutedEventArgs e)
        {
            line1.Visibility = (bool)checkbox1.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CheckBox2Click(object sender, RoutedEventArgs e)
        {
            line2.Visibility = (bool)checkbox2.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // not go back
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            // data recovering
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MainPageInfo"))
            {
                var composite = ApplicationData.Current.LocalSettings.Values["MainPageInfo"] as ApplicationDataCompositeValue;

                bool c1, c2;
                c1 = (bool)composite["checkbox1"];
                c2 = (bool)composite["checkbox2"];
                checkbox1.IsChecked = c1;
                checkbox2.IsChecked = c2;
                line1.Visibility = c1 ? Visibility.Visible : Visibility.Collapsed;
                line2.Visibility = c2 ? Visibility.Visible : Visibility.Collapsed;
                // finish data recovering
                ApplicationData.Current.LocalSettings.Values.Remove("MainPageInfo");
            }
        }

        //store data
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
            composite["checkbox1"] = (bool)checkbox1.IsChecked;
            composite["checkbox2"] = (bool)checkbox2.IsChecked;
            ApplicationData.Current.LocalSettings.Values["MainPageInfo"] = composite;
        }
    }
}
