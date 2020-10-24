using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase.Database;
using Scopes.Model;
using Scopes.Service;
using Xamarin.Essentials;

namespace Scopes
{
    public partial class MainPage : ContentPage
    {
        private readonly ScopeFirebaseService Service;

        public MainPage()
        {
            InitializeComponent();
            Service = new ScopeFirebaseService(App.Firebase);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FetchScopes();
        }

        private void FetchScopes()
        {
            Task.Run(async () =>
            {
                var scopes = await Service.AllScopes();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ScopesListView.ItemsSource = scopes;
                    ScopesListView.IsRefreshing = false;
                });
            });
        }

        void OnAddClicked(Object sender, EventArgs e)
        {
            var title = DisplayPromptAsync("New scope", null, placeholder: "Title");
            Task.Run(async () =>
            {
                var newTitle = await title;
                if (newTitle.Trim().Length == 0) return;

                var scope = await Service.CreateScope(newTitle);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PushAsync(new ScopePage
                    {
                        BindingContext = scope
                    });
                });
            });
        }

        void OnScopesRefresh(object sender, EventArgs e)
        {
            FetchScopes();
        }

        void OnScopeSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            ScopesListView.SelectedItem = null;

            Navigation.PushAsync(new ScopePage {
                BindingContext = e.SelectedItem
            });
        }

        void OnItemRename(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            var scope = (FirebaseObject<Scope>)item.CommandParameter;

            var title = DisplayPromptAsync(
                "Rename " + scope.Object.Title,
                null,
                placeholder: "New title",
                initialValue: scope.Object.Title);
            Task.Run(async () =>
            {
                var newTitle = await title;
                if (newTitle.Trim().Length == 0) return;

                await Service.UpdateScope(scope, newTitle);
                FetchScopes();
            });
        }

        void OnItemDelete(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            var scope = (FirebaseObject<Scope>)item.CommandParameter;

            var title = "Delete " + scope.Object.Title + "?";
            var delete = DisplayAlert(title, null, "Yes", "Cancel");

            Task.Run(async () =>
            {
                if (await delete)
                {
                    await Service.DeleteScope(scope);
                    FetchScopes();
                }
            });
        }
    }
}
