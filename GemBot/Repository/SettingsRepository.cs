using GemBot.Models;
using Google.Cloud.Firestore;
using System.Threading.Tasks;

namespace GemBot.Repository
{
    class SettingsRepository
    {
        private readonly FirestoreDb _firestoreDb;
        private const string _collectionName = "SettingsBot";

        public SettingsRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<Settings> GetSettingsBot()
        {
            var snapshot = await _firestoreDb.Document($"{_collectionName}/GemBot").GetSnapshotAsync().ConfigureAwait(false);
            Settings SettingsSnapshot = snapshot.ConvertTo<Settings>();

            return SettingsSnapshot;
        }
    }
}
