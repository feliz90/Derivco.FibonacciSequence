using System.Collections;
using System.Collections.Generic;

namespace Derivco.FibonacciSequence.API.Models.FibonacciSequence
{
    public class FibonacciSequenceResponse
    {
        public IEnumerable<string> FibonacciSequence { get; set; }
        public List<string> ErrorMessage { get; set; }
    }
}