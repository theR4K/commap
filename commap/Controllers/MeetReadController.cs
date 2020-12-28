using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace commap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetReadController : ControllerBase
    {
        // GET: api/<MeetReadController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MeetReadController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MeetReadController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MeetReadController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MeetReadController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
