using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using KA_SWD63B_Cloud_HA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KA_SWD63B_Cloud_HA.DataAccess
{
    public class FirestoreVideosRepository
    {
        FirestoreDb db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StorageClient _storageClient;
        public FirestoreVideosRepository(string project, IHttpContextAccessor httpContextAccessor)
        {
            db = FirestoreDb.Create(project);
            _httpContextAccessor = httpContextAccessor;
            _storageClient = StorageClient.Create();
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

        public async Task<string> GetVideoFileName(string Id)
        {
            DocumentReference docRef = db.Collection("videos").Document(Id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Video v = snapshot.ConvertTo<Video>();
                return v.VideoUrl;
            }
            else
            {
                return null;
            }
        }

        public async Task AddVideo(Video v, IFormFile videoFile, IFormFile thumbnailFile)
        {
            var bucketName = "cloud_programming_ha";
            var vFileName = $"{v.Id}-video-{videoFile.FileName}";
            var tFileName = $"{v.Id}-thumbnail-{thumbnailFile.FileName}";

            if (videoFile != null && thumbnailFile != null)
            {
                var storage = StorageClient.Create();


                using var videoMemoryStream = videoFile.OpenReadStream();
                using var thumbnailMemoryStream = thumbnailFile.OpenReadStream();

                storage.UploadObject(bucketName, vFileName, null, videoMemoryStream);
                storage.UploadObject(bucketName, tFileName, null, thumbnailMemoryStream);

                var storageVideoObject = storage.GetObject(bucketName, vFileName);
                var storageThumbnailObject = storage.GetObject(bucketName, tFileName);


                //await videoFile.CopyToAsync(videoMemoryStream);
                //videoMemoryStream.Seek(0, SeekOrigin.Begin);

                //var vStorageObject = await _storageClient.UploadObjectAsync(bucketName, vFileName, videoFile.ContentType, videoMemoryStream);
                v.VideoUrl = $"https://storage.googleapis.com/{bucketName}/{vFileName}";

                //await thumbnailFile.CopyToAsync(thumbnailMemoryStream);
                //thumbnailMemoryStream.Seek(0, SeekOrigin.Begin);
                
                //var tStorageObject = await _storageClient.UploadObjectAsync(bucketName, tFileName, thumbnailFile.ContentType, thumbnailMemoryStream);
                v.ThumbnailUrl = $"https://storage.googleapis.com/{bucketName}/{tFileName}";
            }
            else
            {
                throw new ArgumentException("Both videoFile and thumbnailFile must be non-null");
            }

            await db.Collection("videos").Document(v.Id).SetAsync(v);
        }

        public async Task AddDownloadHistoryAsync(string videoId, DownloadHistory downloadHistory)
        {
            var videoRef = db.Collection("videos").Document(videoId);
            var downloadHistoryRef = videoRef.Collection("downloadHistory").Document(downloadHistory.Id);
            await downloadHistoryRef.SetAsync(downloadHistory);
        }


    }
}
