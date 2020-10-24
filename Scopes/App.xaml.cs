using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase.Database;

namespace Scopes
{
    public partial class App : Application
    {
        internal static readonly FirebaseClient Firebase = new FirebaseClient("https://scopes-8fa88.firebaseio.com/");

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
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
