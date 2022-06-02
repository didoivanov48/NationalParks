using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NationalParksWeb.Models;
using NationalParksWeb.Models.ViewModel;
using NationalParksWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NationalParksWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepository npRepository;
        private readonly IAccountRepository accRepository;
        private readonly ITrailRepository trRepository;
        public HomeController(ILogger<HomeController> logger, ITrailRepository trRepository, INationalParkRepository npRepository, IAccountRepository accRepository)
        {
            _logger = logger;
            this.trRepository = trRepository;
            this.npRepository = npRepository;
            this.accRepository = accRepository;
        }

        public async Task<IActionResult> Index()
        {
            IndexVM listOfParkAndTrails = new IndexVM()
            {
                NationalParkList = await npRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken")),
                TrailList = await trRepository.GetAllAsync(SD.TrailAPIPath, HttpContext.Session.GetString("JWToken")),
            };
            return View(listOfParkAndTrails);
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
        [HttpGet]
        public IActionResult Login()
        {
            User obj = new User();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User obj)
        {
            User objUser = await accRepository.LoginAsync(SD.AccountAPIPath + "authenticate/", obj);
            if (objUser.Token == null)
            {
                return View();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, objUser.Username));
            identity.AddClaim(new Claim(ClaimTypes.Role, objUser.Role));
            var pricipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, pricipal);

            HttpContext.Session.SetString("JWToken", objUser.Token);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User obj)
        {
            bool result = await accRepository.RegisterAsync(SD.AccountAPIPath + "register/", obj);
            if (result == null)
            {
                return View();
            }

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString("JWToken", "");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
