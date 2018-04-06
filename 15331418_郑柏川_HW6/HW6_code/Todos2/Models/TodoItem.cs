using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos2.Models
{
    class TodoItem
    {

        public long id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public bool completed { get; set; }

        public DateTimeOffset date { get; set; }

        public Uri ImageUri { get; set; }

        public TodoItem()
        {
            this.id = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            this.ImageUri = new Uri("ms-appdata:///local/Images/" + this.id + ".png");
        }
        //日期字段自己写

        public TodoItem(string title, string description, DateTimeOffset date)
        {
            this.id = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(); //生成id
            this.title = title;
            this.description = description;
            this.completed = false; //默认为未完成
            this.date = date;
            this.ImageUri = new Uri("ms-appdata:///local/Images/" + this.id + ".png");
        }

        public long getid()
        {
            return id;
        }
    }
}
