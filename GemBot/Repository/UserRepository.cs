using GemBot.Models;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemBot.Repository
{
    public class UserRepository
    {
        private readonly FirestoreDb _firestoreDb;
        private const string _collectionName = "Users";

        public UserRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<User> GetUser(ulong userId)
        {
            var snapshot = await _firestoreDb.Document($"{_collectionName}/{userId}").GetSnapshotAsync().ConfigureAwait(false);
            return snapshot.ConvertTo<User>();
        }

        public async Task<List<User>> GetUsers(ulong userId)
        {
            var snapshot = await _firestoreDb.Document(_collectionName).GetSnapshotAsync().ConfigureAwait(false);
            return snapshot.ConvertTo<List<User>>();
        }

        public async Task UserCreateOrUpdate(User user)
        {
            await _firestoreDb.Collection(_collectionName).Document(user.UserId).SetAsync(user, SetOptions.MergeAll).ConfigureAwait(false);
        }
    }
}
