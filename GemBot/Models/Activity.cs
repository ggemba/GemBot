using Google.Cloud.Firestore;

namespace GemBot.Models
{
    [FirestoreData]
    public class Activity
    {
        [FirestoreProperty("JoinedGame")]
        public Timestamp JoinGame { get; set; }
        [FirestoreProperty("Minutes")]
        public int Minutes { get; set; }
    }
}
