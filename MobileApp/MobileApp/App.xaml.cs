using MobileApp.Models;
using MobileApp.Data;
using MobileApp.Services;
using Xamarin.Forms;
using MobileApp.ViewModels;
using MobileApp.Views;
using System;

namespace MobileApp
{
    public partial class App : Application
    {
        private static TodoItemDatabase _database; //This is where we attach our databases to the app. _database is a dummy todolist database 
        private static IOTButtonDatabase _iotDatabase;
        private static MoistMeterDatabase _mmDatabase;

        public static IMQTT Client { get; private set; }

        public static string ServerIP { get; set; }
        public static int ServerPort { get; set; }

        public App()
        {
            //App.Client = new MQTTClient();
            App.Client = new MQTTMockClient();
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

        public static MoistMeterDatabase MoistMeterDatabase
        {
            get
            {
                if (_mmDatabase == null)
                {
                    _mmDatabase = new MoistMeterDatabase();
                }
                return _mmDatabase;
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
