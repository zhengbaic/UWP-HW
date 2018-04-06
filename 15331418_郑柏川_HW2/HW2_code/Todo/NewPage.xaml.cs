using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
using Windows.UI.Popups;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos
{
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }


            // TODO

            var i = new MessageDialog("Welcome!").ShowAsync();
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            bool SomeThingWrong = false;
            string message = "";
            if (Title.Text == "")
            {
                message += "Title is empty\n";
                SomeThingWrong = true;
            }
            if(Details.Text == "")
            {
                message += "Details is empty\n";
                SomeThingWrong = true;
            }
            DateTime DataPickTime = DataPick.Date.DateTime;
            if(DateTime.Compare(DataPickTime, DateTime.Now.Date) < 0)
            {
                message += "Date is wrong\n";
                SomeThingWrong = true;
            }
            if (SomeThingWrong)
            {
                var WrongMessage = new MessageDialog(message).ShowAsync();
                Title.Text = "";
                Details.Text = "";
                DataPick.Date = DateTime.Now.Date;
            }else
            {
                var Message = new MessageDialog("Creat you todo list suceessful!").ShowAsync();
                Title.Text = "";
                Details.Text = "";
                DataPick.Date = DateTime.Now.Date;
            }
        }
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            var Message = new MessageDialog("you have canceled the creation!").ShowAsync();
            Title.Text = "";
            Details.Text = "";
            DataPick.Date = DateTime.Now.Date;
        }
        //select pic btn
        private async void SelectPictureButtonClick(object s, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if(file != null)
            {
                using (IRandomAccessStream filestream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.DecodePixelWidth = 600;
                    await bitmapimage.SetSourceAsync(filestream);
                    img.Source = bitmapimage;
                }
            }else
            {
                var WrongMessage = new MessageDialog("cancell select picture").ShowAsync();
            }
        }


    }
}
