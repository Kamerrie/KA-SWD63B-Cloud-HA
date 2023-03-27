using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace KA_SWD63B_Cloud_HA.Models
{
    [FirestoreData]
    public class Account
    {
        [FirestoreProperty]
        [Required]

        public int accountID { get; set; }
        [FirestoreProperty]
        public int names { get; set; }
    }
}
