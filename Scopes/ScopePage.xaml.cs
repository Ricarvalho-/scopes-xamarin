using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase.Database;
using Scopes.Model;
using Scopes.Service;
using Xamarin.Essentials;

namespace Scopes
{
    public partial class ScopePage : ContentPage
    {
        private readonly GoalFirebaseService Service;
        private FirebaseObject<Scope> Scope;

        public ScopePage()
        {
            InitializeComponent();
            Service = new GoalFirebaseService(App.Firebase);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Scope = (FirebaseObject<Scope>)BindingContext;
            FetchGoals();
        }

        private void FetchGoals()
        {
            Task.Run(async () =>
            {
                var goals = await Service.AllGoalsOfScope(Scope);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    GoalsListView.ItemsSource = goals;
                    GoalsListView.IsRefreshing = false;

                    var isEmpty = goals.Count == 0;
                    DoneProgressBar.IsVisible = !isEmpty;
                    if (isEmpty) return;

                    var doingGoals = goals.FindAll((goal) => goal.Object.IsDoing).Count;
                    var doneGoals = goals.FindAll((goal) => goal.Object.IsDone).Count;
                    DoneProgressBar.Progress = (doneGoals + (float)doingGoals / 2) / goals.Count;
                });
            });
        }

        void OnAddClicked(Object sender, EventArgs e)
        {
            var title = DisplayPromptAsync("New goal", null, placeholder: "Title");
            Task.Run(async () =>
            {
                var newTitle = await title;
                if (newTitle.Trim().Length == 0) return;

                await Service.CreateGoal(Scope, newTitle);
                FetchGoals();
            });
        }

        void OnGoalsRefresh(object sender, EventArgs e)
        {
            FetchGoals();
        }

        void OnItemRename(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            var goal = (FirebaseObject<Goal>)item.CommandParameter;

            var title = DisplayPromptAsync(
                "Rename " + goal.Object.Title,
                null,
                placeholder: "New title",
                initialValue: goal.Object.Title);

            Task.Run(async () =>
            {
                var newTitle = await title;
                if (newTitle.Trim().Length == 0) return;

                await Service.UpdateGoal(Scope, goal, newTitle);
                FetchGoals();
            });
        }

        void OnItemDelete(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            var goal = (FirebaseObject<Goal>)item.CommandParameter;

            var title = "Delete " + goal.Object.Title + "?";
            var delete = DisplayAlert(title, null, "Yes", "Cancel");

            Task.Run(async () =>
            {
                if (await delete)
                {
                    await Service.DeleteGoal(Scope, goal);
                    FetchGoals();
                }
            });
        }

        void OnItemCancel(object sender, EventArgs e)
        {
            var item = (Button)sender;
            var goal = (FirebaseObject<Goal>)item.CommandParameter;
            goal.Object.Cancel();

            Task.Run(async () =>
            {
                await Service.UpdateGoal(Scope, goal);
                FetchGoals();
            });
        }

        void OnItemBegin(object sender, EventArgs e)
        {
            var item = (Button)sender;
            var goal = (FirebaseObject<Goal>)item.CommandParameter;
            goal.Object.Begin();

            Task.Run(async () =>
            {
                await Service.UpdateGoal(Scope, goal);
                FetchGoals();
            });
        }

        void OnItemFinish(object sender, EventArgs e)
        {
            var item = (Button)sender;
            var goal = (FirebaseObject<Goal>)item.CommandParameter;
            goal.Object.Finish();

            Task.Run(async () =>
            {
                await Service.UpdateGoal(Scope, goal);
                FetchGoals();
            });
        }
    }
}
