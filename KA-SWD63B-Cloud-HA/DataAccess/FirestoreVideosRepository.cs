using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using KA_SWD63B_Cloud_HA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xabe.FFmpeg;

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

        public async Task AddVideo(Video v, IFormFile videoFile)
        {
            var bucketName = "cloud_programming_ha";
            var videoFileName = $"{v.Id}-{videoFile.FileName}";
            var thumbnailFileName = $"{v.Id}-thumbnail.jpg";

            using (var memoryStream = new MemoryStream())
            {
                await videoFile.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var ffmpeg = FFmpeg.Conversions.New();
                // Generate thumbnail from the video
                using (var ffmpeg = new Engine(@"C:\ffmpeg\ffmpeg.exe"))
                {
                    var inputFile = @"C:\path\to\input\video.mp4";
                    var outputFile = @"C:\path\to\output\image.jpg";
                    var frameNo = 10; // the frame number you want to extract

                    var conversion = ffmpeg.Conversions.New()
                        .ExtractNthFrame(frameNo, fileName => outputFile);

                    var result = await conversion.Start();
                }

                var thumbnail = ffmpeg.ExtractNthFrame(memoryStream, TimeSpan.FromSeconds(1));

                // Upload video to Google Cloud Storage
                var videoStorageObject = await _storageClient.UploadObjectAsync(
                    bucketName, videoFileName, videoFile.ContentType, memoryStream);

                // Upload thumbnail to Google Cloud Storage
                var thumbnailStorageObject = await _storageClient.UploadObjectAsync(
                    bucketName, thumbnailFileName, "image/jpeg", thumbnail);

                v.VideoUrl = $"https://storage.googleapis.com/{bucketName}/{videoFileName}";
                v.ThumbnailUrl = $"https://storage.googleapis.com/{bucketName}/{thumbnailFileName}";

                await db.Collection("videos").Document(v.Id).SetAsync(v);
            }
        }

    }
}
