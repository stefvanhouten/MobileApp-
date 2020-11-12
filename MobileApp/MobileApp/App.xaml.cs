using MobileApp.Services;
using Xamarin.Forms;

namespace MobileApp
{
    public partial class App : Application
    {
        public static MQTTClient Client { get; private set; } = new MQTTClient();

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
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
