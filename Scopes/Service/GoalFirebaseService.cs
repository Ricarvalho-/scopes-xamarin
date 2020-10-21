using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Scopes.Model;

namespace Scopes.Service
{
    public class GoalFirebaseService
    {
        private readonly FirebaseClient Database;

        public GoalFirebaseService(FirebaseClient firebaseClient)
        {
            Database = firebaseClient;
        }

        public async Task<List<FirebaseObject<Goal>>> AllGoalsOfScope(FirebaseObject<Scope> scope)
        {
            return (await Database.Child("goals").Child(scope.Key).OnceAsync<Goal>()).ToList();
        }

        public async Task<FirebaseObject<Goal>> CreateGoal(FirebaseObject<Scope> scope, string title)
        {
            return await Database.Child("goals").Child(scope.Key).PostAsync(new Goal
            {
                Title = title,
                Progress = 0
            });
        }

        public async Task UpdateGoal(FirebaseObject<Scope> scope, FirebaseObject<Goal> goal, string title = null, int? progress = null)
        {
            await Database.Child("goals").Child(scope.Key).Child(goal.Key).PutAsync(new Goal
            {
                Title = title ?? goal.Object.Title,
                Progress = progress ?? goal.Object.Progress
            });
        }

        public async Task DeleteGoal(FirebaseObject<Scope> scope, FirebaseObject<Goal> goal)
        {
            await Database.Child("goals").Child(scope.Key).Child(goal.Key).DeleteAsync();
        }
    }
}
