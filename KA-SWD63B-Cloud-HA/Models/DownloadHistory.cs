using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace KA_SWD63B_Cloud_HA.Models
{
    [FirestoreData]
    public class DownloadHistory
    {
        [FirestoreProperty]
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [FirestoreProperty]
        public string UserEmail { get; set; }
        [FirestoreProperty]
        [Required]
        public DateTime DownloadTimestamp { get; set; } = DateTime.UtcNow;
    }
}
