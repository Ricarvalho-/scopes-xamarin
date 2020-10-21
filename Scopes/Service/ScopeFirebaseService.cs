using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Scopes.Model;

namespace Scopes.Service
{
    public class ScopeFirebaseService
    {
        private readonly FirebaseClient Database;

        public ScopeFirebaseService(FirebaseClient firebaseClient)
        {
            Database = firebaseClient;
        }

        public async Task<List<FirebaseObject<Scope>>> AllScopes()
        {
            return (await Database.Child("scopes").OnceAsync<Scope>()).ToList();
        }

        public async Task<FirebaseObject<Scope>> CreateScope(string title)
        {
            return await Database.Child("scopes").PostAsync(new Scope
            {
                Title = title
            });
        }

        public async Task UpdateScope(FirebaseObject<Scope> scope, string title)
        {
            await Database.Child("scopes").Child(scope.Key).PutAsync(new Scope
            {
                Title = title
            });
        }

        public async Task DeleteScope(FirebaseObject<Scope> scope)
        {
            await Task.WhenAll(
                Database.Child("scopes").Child(scope.Key).DeleteAsync(),
                Database.Child("goals").Child(scope.Key).DeleteAsync()
                );
        }
    }
}
