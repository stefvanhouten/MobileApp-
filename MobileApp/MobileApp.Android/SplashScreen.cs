using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace MobileApp.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash",
        MainLauncher = true, NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        LaunchMode = LaunchMode.SingleTop)]
    public class SplashScreen : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //SetContentView(Resource.Layout.Splash);
            //FindViewById<TextView>(Resource.Id.txtAppVersion).Text = "IOT Groep 5";
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            await Task.Delay(3000); // Simulate a bit of startup work.
            var mainActivityIntent = new Intent(Application.Context, typeof(MainActivity));
            mainActivityIntent.AddFlags(ActivityFlags.NoAnimation); //Add this line
            StartActivity(mainActivityIntent);
        }
    }
}
