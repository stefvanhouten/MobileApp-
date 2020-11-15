using MobileApp.Models;
using MobileApp.Services;
using Xamarin.Forms;

namespace MobileApp
{
    public partial class App : Application
    {
        public static MQTTClient Client { get; private set; } = new MQTTClient();
        private static TodoItemDatabase _database;
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
