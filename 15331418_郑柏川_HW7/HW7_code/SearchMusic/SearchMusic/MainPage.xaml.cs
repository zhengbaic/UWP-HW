using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

namespace SearchMusic
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string url = "http://api.map.baidu.com/telematics/v3/weather?location=汕尾&output=xml&ak=11ffd27d38deda622f51c9d314d46b17";
            HttpClient client = new HttpClient();
            string xmlString = await client.GetStringAsync(url);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);

            XmlNodeList weather = xml.GetElementsByTagName("weather");
            XmlNodeList wind = xml.GetElementsByTagName("wind");
            XmlNodeList temperature = xml.GetElementsByTagName("temperature");
            XmlNodeList date = xml.GetElementsByTagName("date");

            string str = "";
            for (int i = 0; i < weather.Count; ++i)
            {
                str += date[i + 1].InnerText + "\n"
                     + wind[i].InnerText + "\n"
                     + temperature[i].InnerText + "\n"
                     + weather[i].InnerText + "\n\n";
            }
            var md = new MessageDialog(str).ShowAsync();
        }
        private async void Button_Click(object o, RoutedEventArgs a)
        {
            if (SearchText.Text.Length == 0)
            {

                Result.Visibility = Visibility.Collapsed;
                var mes = new MessageDialog("you did not input anything").ShowAsync();
                return;
            }
            else
            {
                Result.Visibility = Visibility.Visible;
            }
            string url = "https://api.douban.com/v2/music/search?q=" + SearchText.Text;
            HttpClient client = new HttpClient();
            string jsonObject;
            try
            {
                jsonObject = await client.GetStringAsync(url);
            }
            catch (Exception e)
            {
                jsonObject = "null";
                var mes = new MessageDialog("Nothing found").ShowAsync();
                return;
            }
            //string jsonObject = await client.GetStringAsync(url);
            JObject res = (JObject)JsonConvert.DeserializeObject(jsonObject);
            int num = (int)res["total"];
            if (num == 0)
            {
                Result.Visibility = Visibility.Collapsed;
                var mes = new MessageDialog("Nothing found").ShowAsync();
                return;
            }
            JArray array = (JArray)res["musics"];
            JArray a1 = (JArray)res["musics"][0]["author"];
            singer.Text = "歌手："+(String)a1[0]["name"];
            name.Text = "歌名："+(String)res["musics"][0]["title"];
            img.Source = new BitmapImage(new Uri(res["musics"][0]["image"].ToString()));

        }
    }
}
