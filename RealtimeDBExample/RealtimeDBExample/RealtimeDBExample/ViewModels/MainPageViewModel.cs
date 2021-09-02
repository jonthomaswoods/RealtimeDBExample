using Firebase.Database;
using RealtimeDBExample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using RealtimeDBExample.Config;
using System.Threading.Tasks;
using System.Linq;
using Firebase.Database.Query;

namespace RealtimeDBExample.ViewModels
{
    /// <summary>
    /// MainPage example of Firebase Realtime Database operations
    /// </summary>
    public class MainPageViewModel : BaseViewModel
    {
        /// <summary>
        /// Setting up a Firebase Client with App Secrect Auth
        /// </summary>
        public FirebaseClient fc = new FirebaseClient(ConfigFirebase.FirebaseClient,
                                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(ConfigFirebase.FrebaseSecret) }); //The ConfigFirebase class containts the required values 

        public ICommand RefreshListCommand { get; set; }
        public ICommand PostCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand UpdateCommand { get; set; }

        //count for only loading 5 items into listview
        public int listcount = 5;

        /// <summary>
        /// MainPage View Model initializiation
        /// </summary>
        public MainPageViewModel()
        {
            RefreshListCommand = new Command(PerformRefresh);
            PostCommand = new Command(Post);
            DeleteCommand = new Command(Delete);
            UpdateCommand = new Command(Update);

            GetData();
        }

        /// <summary>
        /// Gets a list of ItemsModel
        /// </summary>
        private async void GetData()
        {

                Items = new ObservableCollection<ItemsModel>();

                var GetItems = (await fc
                  .Child("ItemTable")
                  .OnceAsync<ItemsModel>()).Select(item => new ItemsModel
                  {
                      Description = item.Object.Description,
                      Date = item.Object.Date,
                      Key = item.Key
                  });

                int count = 0;
                foreach (var item in GetItems)
                {

                    Items.Add(item);
                    count++;
                    if (count >= listcount)
                        break;
                }

        }

        /// <summary>
        /// Posts a new ItemsModel
        /// </summary>
        private async void Post()
        {

            if (!string.IsNullOrEmpty(InputDescription))
            {
                
                await fc.Child("ItemTable")
                 .PostAsync(new ItemsModel() { Description = InputDescription, Date = DateTime.Now.ToString()});

                GetData();

                InputDescription = null;

            }

        }

        /// <summary>
        /// Deletes one of the ItemsModel from the Items listview
        /// </summary>
        private async void Delete()
        {
            var selected = Items.Where(k => k.Key == SelectedKey.Key).FirstOrDefault();

            await fc.Child("ItemTable").Child(selected.Key).DeleteAsync();

            Items.Remove(selected);
        }

        /// <summary>
        /// Updates the Description of a specified ItemsModel in the Items listview
        /// </summary>
        private async void Update()
        {
            var selected = Items.Where(k => k.Key == SelectedKey.Key).FirstOrDefault();

            await fc.Child("ItemTable").Child(selected.Key).PutAsync(new ItemsModel() { Description = selected.Description, Date = selected.Date });

        }


        /// <summary>
        /// Refreshes the listview
        /// </summary>
        private void PerformRefresh()
        {
            GetData();
        }

        private ObservableCollection<ItemsModel> items;
        public ObservableCollection<ItemsModel> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }

        private string inputdescription;
        public string InputDescription
        {
            get => inputdescription;
            set
            {
                inputdescription = value;
                OnPropertyChanged();
            }
        }

        private ItemsModel selectedkey;
        public ItemsModel SelectedKey
        {
            get => selectedkey;
            set
            {

                selectedkey = value;
                OnPropertyChanged();
            }
        }

        private bool isRefreshing = false;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
    }

}
