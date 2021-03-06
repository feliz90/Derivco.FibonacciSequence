using System.Threading;
using System.Threading.Tasks;

namespace Derivco.FibonacciSequence.Logic.MemoryService
{
    public interface IMemoryService
    {
        Task CheckMemoryUsage(long amountOfMemory, CancellationToken token);
    }
}