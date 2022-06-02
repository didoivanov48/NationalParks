using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalParksAPI.Models;
using NationalParksAPI.Repository.IRepository;
using System.Collections.Generic;

namespace NationalParksAPI.Controllers
{
    [Route("api/nationalparks")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository npRepository;
        private readonly IMapper mapper;

        public NationalParksController(INationalParkRepository npRepository, IMapper mapper)
        {
            this.npRepository = npRepository;
            this.mapper = mapper;
        }

        
        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = npRepository.GetNationalParks();
            var objDto = new List<NationalParkDTO>();
            foreach (var obj in objList)
            {
                objDto.Add(mapper.Map<NationalParkDTO>(obj));
            }
            return Ok(objDto);
        }
        
        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        [Authorize]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = npRepository.GetNationalPark(nationalParkId);
            if (obj == null)
            {
                return NotFound();
            }
            var objDto = mapper.Map<NationalParkDTO>(obj);
            return Ok(objDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (npRepository.NationalParkExists(nationalParkDTO.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }

            var nationalParkObj = mapper.Map<NationalPark>(nationalParkDTO);

            if (!npRepository.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { nationalParkID = nationalParkObj.Id }, nationalParkObj);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkID, [FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null || nationalParkID != nationalParkDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = mapper.Map<NationalPark>(nationalParkDTO);

            if (!npRepository.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        public IActionResult DeleteNationalPark(int nationalParkID)
        {
            if (!npRepository.NationalParkExists(nationalParkID))
            {
                return NotFound();
            }

            var nationalParkObj = npRepository.GetNationalPark(nationalParkID);

            if (!npRepository.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }
    }
}
