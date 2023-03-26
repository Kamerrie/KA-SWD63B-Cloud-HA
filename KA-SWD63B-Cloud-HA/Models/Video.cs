using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace KA_SWD63B_Cloud_HA.Models
{
    [FirestoreData]
    public class Video
    {
        [FirestoreProperty]
        [Required]
        public string title { get; set; }
        [FirestoreProperty]
        public DateAndTime dateUploaded { get; }
        [FirestoreProperty]
        [Required]
        public UserCredential user { get; set; }

    }
}
