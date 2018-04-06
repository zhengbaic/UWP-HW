using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Todos2.Models;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
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
            ViewModel.init();
        }

        ViewModels.TodoItemViewModel ViewModel { get; set; }
        DataTransferManager manager;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            manager = DataTransferManager.GetForCurrentView();
            manager.DataRequested += manager_DataRequested;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            manager.DataRequested -= manager_DataRequested;
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
                image_grid.Source =new BitmapImage(ViewModel.SelectedItem.ImageUri);
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
        private async void UpdateBtnClick(object o, RoutedEventArgs e)
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
                    Uri uri = (image_grid.Source as BitmapImage).UriSource;
                    await ViewModel.UpdateTodoItem(ViewModel.SelectedItem.getid(), title_grid.Text, description_grid.Text, date_grid.Date, uri, ViewModel.SelectedItem.completed);
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
                    Uri uri = (image_grid.Source as BitmapImage).UriSource;
                    await ViewModel.AddTodoItem(title_grid.Text, description_grid.Text, date_grid.Date, uri);
                    title_grid.Text = "";
                    description_grid.Text = "";
                    date_grid.Date = DateTime.Now;
                    image_grid.Source = new BitmapImage(new Uri("ms-appx:/Assets/background.jpg"));
                }

            }
        }
        private async void DeleteBtnClick(object o, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);
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
        private void TileAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            var updatemanager = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication();
            updatemanager.Clear();
            updatemanager.EnableNotificationQueue(true);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(File.ReadAllText("tile.xml"));

            XmlNodeList texts = xmlDoc.GetElementsByTagName("text");
            foreach (TodoItem item in ViewModel.AllItems)
            {
                int count = 0;

                // small
                ((XmlElement)texts[count]).InnerText = item.title;
                count++;

                // medium
                ((XmlElement)texts[count]).InnerText = item.title;
                count++;
                ((XmlElement)texts[count]).InnerText = item.description;
                count++;

                // wide
                ((XmlElement)texts[count]).InnerText = item.title;
                count++;
                ((XmlElement)texts[count]).InnerText = item.description;
                count++;
                ((XmlElement)texts[count]).InnerText = item.date.ToString();
                count++;

                TileNotification notification = new TileNotification(xmlDoc);
                updatemanager.Update(notification);
            }
        }
        private void ShareButtonClick(object sender, RoutedEventArgs e)
        {
            //找到要分享的item
            DataTransferManager.ShowShareUI();
            ViewModel.SelectedItem = ((MenuFlyoutItem)e.OriginalSource).DataContext as TodoItem;
        }
        async void manager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            //所要传输的数据
            DataPackage data = args.Request.Data;
            if (ViewModel.SelectedItem != null)
            {
                //为数据添加值
                string textTitle = ViewModel.SelectedItem.title;
                string textDescription = ViewModel.SelectedItem.description;

                data.Properties.Title = textTitle;
                data.Properties.Description = textDescription;
               
            }
            else
            {
                //在侧边拿数据
                data.Properties.Title = title_grid.Text;
                data.Properties.Description = description_grid.Text;
            }

            // 图片
            DataRequestDeferral GetFiles = args.Request.GetDeferral();
            try
            {
                StorageFile imageFile = await Package.Current.InstalledLocation.GetFileAsync("Assets\\background.jpg");
                data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(imageFile);
                data.SetBitmap(RandomAccessStreamReference.CreateFromFile(imageFile));
            }
            finally
            {
                GetFiles.Complete();
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchContent.Text;
            if (query == "")
            {
                var message = new MessageDialog("请输入查询内容", "Search Error").ShowAsync();
                return;
            }

            string result = ViewModel.FindTodiItem(query);
            if (result == "") result = "未找到";
            var message2 = new MessageDialog(result, "Search Result").ShowAsync();
        }
    }
}
