using GemBot.Models;
using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

namespace GemBot.Repository
{
    public class LogRepository
    {
        private readonly FirestoreDb _firestoreDb;

        public LogRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task InsertLogError(string pEvent, string method, string error)
        {
            Log log = new Log
            {
                Event = pEvent,
                Method = method,
                Error = error,
                DateTime = DateTime.Now.ToString()
            };
            await _firestoreDb.Collection("Errors").AddAsync(log).ConfigureAwait(false);
        }
    }
}
