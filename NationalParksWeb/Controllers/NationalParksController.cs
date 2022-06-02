using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalParksWeb.Models;
using NationalParksWeb.Repository.IRepository;
using System.IO;
using System.Threading.Tasks;

namespace NationalParksWeb.Controllers
{
    [Authorize]
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository npRepository;
        public NationalParksController(INationalParkRepository npRepository)
        {
            this.npRepository = npRepository;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();
            if (id == null)
            {
                //true for Insert/Create
                return View(obj);
            }
            //Flow will come here for update
            obj = await npRepository.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    obj.Picture = p1;
                }
                else
                {
                    var objFromDb = await npRepository.GetAsync(SD.NationalParkAPIPath, obj.Id, HttpContext.Session.GetString("JWToken"));
                    obj.Picture = objFromDb.Picture;
                }
                if (obj.Id == 0)
                {
                    await npRepository.CreateAsync(SD.NationalParkAPIPath, obj, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await npRepository.UpdateAsync(SD.NationalParkAPIPath + obj.Id, obj, HttpContext.Session.GetString("JWToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await npRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken")) });
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await npRepository.DeleteAsync(SD.NationalParkAPIPath, id, HttpContext.Session.GetString("JWToken"));
            if (status)
            {
                return Json(new { successs = true, message="Delete Sussessful!" });
            }
            return Json(new { successs = false, message = "Delete Not Sussessful!" });
        }
    }
}
