using Google.Cloud.Storage.V1;
using KA_SWD63B_Cloud_HA.DataAccess;
using KA_SWD63B_Cloud_HA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KA_SWD63B_Cloud_HA.Controllers
{
    [Authorize]
    public class VideosController : Controller
    {
        FirestoreVideosRepository _videosRepo;
        public VideosController(FirestoreVideosRepository videosRepo) {
            _videosRepo = videosRepo;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _videosRepo.GetAccountVideos();
            return View(list);
        }

        public async Task<IActionResult> Details(string Id)
        {
            var videoDetails = await _videosRepo.GetVideo(Id);
            return View(videoDetails);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            //_videosRepo.Update(v);
            var videoDetails = await _videosRepo.GetVideo(Id);
            return View(videoDetails);
        }

        [HttpPost]
        public IActionResult Edit(Video v)
        {
            _videosRepo.Update(v);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile videoFile, IFormFile thumbnailFile)
        {
            var v = new Video();
            // Retrieve the authenticated user's email
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // Set the email property of the video object to the user's email
            v.email = email;
            v.title = videoFile.FileName;

            if (videoFile != null && videoFile.Length > 0 && thumbnailFile != null && thumbnailFile.Length > 0)
            {
                try
                {
                    await _videosRepo.AddVideo(v, videoFile, thumbnailFile);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false });
                }
            }
            else
            {
                return Json(new { success = false });
            }
        }
        /*
        [HttpPost]
        public IActionResult Create(Video v, IFormFile videoFile, IFormFile thumbnailFile)
        {
            
            // Retrieve the authenticated user's email
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // Set the email property of the video object to the user's email
            v.email = email;
            v.title = videoFile.FileName;
            
            if (//ModelState.IsValid &&
                videoFile != null && videoFile.Length > 0 && thumbnailFile != null && thumbnailFile.Length > 0)
            {
                try
                {
                    _videosRepo.AddVideo(v, videoFile, thumbnailFile);
                    TempData["success"] = "Video was added successfully in database";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Failed to add Video to database";
                }
            }
            else
            {
                TempData["error"] = "Please select a file to upload.";
                return View();
            }
            return View();
        }
        */

        [HttpGet]
        public async Task<IActionResult> DownloadVideo(string Id)
        {
            string videoUrl = await _videosRepo.GetVideoFileName(Id);


            if (string.IsNullOrEmpty(videoUrl))
            {
                return NotFound();
            }

            //Download history updated
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var dh = new DownloadHistory();
            dh.UserEmail = email;
            await _videosRepo.AddDownloadHistoryAsync(Id, dh);

            string bucketName = "cloud_programming_ha";
            string videoFileName = videoUrl.Substring(videoUrl.LastIndexOf('/') + 1);

            var storage = StorageClient.Create();
            var stream = new MemoryStream();
            storage.DownloadObject(bucketName, videoFileName, stream);
            stream.Seek(0, SeekOrigin.Begin);

            string contentType = "application/octet-stream";
            string fileExtension = Path.GetExtension(videoFileName);

            if (!string.IsNullOrEmpty(fileExtension))
            {
                contentType = GetContentType(fileExtension);
            }

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = videoFileName
            };
        }

        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".mp4":
                    return "video/mp4";
                case ".webm":
                    return "video/webm";
                default:
                    return "application/octet-stream";
            }
        }


        [HttpPost]
        public IActionResult Delete(string Id)
        {
            
            try
            {
                _videosRepo.Delete(Id);
                TempData["success"] = "Video was deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to delete video";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PublishMessage(string videoUrl)
        {
            if (!string.IsNullOrEmpty(videoUrl))
            {
                await _videosRepo.PublishMessageToPubSub(videoUrl);
                TempData["success"] = "Message published to Pub/Sub successfully.";
            }
            else
            {
                TempData["error"] = "Video URL is missing.";
            }

            return RedirectToAction("Index");
        }

    }
}
