using System;
using System.Collections.Generic;
using reportModule.Services;
using reportModule.Models;
using commapModels.Models;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace reportModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly MeetReadService _meetService;

        public ReportController(MeetReadService service)
        {
            _meetService = service;
        }
        // GET: api/<Controller>
        [HttpGet]
        public ActionResult<List<Meet>> Get(MeetState state, string name, string subject, double lat, double lng, double radius)
        {
            List<FilterDefinition<Meet>> filtersList = new List<FilterDefinition<Meet>>();
            if(Request.Query.ContainsKey("name")) filtersList.Add(Builders<Meet>.Filter.Regex("Name", ".*"+name+".*"));
            if (Request.Query.ContainsKey("subject")) filtersList.Add(Builders<Meet>.Filter.Regex("Subject", ".*" + subject + ".*"));
            if (Request.Query.ContainsKey("state")) filtersList.Add(Builders<Meet>.Filter.Eq("State", state));
            if(Request.Query.ContainsKey("lat")&& Request.Query.ContainsKey("lng")&& Request.Query.ContainsKey("radius")) 
                filtersList.Add(Builders<Meet>.Filter.GeoWithinCenter("Location", lat, lng, radius));
            if (filtersList.Count < 1)
                return _meetService.Get();
            FilterDefinition<Meet> fd = Builders<Meet>.Filter.And(filtersList.ToArray());
            return _meetService.Get(fd);
        }
    }
}
