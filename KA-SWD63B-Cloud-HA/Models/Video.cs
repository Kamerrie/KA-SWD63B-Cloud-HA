using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Type;
using Microsoft.VisualBasic;
using System;
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
        [Required]
        public System.DateTime dateUploaded { get; set; } = System.DateTime.UtcNow;
        //[FirestoreProperty]
        //[Required]
        //public userCredential user { get; set; }

    }
}
