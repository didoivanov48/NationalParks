using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NationalParksWeb.Models;
using NationalParksWeb.Models.ViewModel;
using NationalParksWeb.Repository.IRepository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NationalParksWeb.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository npRepository;
        private readonly ITrailRepository trRepository;
        public TrailsController(INationalParkRepository npRepository, ITrailRepository trRepository)
        {
            this.npRepository = npRepository;
            this.trRepository = trRepository;
        }

        public IActionResult Index()
        {
            return View(new Trail() { });
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await npRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));
            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Trail = new Trail()
            };
            if (id == null)
            {
                //true for Insert/Create
                return View(objVM);
            }
            //Flow will come here for update
            objVM.Trail = await trRepository.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
            if (objVM.Trail == null)
            {
                return NotFound();
            }
            return View(objVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if (ModelState.IsValid)
            {
                
                if (obj.Trail.Id == 0)
                {
                    await trRepository.CreateAsync(SD.TrailAPIPath, obj.Trail, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await trRepository.UpdateAsync(SD.TrailAPIPath + obj.Trail.Id, obj.Trail, HttpContext.Session.GetString("JWToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await npRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));
                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                    Trail = obj.Trail
                };
                return View(objVM);
            }
        }

        public async Task<IActionResult> GetAllTrail()
        {
            return Json(new { data = await trRepository.GetAllAsync(SD.TrailAPIPath, HttpContext.Session.GetString("JWToken")) });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await trRepository.DeleteAsync(SD.TrailAPIPath, id, HttpContext.Session.GetString("JWToken"));
            if (status)
            {
                return Json(new { successs = true, message="Delete Sussessful!" });
            }
            return Json(new { successs = false, message = "Delete Not Sussessful!" });
        }
    }
}
