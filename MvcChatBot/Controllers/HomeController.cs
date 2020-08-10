using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AirtableApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcChatBot.Models;
using MvcChatBot.Services;

namespace MvcChatBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AirtableService _airtableService;

        public HomeController(ILogger<HomeController> logger,
            AirtableService airtableService)
        {
            _logger = logger;
            _airtableService = airtableService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Goal()
        {
            
            ViewBag.Count  = await _airtableService.GetCount();
            
            return View();
        }
        [Route("rain")]
        public IActionResult Destructopups()
        {
            return View();
        }

        [Route("sound")]
        public IActionResult Sound()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}