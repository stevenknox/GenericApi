﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi;
using Microsoft.AspNetCore.Mvc;
using SampleWebApi.Models;

namespace SampleWebApi.Controllers
{
    public class HomeController : Controller
    {
        private IGenericService<Product, Guid, StoreDbContext> _service;
        public HomeController(IGenericService<Product, Guid, StoreDbContext> service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            var products = _service.GetAll();

            return View(products);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
