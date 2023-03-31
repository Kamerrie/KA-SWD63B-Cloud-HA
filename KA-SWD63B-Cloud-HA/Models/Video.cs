using Google.Api;
using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1.Data;
using Google.Cloud.Firestore;
using Google.Type;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace KA_SWD63B_Cloud_HA.Models
{
    [FirestoreData]
    public class Video
    {
        [FirestoreProperty]
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [FirestoreProperty]
        [Required]
        public string title { get; set; }
        [FirestoreProperty]
        [Required]
        public System.DateTime dateUploaded { get; set; } = System.DateTime.UtcNow;
        [FirestoreProperty]
        //[Required]
        //[EmailAddress]
        public string email { get; set; }// = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
    }
}
