using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Animal
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private delegate string AnimalSaying(object sender, myEventArgs e);//声明一个委托
        private event AnimalSaying Say;//委托声明一个事件
        private int times = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        interface Animal
        {
            //方法
            string saying(object sender, myEventArgs e);
            //属性
            int A { get; set; }
        }

        class cat : Animal
        {
            TextBlock word;
            private int a;

            public cat(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                this.word.Text += "cat : i am a cat\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class dog : Animal
        {
            TextBlock word;
            private int a;

            public dog(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                this.word.Text += "dog: i am a dog\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class pig : Animal
        {
            TextBlock word;
            private int a;
            public pig(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                this.word.Text += "pig: i am a pig\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        private cat c;
        private dog d;
        private pig p;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "";
            Random ran = new Random();
            int RandKey = ran.Next(0, 3);
            
            if(RandKey == 0)
            {
                c = new cat(words);

                Say += new AnimalSaying(c.saying);
            }
            else if(RandKey == 1)
            {
                d = new dog(words);

                Say += new AnimalSaying(d.saying);
            }
            else
            {
                p = new pig(words);

                Say += new AnimalSaying(p.saying);
            }
            Say(this, new myEventArgs(times++));
            if (RandKey == 0) Say -= new AnimalSaying(c.saying);
            else if (RandKey == 1) Say -= new AnimalSaying(d.saying);
            else Say -= new AnimalSaying(p.saying);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(textBox.Text == "cat")
            {
                c = new cat(words);
                Say += new AnimalSaying(c.saying);
                Say(this, new myEventArgs(times++));
                Say -= new AnimalSaying(c.saying);
                textBox.Text = "";
            }
            else if(textBox.Text == "dog")
            {
                d = new dog(words);
                Say += new AnimalSaying(d.saying);
                Say(this, new myEventArgs(times++));
                Say -= new AnimalSaying(d.saying);
                textBox.Text = "";
            }
            else if(textBox.Text == "pig")
            {
                p = new pig(words);
                Say += new AnimalSaying(p.saying);
                Say(this, new myEventArgs(times++));
                Say -= new AnimalSaying(p.saying);
                textBox.Text = "";
            }
            else
            {
                textBox.Text = "";
            }
        }

        //自定义一个Eventargs传递事件参数
        class myEventArgs : EventArgs
        {
            public int t = 0;
            public myEventArgs(int tt)
            {
                this.t = tt;
            }
        }
    }
}
