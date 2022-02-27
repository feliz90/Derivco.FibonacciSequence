using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Derivco.FibonacciSequence.API.Models.FibonacciSequence;
using Derivco.FibonacciSequence.Logic.Cache;
using Derivco.FibonacciSequence.Logic.MemoryService;

namespace Derivco.FibonacciSequence.Logic
{
    public class FibonacciSequenceService : IFibonacciSequenceService
    {
        private const int COUNT_OF_THREADS = 2;
        private readonly IFibonacciSequenceMemoryCacheService _cacheService;
        private readonly IMemoryService _memoryService;

        public FibonacciSequenceService(IFibonacciSequenceMemoryCacheService cacheService, IMemoryService memoryService)
        {
            _cacheService = cacheService;
            _memoryService = memoryService;
        }
        
        public async Task<FibonacciSequenceResponse> GetFibonacciSubsequence(uint firstIndex, uint lastIndex, bool useCache, int milliseconds, long amountOfMemory)
        {
            var response = new FibonacciSequenceResponse();

            using var cancellationTokenSource = new CancellationTokenSource();
            
            var timeoutTask = Task.Delay(milliseconds, cancellationTokenSource.Token);
            var checkMemoryTask = Task.Run(() => _memoryService.CheckMemoryUsage(amountOfMemory, cancellationTokenSource.Token), cancellationTokenSource.Token);
            
            await Task.Delay(1, cancellationTokenSource.Token);
            
            var generateSequenceTask = Task.Run(() => GenerateSequence(firstIndex, lastIndex, useCache, cancellationTokenSource.Token), cancellationTokenSource.Token);

            var listOfTasks = new List<Task>
            {
                generateSequenceTask,
                timeoutTask,
                checkMemoryTask
            };


            try
            {
                await Task.WhenAny(listOfTasks);
                cancellationTokenSource.Cancel();
                await Task.WhenAll(listOfTasks);
            }
            catch (Exception ex) when(ex is TaskCanceledException or OperationCanceledException)
            {
                
            }
            
            if (timeoutTask.IsCompletedSuccessfully)
            {
                response.ErrorMessage.Add("Timeout error");
            }
            if (checkMemoryTask.IsCompletedSuccessfully)
            {
                response.ErrorMessage.Add("Amount of memory limit reached");
            }
            if (generateSequenceTask.IsCompletedSuccessfully)
            {
                response.FibonacciSequence = generateSequenceTask.Result;
            }

            return response;
        }
        
        private async Task<List<string>> GenerateSequence(uint firstIndex, uint lastIndex, bool useCache, CancellationToken token)
        {
            var fibonacciSequence = new List<string>();

            for (var i = firstIndex; i <= lastIndex && !token.IsCancellationRequested;  i += COUNT_OF_THREADS)
            {
                var tasksList = new List<Task<string>>();
                
                for (var j = i; j < i + COUNT_OF_THREADS; j++)
                {
                    if (j > lastIndex) continue;
                    
                    var index = j;
                    tasksList.Add(Task.Run(() => GetFibonacciNumber(index, useCache, token), token));
                }

                try
                {
                    await Task.WhenAll(tasksList);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                
                tasksList.ForEach(t => fibonacciSequence.Add(t.Result));
                
            }
            
            return fibonacciSequence;
        }

        private async Task<string> GetFibonacciNumber(long index, bool useCache, CancellationToken token)
        {
            if (useCache)
            {
                var cachedFibonacciNumber = await _cacheService.GetSequence(index);
                if (!string.IsNullOrEmpty(cachedFibonacciNumber)) return cachedFibonacciNumber;
            }
            
            var fibonacciNumber = CalculateFibonacciNumber(index, token).ToString();
            
            _cacheService.SetSequence(index, fibonacciNumber);
            
            return fibonacciNumber;
        }

        private static BigInteger CalculateFibonacciNumber(long index, CancellationToken token)
        {
            if (index <= 1)
            {
                return index;
            }
            
            token.ThrowIfCancellationRequested();
            
            return CalculateFibonacciNumber(index - 1, token) + CalculateFibonacciNumber(index - 2, token);
        }
    }
}