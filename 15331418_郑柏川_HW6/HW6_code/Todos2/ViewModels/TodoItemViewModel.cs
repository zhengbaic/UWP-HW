using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos2.Models;
using Windows.Storage;
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

        private SQLiteConnection db;

        public TodoItemViewModel()
        {
            db = new SQLiteConnection("Todos.db");

            string query =
                @"CREATE TABLE IF NOT EXISTS
                    TodoItem (
                        Id          INTEGER PRIMARY KEY,
                        Title       TEXT,
                        Description TEXT,
                        DueDate     CHAR(8),
                        Completed   CHAR(1),
                        ImageUri    CHAR(44)
                    );";
            using (var statement = db.Prepare(query))
            {
                statement.Step();
            }

            // 加入两个用来测试的item
            /*BitmapImage image = new BitmapImage(new Uri("ms-appx:/Assets/background.jpg"));

            this.allItems.Add(new Models.TodoItem("123", "123", new DateTime(2222, 2, 2, 2, 2, 2), image));
            this.allItems.Add(new Models.TodoItem("456", "456", new DateTime(2223, 2, 2, 2, 2, 2), image));*/
        }

        public async void init()
        {
            string query = @"SELECT * FROM TodoItem";
            using (var statement = db.Prepare(query))
            {
                while (statement.Step() != SQLiteResult.DONE)
                {
                    TodoItem item = new TodoItem();
                    item.id = (long)statement[0];
                    item.title = (string)statement[1];
                    item.description = (string)statement[2];
                    string date = (string)statement[3];
                    int year = int.Parse(date.Substring(0, 4));
                    int month = int.Parse(date.Substring(4, 2));
                    int day = int.Parse(date.Substring(6, 2));
                    item.date = new DateTimeOffset(new DateTime(year, month, day));
                    item.completed = (string)statement[4] == "Y";
                    item.ImageUri = new Uri((string)statement[5]);

                    allItems.Add(item);
                }
            }
            if (allItems.Count == 0)
            {
                await AddTodoItem("test1", "test1", new DateTimeOffset(new DateTime(2017, 4, 4)), new Uri("ms-appx:///Assets/background.jpg"));
                await AddTodoItem("test2", "test2", new DateTimeOffset(new DateTime(2017, 4, 4)), new Uri("ms-appx:///Assets/background.jpg"));
            }
        }
        public async         Task
AddTodoItem(string title, string description, DateTimeOffset date, Uri imageUri)
        {
            var item = new TodoItem(title, description, date);

            // 把图片保存到local文件夹
            var imageFile = await StorageFile.GetFileFromApplicationUriAsync(imageUri);
            var imageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
            var fileToSave = await imageFolder.CreateFileAsync(item.id + ".png");
            var stream = await imageFile.OpenReadAsync();
            var bytes = await Temp.GetBytesFromStream(stream);
            await FileIO.WriteBytesAsync(fileToSave, bytes);

            this.allItems.Add(item);

            // 保存到数据库
            using (var statement = db.Prepare("INSERT INTO TodoItem (Id, Title, Description, DueDate, Completed, ImageUri) VALUES (?, ?, ?, ?, ?, ?)"))
            {
                statement.Bind(1, item.id);
                statement.Bind(2, item.title);
                statement.Bind(3, item.description);
                string duedate = item.date.Year.ToString();
                duedate += (item.date.Month < 10) ? "0" + item.date.Month : item.date.Month.ToString();
                duedate += (item.date.Day < 10) ? "0" + item.date.Day : item.date.Day.ToString();
                statement.Bind(4, duedate);
                statement.Bind(5, item.completed ? "Y" : "N");
                statement.Bind(6, item.ImageUri.ToString());
                statement.Step();
            }
        }

        public void RemoveTodoItem(long id)
        {
            //不是很确定
            // DIY
            int i;
            for (i = 0; allItems[i].id != id; ++i) ;
            selectedItem = allItems[i];
            this.allItems.Remove(selectedItem);
            this.selectedItem = null;

            // 更新数据库
            using (var statement = db.Prepare("DELETE FROM TodoItem WHERE Id = ?"))
            {
                statement.Bind(1, id);
                statement.Step();
            }
        }

        public async 
        Task
UpdateTodoItem(long id, string title, string description, DateTimeOffset date, Uri imageUri, bool completed)
        {
            int i;
            for (i = 0; allItems[i].id != id; ++i) ;
            allItems[i].title = title;
            allItems[i].description = description;
            allItems[i].date = date;
            allItems[i].completed = completed;

            long tempid = 0;
            if (imageUri.ToString() == "ms-appdata:///temp/temp.png")
            {
                tempid = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                var imageFile = await StorageFile.GetFileFromApplicationUriAsync(imageUri);
                //创建文件夹
                var imageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                //在创建的文件夹保存图片
                var fileToSave = await imageFolder.CreateFileAsync(tempid + ".png", CreationCollisionOption.ReplaceExisting);
                var stream = await imageFile.OpenReadAsync();
                var bytes = await Temp.GetBytesFromStream(stream);
                await FileIO.WriteBytesAsync(fileToSave, bytes);
                allItems[i].ImageUri = new Uri("ms-appdata:///local/Images/" + tempid + ".png");  
            }


            // 数据库操作
            string sql = "UPDATE TodoItem SET Title = ?, Description = ?, DueDate = ?, Completed = ?, ImageUri = ? WHERE Id = ?";
            using (var statement = db.Prepare(sql))
            {
                statement.Bind(1, allItems[i].title);
                statement.Bind(2, allItems[i].description);
                string duedate = allItems[i].date.Year.ToString();
                duedate += (allItems[i].date.Month < 10) ? "0" + allItems[i].date.Month : allItems[i].date.Month.ToString();
                duedate += (allItems[i].date.Day < 10) ? "0" + allItems[i].date.Day : allItems[i].date.Day.ToString();
                statement.Bind(3, duedate);
                statement.Bind(4, allItems[i].completed ? "Y" : "N");
                statement.Bind(5, allItems[i].ImageUri.ToString());
                statement.Bind(6, allItems[i].id);
                statement.Step();
            }
        }
        // 查
        public string FindTodiItem(string query)
        {
            string result = "";
            if (query == "") return result;
            for (int i = 0; i < AllItems.Count; ++i)
            {
                if (ItemMatch(AllItems[i], query))
                {
                    result +=
                        "[Title] " + AllItems[i].title + "\n" +
                        "[Description] " + AllItems[i].description + "\n" +
                        "[Due Date] " + AllItems[i].date.ToString() + "\n\n";
                }
            }
            return result;
        }

        private bool ItemMatch(TodoItem item, string query)
        {
            if (item.title.Contains(query))
            {
                return true;
            }
            if (item.description.Contains(query))
            {
                return true;
            }
            if (item.date.Year.ToString().Contains(query) ||
                item.date.Month.ToString().Contains(query) ||
                item.date.Day.ToString().Contains(query))
            {
                return true;
            }
            return false;
        }

    }
}
