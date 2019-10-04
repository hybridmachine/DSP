using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SignalProcessor;

namespace SignalProcessorService.Controllers
{
    [Route("api/v1/fft")]
    [ApiController]
    public class SignalProcessorController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public ActionResult<FrequencyDomain> Post([FromBody] List<Double> value)
        {
            FastFourierTransform fft = new FastFourierTransform();
            FrequencyDomain transform = fft.Transform(value);
            return Ok(transform);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
