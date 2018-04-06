using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos2.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos2.ViewModels
{
    class TodoItemViewModel
    {
        private NewObservableCollection<Models.TodoItem> allItems = new NewObservableCollection<Models.TodoItem>();
        public NewObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; }  }

        public TodoItemViewModel()
        {
            // 加入两个用来测试的item
            BitmapImage image = new BitmapImage(new Uri("ms-appx:/Assets/background.jpg"));

            this.allItems.Add(new Models.TodoItem("123", "123", new DateTime(2222, 2, 2, 2, 2, 2), image));
            this.allItems.Add(new Models.TodoItem("456", "456", new DateTime(2223, 2, 2, 2, 2, 2), image));
        }

        public void AddTodoItem(string title, string description, DateTimeOffset date, BitmapImage pic)
        {
            this.allItems.Add(new Models.TodoItem(title, description, date, pic));
        }

        public void RemoveTodoItem()
        {
            //不是很确定
            // DIY
            this.allItems.Remove(this.selectedItem);
            // set selectedItem to null after remove
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string description, DateTimeOffset date, BitmapImage pic)
        {
            // DIY
            for(int ItemCount = 0; ItemCount < this.allItems.Count; ItemCount++)
            {
                var CurrentItem = this.allItems.ElementAt(ItemCount);
                if(CurrentItem.getid() == id)
                {
                    CurrentItem.title = title;
                    CurrentItem.description = description;
                    CurrentItem.date = date;
                    CurrentItem.image = pic;
                    //????
                    allItems.SetItem(ItemCount, CurrentItem);
                }
            }
            // set selectedItem to null after update
            this.selectedItem = null;
        }

    }
}
