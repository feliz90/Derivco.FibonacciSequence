using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Derivco.FibonacciSequence.Logic.Cache
{
    public interface IMemoryCacheService<TEntry, in TKey>
    {
        Task<TEntry> GetSequence(TKey key);
        void SetSequence(TKey key, TEntry entry);
        void Remove(TKey key);
    }
}