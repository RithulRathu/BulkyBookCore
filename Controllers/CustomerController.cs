﻿using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
