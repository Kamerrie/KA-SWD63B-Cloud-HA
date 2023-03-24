using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.VisualBasic;

namespace KA_SWD63B_Cloud_HA.Models
{
    [FirestoreData]
    public class Video
    {
        [FirestoreProperty]
        public string title { get; set; }
        [FirestoreProperty]
        public DateAndTime dateUploaded { get; }
        [FirestoreProperty]
        public UserCredential user { get; set; }

    }
}
