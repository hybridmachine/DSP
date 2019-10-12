using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SignalProcessor;

namespace SignalProcessorService.Controllers
{
    [Route("api/v1/fft")]
    [ApiController]
    public class SignalProcessorController : ControllerBase
    {
        /// <summary>
        /// Given a time series list, perform the FFT on it and return the frequency domain information
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<FrequencyDomain> FFT([FromBody] List<Double> value)
        {
            FastFourierTransform fft = new FastFourierTransform();
            FrequencyDomain transform = fft.Transform(value);
            return Ok(transform);
        }

        /// <summary>
        /// Given a frequency domain object, synthesize it and return the time domain
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        [HttpPost("inverse")]
        public ActionResult<List<Double>> InverseFFT([FromBody] FrequencyDomain fd)
        {
            FastFourierTransform transformer = new FastFourierTransform();
            return Ok(transformer.Synthesize(fd));
        }
    }
}
