using Google.Cloud.Firestore;
using KA_SWD63B_Cloud_HA.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace KA_SWD63B_Cloud_HA.DataAccess
{
    public class FirestoreVideosRepository
    {
        FirestoreDb db;
        public FirestoreVideosRepository(string project)
        {
            db = FirestoreDb.Create(project);
            Console.WriteLine("Created Cloud Firestore client with project ID: {0}", project);
        }

        public List<Video> GetVideos()
        {

        }

        public void AddVideo(Video v)
        {

        }
    }
}
