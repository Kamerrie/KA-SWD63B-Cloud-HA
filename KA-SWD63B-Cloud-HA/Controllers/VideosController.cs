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
