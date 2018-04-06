using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Todos2
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
            this.ViewModel = new ViewModels.TodoItemViewModel();
        }

        ViewModels.TodoItemViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
        }

        private void TodoItemClick(object sender, ItemClickEventArgs e)
        {
            UpdateButton.Content = "Update";
            DeleteButton.Content = "Delete";
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            if(InlineToDoItemViewGrid.Visibility == Visibility.Collapsed)
            {
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
            else
            {
                title_grid.Text = ViewModel.SelectedItem.title;
                description_grid.Text = ViewModel.SelectedItem.description;
                date_grid.Date = ViewModel.SelectedItem.date;
                image_grid.Source = ViewModel.SelectedItem.image;
            }
        }
        private async void SelectPictureBtn_Click(object sender, RoutedEventArgs e)
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
                    image_grid.Source = bitmapImage;
                }
            }
            else
            {
                var i = new MessageDialog("select picture operation cancelled!").ShowAsync();
            }
        }
        private void UpdateBtnClick(object o, RoutedEventArgs e)
        {
            bool SomeThingWrong = false;
            string message = "";
            if (title_grid.Text == "")
            {
                message += "Title is empty\n";
                SomeThingWrong = true;
            }
            if (description_grid.Text == "")
            {
                message += "Details is empty\n";
                SomeThingWrong = true;
            }
            DateTime DataPickTime = date_grid.Date.DateTime;
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
                //检查正确
                //如果选中了
                if (ViewModel.SelectedItem != null)
                {
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem.getid(), title_grid.Text, description_grid.Text, date_grid.Date, (BitmapImage)image_grid.Source);
                    ViewModel.SelectedItem = null;
                    title_grid.Text = "";
                    description_grid.Text = "";
                    date_grid.Date = DateTime.Now;
                    image_grid.Source = new BitmapImage(new Uri("ms-appx:/Assets/background.jpg"));
                    UpdateButton.Content = "Creat";
                    DeleteButton.Content = "Cancel";
                }
                else
                {
                    ViewModel.AddTodoItem(title_grid.Text, description_grid.Text, date_grid.Date, (BitmapImage)image_grid.Source);
                    title_grid.Text = "";
                    description_grid.Text = "";
                    date_grid.Date = DateTime.Now;
                    image_grid.Source = new BitmapImage(new Uri("ms-appx:/Assets/background.jpg"));
                }

            }
        }
        private void DeleteBtnClick(object o, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem();
                title_grid.Text = "";
                description_grid.Text = "";
                date_grid.Date = DateTime.Now;
                image_grid.Source = new BitmapImage(new Uri("ms-appx:/Assets/background.jpg"));
                UpdateButton.Content = "Creat";
                DeleteButton.Content = "Cancel";
            }
            else
            {
                title_grid.Text = "";
                description_grid.Text = "";
                date_grid.Date = DateTime.Now;
                image_grid.Source = new BitmapImage(new Uri("ms-appx:/Assets/background.jpg"));
            }
        }
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = null;
            Frame.Navigate(typeof(NewPage), ViewModel);
        }
        private void uptile_click(object sender, RoutedEventArgs e)
        {
            //build badge(DOM方式加载XML文件）
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(File.ReadAllText("tile.xml"));   //File.ReadAllText读取所有text，否则抛出异常

            //update element(两个text分别被选中item的title和details，上传给磁贴)
            XmlNodeList xmlList = xmlDoc.GetElementsByTagName("text");
            //for (int i = 0; i <xmlList.Length; i++)
            //{
            int i = 0;
            ((XmlElement)xmlList[i]).InnerText = ViewModel.AllItems[ViewModel.AllItems.Count - 1].title;
            i++;
            ((XmlElement)xmlList[i]).InnerText = ViewModel.AllItems[ViewModel.AllItems.Count - 1].description;
            i++;
            ((XmlElement)xmlList[i]).InnerText = ViewModel.AllItems[ViewModel.AllItems.Count - 1].title;
            i++;
            ((XmlElement)xmlList[i]).InnerText = ViewModel.AllItems[ViewModel.AllItems.Count - 1].description;
            i++;
            ((XmlElement)xmlList[i]).InnerText = ViewModel.AllItems[ViewModel.AllItems.Count - 1].title;
            i++;
            ((XmlElement)xmlList[i]).InnerText = ViewModel.AllItems[ViewModel.AllItems.Count - 1].description;
            //}

            //perform update(显示更新的磁贴）
            TileNotification notification = new TileNotification(xmlDoc); //var和TileNotification
            var updator = TileUpdateManager.CreateTileUpdaterForApplication();
            updator.Update(notification);
        }
    }
}
