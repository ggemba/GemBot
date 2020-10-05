using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GemBot.Models
{
    [FirestoreData]
    public class Log
    {
        [FirestoreProperty("Event")]
        public string Event { get; set; }
        [FirestoreProperty("Method")]
        public string Method { get; set; }
        [FirestoreProperty("Error")]
        public string Error { get; set; }
        [FirestoreProperty("DateTime")]
        public string DateTime { get; set; }
    }
}
