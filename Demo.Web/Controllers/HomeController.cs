using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Demo.Web.Models;
using Demo.Definitions;
using Demo.Models;
using Microsoft.AspNetCore.SignalR;
using Demo.Web.Hubs;

namespace Demo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<ICustomer> _repo = null;
        IHubContext<NotificationHub> _hubContext = null;
        public HomeController(IRepository<ICustomer> repo, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Customer(int id)
        {
            var customer = _repo.GetById(id);
            return View(customer);
        }

      
        public async Task<IActionResult> UpdateCustomer([FromForm] Customer customer)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", customer);
            return Ok(customer);
        }
        
    }
}
