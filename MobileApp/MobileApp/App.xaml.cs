using MobileApp.Models;
using MobileApp.Data;
using MobileApp.Services;
using Xamarin.Forms;

namespace MobileApp
{
    public partial class App : Application
    {
        public static MQTTClient Client { get; private set; } = new MQTTClient(); //Static because only one instance of the app anyway and we want it to be globally available
        private static TodoItemDatabase _database; //This is where we attach our databases to the app. _database is a dummy todolist database 
        private static IOTButtonDatabase _iotDatabase;

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        public static TodoItemDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new TodoItemDatabase();
                }
                return _database;
            }
        }

        public static IOTButtonDatabase IOTDatabase
        {
            get
            {
                if (_iotDatabase == null)
                {
                    _iotDatabase = new IOTButtonDatabase();
                }
                return _iotDatabase;
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
