﻿using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
