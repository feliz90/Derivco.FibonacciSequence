using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Derivco.FibonacciSequence.API.Models.FibonacciSequence;

namespace Derivco.FibonacciSequence.Logic
{
    public interface IFibonacciSequenceService
    { 
        Task<FibonacciSequenceResponse> GetFibonacciSubsequence(uint firstIndex, uint lastIndex, bool useCache, int milliseconds, long amountOfMemory);
    }
}