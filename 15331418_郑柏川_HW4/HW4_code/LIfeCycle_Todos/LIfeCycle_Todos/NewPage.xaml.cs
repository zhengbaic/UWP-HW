using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace LIfeCycle_Todos
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {

        private StorageFile ImageData;

        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            // 恢复数据
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NewPageInfo"))
            {
                var composite = ApplicationData.Current.LocalSettings.Values["NewPageInfo"] as ApplicationDataCompositeValue;

                title.Text = (string)composite["Title"];
                details.Text = (string)composite["Detail"];
                datepicker.Date = (DateTimeOffset)composite["DueDate"];

                // 获取token
                string token = (string)composite["ImageData"];
                if (token != "no")
                {
                    
                    ImageData = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Remove(token);

                    var fileStream = await ImageData.OpenReadAsync();
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);

                    img.Source = bitmapImage;
                }

                ApplicationData.Current.LocalSettings.Values.Remove("NewPageInfo");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();

            composite["Title"] = title.Text;
            composite["Detail"] = details.Text;
            composite["DueDate"] = datepicker.Date;

            if (ImageData != null)
            {
                // 讲被选择的图片文件保存到FutureAccessList，并且保存token到composite中
                string token = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(ImageData);
                composite["imagedata"] = token;
            }
            else
            {
                composite["ImageData"] = "no";
            }

            ApplicationData.Current.LocalSettings.Values["NewPageInfo"] = composite;
        }

        private void Create_Item(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), "");
        }

        private async void SelectPicture(object o, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 600;
                    await bitmapImage.SetSourceAsync(fileStream);
                    img.Source = bitmapImage;
                }
            }
            else
            {
                var i = new MessageDialog("select picture operation cancelled!").ShowAsync();
            }
        }


    }
}
