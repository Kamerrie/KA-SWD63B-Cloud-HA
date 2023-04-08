using Google.Cloud.Firestore;
using KA_SWD63B_Cloud_HA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KA_SWD63B_Cloud_HA.DataAccess
{
    public class FirestoreVideosRepository
    {
        FirestoreDb db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FirestoreVideosRepository(string project, IHttpContextAccessor httpContextAccessor)
        {
            db = FirestoreDb.Create(project);
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Video>> GetVideos()
        {
            List<Video> videos = new List<Video>();
            Query allVideosQuery = db.Collection("videos");
            QuerySnapshot allVideosQuerySnapshot = await allVideosQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allVideosQuerySnapshot.Documents)
            {
                Video v = documentSnapshot.ConvertTo<Video>();
                videos.Add(v);
                
            }
            return videos;
        }

        public async void Update (Video v)
        {
            string userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            DocumentReference docRef = db.Collection("videos").Document(v.Id);
            DocumentSnapshot documentSnapshot = await docRef.GetSnapshotAsync();
            if (documentSnapshot.Exists != true)
            {
                throw new Exception("Book does not exist!");
            }
            else
            {
                DocumentReference videoRef = db.Collection("videos").Document(documentSnapshot.Id);
                v.dateUploaded = v.dateUploaded.ToUniversalTime();
                v.email = userEmail;
                await videoRef.SetAsync(v);
            }
        }

        public async Task<Video> GetVideo(string Id)
        {
            DocumentReference docRef = db.Collection("videos").Document(Id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists) {
                Video v = snapshot.ConvertTo<Video>();
                return v;
            }
            else
            {
                return null;
            }
        }

        //for use later down the line
        public async Task<List<Video>> GetAccountVideos()
        {
            string userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            List<Video> videos = new List<Video>();
            Query allVideosQuery = db.Collection("videos").WhereEqualTo("email", userEmail);
            QuerySnapshot videoQuerySnapshot = await allVideosQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in videoQuerySnapshot.Documents)
            {
                Video v = documentSnapshot.ConvertTo<Video>();
                videos.Add(v);
            }
            return videos;
        }

        /*
        public async void Delete (string title) {
            Query videosQuery = db.Collection("videos").WhereEqualTo("title", title);
            QuerySnapshot videosQuerySnapshot = await videosQuery.GetSnapshotAsync();
            DocumentSnapshot documentSnapshot = videosQuerySnapshot.Documents.FirstOrDefault();
            if (documentSnapshot.Exists != true)
            {
                throw new Exception("Book does not exist!");
            }
            else
            {
                DocumentReference videoRef = db.Collection("videos").Document(documentSnapshot.Id);
                await videoRef.DeleteAsync();
            }
        }
        */
        public async void Delete (string Id)
        {
            DocumentReference videoRef = db.Collection("videos").Document(Id);
            await videoRef.DeleteAsync(); 
        }

        public async void AddVideo(Video v)
        {
            await db.Collection("videos").Document(v.Id).SetAsync(v);
        }
    }
}
