using System.Numerics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Derivco.FibonacciSequence.Logic.Cache
{
    public class FibonacciSequenceMemoryCacheService : MemoryCacheService<string, uint>, IFibonacciSequenceMemoryCacheService
    {
        public FibonacciSequenceMemoryCacheService(IMemoryCache memoryCache, IConfiguration configuration) : base(memoryCache, configuration)
        {
            
        }
    }
}