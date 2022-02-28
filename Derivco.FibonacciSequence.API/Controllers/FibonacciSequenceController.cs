using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Derivco.FibonacciSequence.API.Filters;
using Derivco.FibonacciSequence.API.Models.FibonacciSequence;
using Derivco.FibonacciSequence.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Derivco.FibonacciSequence.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FibonacciSequenceController : ControllerBase
    {
        private readonly IFibonacciSequenceService _sequenceLogic;

        public FibonacciSequenceController(IFibonacciSequenceService sequenceLogic)
        {
            _sequenceLogic = sequenceLogic;
        }

        /// <summary>
        /// Subsequence from the sequence of Fibonacci numbers that is matching the input indexes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ServiceFilter(typeof(RequestValidationFilterAttribute))]
        public async Task<IActionResult> Get([FromQuery]FibonacciSequenceRequest request)
        {
            return Ok(await _sequenceLogic.GetFibonacciSubsequence(request.FirstIndex, request.LastIndex, request.UseCache, request.Milliseconds, request.AmountOfMemory));
        }
    }
}
