using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Cloud.Firestore;
using KA_SWD63B_Cloud_HA.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KA_SWD63B_Cloud_HA.DataAccess
{
    public class UserAccountsRepository
    {
        FirestoreDb db;
        public UserAccountsRepository(string project)
        {
            db = FirestoreDb.Create(project);
        }

        //This was for getting a list of documents from a collection, this should only return the 1 document for the 1 unique-id
        //public async Task<List<Video>> GetAccount()
        //{
        //    List<Video> videos = new List<Video>();
        //    Query allVideosQuery = db.Collection("videos");
        //    QuerySnapshot allVideosQuerySnapshot = await allVideosQuery.GetSnapshotAsync();
        //    foreach (DocumentSnapshot documentSnapshot in allVideosQuerySnapshot.Documents)
        //    {
        //        Video v = documentSnapshot.ConvertTo<Video>();
        //        videos.Add(v);
        //        
        //    }
        //    return videos;
        //}

        public void Update (Video v)
        {

        }
        /*
        public async Task<Account> GetAccount(string accountId)
        {
            PeopleResource.GetRequest peopleRequest = peopleService.People.Get("people/me");
            peopleRequest.PersonFields = "names,emailAddresses";
            Person profile = peopleRequest.Execute();
        }

        */
        //for use later down the line
        //public async Task<Video> GetAccountVideos(string accountId)
        //{
        //
        //}


        //so far, we have no use for delecting an account, so im not implementing it XD
        //public void Delete (string documentId) { }

        public async void AddAccount(Account a)
        {
            await db.Collection("accounts").Document().SetAsync(a);
        }
    }
}
