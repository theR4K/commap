using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using commapModels.Models;
using meetCreationModule.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace meetCreationModule.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MeetCreationController : ControllerBase
    {
        private readonly MeetCreationService _meetCreationService;

        public MeetCreationController(MeetCreationService service)
        {
            _meetCreationService = service;
        }

        [HttpGet]
        public string Get()
        {
            return "OK";
        }

        // CREATE
        [HttpPost]
        public ActionResult<Meet> Post(Meet meet)
        {
            if (meet.EndTime <= meet.StartTime)
                return BadRequest();
            _meetCreationService.Create(meet);

            return meet;
        }

        // UPDATE
        [HttpPut("{id}")]
        public void Put(string id, Meet meet)
        {
        }

        // DELETE api/<MeetCreationController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            //TODO not found

            if (_meetCreationService.Remove(id))
                return Ok();
            return NotFound();
        }
    }
}
