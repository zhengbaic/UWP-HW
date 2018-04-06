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
using Todos2.Models;

namespace Todos2
{
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        }
        public event EventHandler<BackRequestedEventArgs> OnBackRequested;
        private ViewModels.TodoItemViewModel ViewModel;
        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            ViewModel.SelectedItem = null;
            Frame.Navigate(typeof(MainPage), ViewModel);
            /*if (OnBackRequested != null) { ViewModel.SelectedItem = null; OnBackRequested(this, e); }
            if (!e.Handled)
            {
                Frame frame = Window.Current.Content as Frame;
                if (frame.CanGoBack)
                {
                    ViewModel.SelectedItem = null;
                    Frame.Navigate(typeof(MainPage), ViewModel);
                    e.Handled = true;
                }
            }*/
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //返回的条件设置
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);
            if (ViewModel.SelectedItem == null)
            {
                CreateButton.Content = "Create";
                var i = new MessageDialog("Welcome to creat your todolist!").ShowAsync();
            }
            else
            {
                CreateButton.Content = "Update";
                var i = new MessageDialog("Welcome to update your todolist!").ShowAsync();
                title.Text = ViewModel.SelectedItem.title;
                description.Text = ViewModel.SelectedItem.description;
                datepicker.Date = ViewModel.SelectedItem.date;
                img.Source = new BitmapImage(ViewModel.SelectedItem.ImageUri);
            }
        }
        private async void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            bool SomeThingWrong = false;
            string message = "";
            if (title.Text == "")
            {
                message += "Title is empty\n";
                SomeThingWrong = true;
            }
            if (description.Text == "")
            {
                message += "Details is empty\n";
                SomeThingWrong = true;
            }
            DateTime DataPickTime = datepicker.Date.DateTime;
            if (DateTime.Compare(DataPickTime, DateTime.Now.Date) < 0)
            {
                message += "Date is wrong\n";
                SomeThingWrong = true;
            }
            if (SomeThingWrong)
            {
                var WrongMessage = new MessageDialog(message).ShowAsync();
            }
            else
            {
                if ((string)CreateButton.Content == "Create")
                {
                    var uri = (img.Source as BitmapImage).UriSource;
                    await ViewModel.AddTodoItem(title.Text, description.Text, datepicker.Date, uri);
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }
                else
                {
                    await ViewModel.UpdateTodoItem(ViewModel.SelectedItem.getid(), title.Text, description.Text, datepicker.Date, (img.Source as BitmapImage).UriSource, false);
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }

            }
        }
        private async void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            var id = ViewModel.SelectedItem.id;
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(id);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }

        private async void SelectPictureBtnClick(object o, RoutedEventArgs e)
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

                    // 保存到临时文件夹
                    var fileToSave = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("temp.png", CreationCollisionOption.ReplaceExisting);
                    var stream = await file.OpenReadAsync();
                    var bytes = await Temp.GetBytesFromStream(stream);
                    await FileIO.WriteBytesAsync(fileToSave, bytes);
                }
            }
            else
            {
                var i = new MessageDialog("select picture operation cancelled!").ShowAsync();
            }

        }
        private void CancelBtnClick(object o, RoutedEventArgs e)
        {
            title.Text = "";
            description.Text = "";
            datepicker.Date = (DateTime.Now);

            Image image = img as Image;
            BitmapImage bitmapImage = new BitmapImage();
            image.Width = bitmapImage.DecodePixelWidth = 350;
            bitmapImage.UriSource = new Uri(image.BaseUri, "Assets/background.jpg");
            img.Source = bitmapImage;

            ViewModel.SelectedItem = null;
            Frame.Navigate(typeof(MainPage), ViewModel);
        }
    }
}
