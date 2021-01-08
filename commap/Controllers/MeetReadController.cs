using System;
using System.Collections.Generic;
using commap.Services;
using commap.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace commap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetReadController : ControllerBase
    {

        private readonly MeetReadService _meetService;

        public MeetReadController(MeetReadService service)
        {
            _meetService = service;
        }
        // GET: api/<MeetReadController>
        [HttpGet]
        public ActionResult<List<Meet>> Get()
        {
            return _meetService.Get();
        }

    }
}
