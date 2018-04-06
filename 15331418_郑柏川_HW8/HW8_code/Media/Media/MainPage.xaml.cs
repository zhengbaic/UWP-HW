using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Media
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void open(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = myMedia.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void end(object sender, RoutedEventArgs e)
        {
            myMedia.Stop();
        }

        private void ChangeMeidaVolume(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (myMedia != null)
            {
                myMedia.Volume = (double)volumnSlider.Value;
            }
        }
        //播放
        private void PlayC(object sender, RoutedEventArgs e)
        {
            myMedia.Play();
        }
        //暂停
        private void PauseC(object sender, RoutedEventArgs e)
        {
            myMedia.Pause();
        }
        //停止
        private void StopC(object sender, RoutedEventArgs e)
        {
            myMedia.Stop();
        }
        //调整音量
        private void AdjustVolume(object sender, RoutedEventArgs e)
        {
            if (volumnSlider.Visibility == Visibility.Collapsed)
            {
                volumnSlider.Visibility = Visibility.Visible;
            }
            else
            {
                volumnSlider.Visibility = Visibility.Collapsed;
            }
        }


        //全屏
        private void FullScreen(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isInFullScreenMode = view.IsFullScreenMode;
            if (isInFullScreenMode)
            {
                myMedia.IsFullWindow = !myMedia.IsFullWindow;
                view.ExitFullScreenMode();
            }
            else
            {
                myMedia.IsFullWindow = !myMedia.IsFullWindow;
                view.TryEnterFullScreenMode();
            }
        }
        //选择文件
        private async void ChooseFile(object sender, RoutedEventArgs e)
        {
            await OpenFile();//等待
        }
        //打开文件
        async private Task OpenFile()
        {
            var open = new FileOpenPicker();

            open.FileTypeFilter.Add(".wma");
            open.FileTypeFilter.Add(".wmv");
            open.FileTypeFilter.Add(".mp3");
            open.FileTypeFilter.Add(".mp4");

            var file = await open.PickSingleFileAsync();

            if (file != null)
            {
                string type = file.FileType;
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                if (type == ".wma" || type == ".mp3")
                {
                    myMedia.Visibility = Visibility.Collapsed;
                    cover.Visibility = Visibility.Visible;
                    myMedia.SetSource(stream, file.ContentType);
                    myMedia.Play();
                }
                else if (type == ".mp4" || type == ".wmv")
                {
                    myMedia.Visibility = Visibility.Visible;
                    cover.Visibility = Visibility.Collapsed;
                    myMedia.SetSource(stream, file.ContentType);
                    myMedia.Play();
                }
                else
                {
                    await new MessageDialog("不支持该文件格式").ShowAsync();
                }
            }
        }

    }
}
