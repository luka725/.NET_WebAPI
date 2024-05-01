using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication.Controllers
{
    public class ValuesController : ApiController
    {
        private static List<string> _values = new List<string> { "Value1", "Value2" };
        // GET api/values
        public IEnumerable<string> Get()
        {
            return _values;
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            if (id >= 0 && id < _values.Count)
            {
                return Ok(_values[id]);
            }
            else
            {
                return NotFound();
            }
            
        }

        // POST api/values
        public IHttpActionResult Post([FromBody] string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _values.Add(value);
                return Ok();
            }
            else
            {
                return BadRequest("Value cannot be empty!");
            }
        }

        // PUT api/values/5
        public IHttpActionResult Put(int id, [FromBody] string value)
        {
            if (id >= 0 && id < _values.Count && !string.IsNullOrEmpty(value))
            {
                _values[id] = value;
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE api/values/5
        public IHttpActionResult Delete(int id)
        {
            if (id >= 0 && id < _values.Count)
            {
                _values.RemoveAt(id);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
