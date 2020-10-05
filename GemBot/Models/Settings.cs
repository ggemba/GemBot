using Google.Cloud.Firestore;

namespace GemBot.Models
{
    [FirestoreData]
    public class Settings
    {
        [FirestoreProperty("Prefix")]
        public string Prefix { get; set; }
        [FirestoreProperty("Token")]
        public string Token { get; set; }
    }
}
