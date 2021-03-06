using System;
using System.Threading;
using System.Threading.Tasks;

namespace Derivco.FibonacciSequence.Logic.MemoryService
{
    public class MemoryService : IMemoryService
    {
        public async Task CheckMemoryUsage(long amountOfMemory, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                await Task.Delay(1, token);
                
                if (GC.GetTotalAllocatedBytes() >= amountOfMemory)
                   return;
            }
        }
    }
}