using KA_SWD63B_Cloud_HA.DataAccess;
using KA_SWD63B_Cloud_HA.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KA_SWD63B_Cloud_HA.Controllers
{
    
    public class VideosController : Controller
    {
        FirestoreVideosRepository _videosRepo;
        public VideosController(FirestoreVideosRepository videosRepo) {
            _videosRepo = videosRepo;
        }
        public IActionResult Index( )
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Video v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _videosRepo.AddVideo(v);
                    TempData["success"] = "Video was added successfully in database";
                }
                catch (Exception e)
                {
                    TempData["error"] = "Failed to add Video to database";
                }
            }
            return View();
        }
    }
}
