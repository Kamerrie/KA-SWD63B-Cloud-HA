using KA_SWD63B_Cloud_HA.DataAccess;
using KA_SWD63B_Cloud_HA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KA_SWD63B_Cloud_HA.Controllers
{
    
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
        public IActionResult Create(Video v)
        {
            
            // Retrieve the authenticated user's email
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // Set the email property of the video object to the user's email
            v.email = email;
            
            if (ModelState.IsValid)
            {
                try
                {
                    _videosRepo.AddVideo(v);
                    TempData["success"] = "Video was added successfully in database";
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Failed to add Video to database";
                }
            }
            else
            {

            }
            return View();
        }

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
    }
}
