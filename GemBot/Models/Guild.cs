using Google.Cloud.Firestore;

namespace GemBot.Models
{
    [FirestoreData]
    public class Guild
    {
        [FirestoreProperty("GuildName")]
        public string GuildName { get; set; }
        [FirestoreProperty("NickName")]
        public string? NickName { get; set; }
        [FirestoreProperty("JoinedChannel")]
        public Timestamp JoinedChannel { get; set; }
        [FirestoreProperty("Expirence")]
        public int Expirence { get; set; }
    }
}
