using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrailsAPI.Repository.IRepository;
using System.Collections.Generic;
using TrailsAPI.Repository.IRepository;
using NationalParksAPI.Models.DTOs;
using NationalParksAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrailsAPI.Controllers
{
    [Route("api/trails")]
    [ApiController]
    //[Authorize(Roles ="Admin")]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository trRepository;
        private readonly IMapper mapper;

        public TrailsController(ITrailRepository trRepository, IMapper mapper)
        {
            this.trRepository = trRepository;
            this.mapper = mapper;
        }

        
        [HttpGet]
        public IActionResult GetTrails()
        {
            var objList = trRepository.GetTrails();
            var objDto = new List<TrailDTO>();
            foreach (var obj in objList)
            {
                objDto.Add(mapper.Map<TrailDTO>(obj));
            }
            return Ok(objDto);
        }
        
        [HttpGet("{trailId:int}", Name = "GetTrail")]
        public IActionResult GetTrail(int trailId)
        {
            var obj = trRepository.GetTrail(trailId);
            if (obj == null)
            {
                return NotFound();
            }
            var objDto = mapper.Map<TrailDTO>(obj);
            return Ok(objDto);
        }

        [HttpPost]
        public IActionResult CreateTrail([FromBody] TrailCreateDTO trailDTO)
        {
            if (trailDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (trRepository.TrailExists(trailDTO.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }

            var trailObj = mapper.Map<Trail>(trailDTO);

            if (!trRepository.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { trailID = trailObj.Id }, trailObj);
        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        public IActionResult UpdateTrail(int trailID, [FromBody] TrailUpdateDTO trailDTO)
        {
            if (trailDTO == null || trailID != trailDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var trailObj = mapper.Map<Trail>(trailDTO);

            if (!trRepository.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        public IActionResult DeleteTrail(int trailID)
        {
            if (!trRepository.TrailExists(trailID))
            {
                return NotFound();
            }

            var trailObj = trRepository.GetTrail(trailID);

            if (!trRepository.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }
    }
}
