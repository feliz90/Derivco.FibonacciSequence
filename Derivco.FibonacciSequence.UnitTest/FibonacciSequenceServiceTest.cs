using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Derivco.FibonacciSequence.API.Models.FibonacciSequence;
using Derivco.FibonacciSequence.Logic;
using Derivco.FibonacciSequence.Logic.Cache;
using Derivco.FibonacciSequence.Logic.MemoryService;
using FluentAssertions;
using Moq;
using Xunit;

namespace Derivco.FibonacciSequence.UnitTest
{
    public class FibonacciSequenceServiceTest
    {
        private readonly Mock<IMemoryService> _mockMemoryService = new ();
        private readonly Mock<IFibonacciSequenceMemoryCacheService> _mockMemoryCacheService = new() { CallBase = true};

        [Theory]
        [MemberData(nameof(FibonacciSubsequenceServiceExpectedResults.ExpectedSequences), MemberType = typeof(FibonacciSubsequenceServiceExpectedResults))]
        public async Task GetFibonacciSequence_WithCorrectRequest_ReturnsExpectedSequence(uint firstIndex, uint lastIndex, string[] expectedResult)
        {
            _mockMemoryService.Setup(s => s.CheckMemoryUsage(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(async (long _, CancellationToken c) =>
                {
                    while (true)
                    {
                        c.ThrowIfCancellationRequested();
                        await Task.Delay(0, c);
                    }
                });

            _mockMemoryCacheService.Setup(s => s.GetSequence(It.IsAny<uint>()))
                .Returns((string _) => Task.FromResult(It.IsAny<string>()));

            _mockMemoryCacheService.Setup(s => s.SetSequence(It.IsAny<uint>(), It.IsAny<string>()));
            
            var service =
                new FibonacciSequenceService(_mockMemoryCacheService.Object, _mockMemoryService.Object);


            var result =
                await service.GetFibonacciSubsequence(firstIndex, lastIndex, false, 10000, 10000);
            
            result.Should().NotBeNull();
            //result.Should().BeOfType<FibonacciSequenceResponse>().Which.ErrorMessage.Should().BeNull();
            result.Should().BeOfType<FibonacciSequenceResponse>().Which.FibonacciSequence.Should().BeEquivalentTo(expectedResult);
        }
        
        [Fact]
        public async Task GetFibonacciSequence_WithMaxAmountOfMemory_ReturnsLimitMemoryMessage()
        {
            var request = new FibonacciSequenceRequest()
            {
                FirstIndex = 0,
                LastIndex = 100,
                Milliseconds = int.MaxValue,
            };

            _mockMemoryService.Setup(s => s.CheckMemoryUsage(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(async (long _, CancellationToken c) =>
                {
                    await Task.Delay(0, c);
                });

            _mockMemoryCacheService.Setup(s => s.GetSequence(It.IsAny<uint>()))
                .Returns((string _) => Task.FromResult(It.IsAny<string>()));

            _mockMemoryCacheService.Setup(s => s.SetSequence(It.IsAny<uint>(), It.IsAny<string>()));

            var service =
                new FibonacciSequenceService(_mockMemoryCacheService.Object, _mockMemoryService.Object);
            
            
            var result = 
                await service.GetFibonacciSubsequence(request.FirstIndex, request.LastIndex, request.UseCache, request.Milliseconds, request.AmountOfMemory);
            
            result.Should().BeOfType<FibonacciSequenceResponse>().Which.ErrorMessage.Should().BeEquivalentTo("Amount of memory limit reached");
        }

        [Fact]
        public async Task GetFibonacciSequence_WithTimeout_ReturnsTimeoutMessage()
        {
            
            var request = new FibonacciSequenceRequest()
            {
                FirstIndex = 0,
                LastIndex = 10,
                Milliseconds = 1,
            };
            
            _mockMemoryService.Setup(s => s.CheckMemoryUsage(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(async (long _, CancellationToken c) =>
                {
                    while (true)
                    {
                        c.ThrowIfCancellationRequested();
                        await Task.Delay(0, c);
                    }
                });

            _mockMemoryCacheService.Setup(s => s.GetSequence(It.IsAny<uint>()))
                .Returns((string _) => Task.FromResult(It.IsAny<string>()));

            _mockMemoryCacheService.Setup(s => s.SetSequence(It.IsAny<uint>(), It.IsAny<string>()));
            
            var service =
                new FibonacciSequenceService(_mockMemoryCacheService.Object, _mockMemoryService.Object);
            
            
            var result = 
                await service.GetFibonacciSubsequence(request.FirstIndex, request.LastIndex, request.UseCache, request.Milliseconds, long.MaxValue);
            
            result.Should().BeOfType<FibonacciSequenceResponse>().Which.ErrorMessage.Should().BeEquivalentTo("Timeout error");
        }
        
        private static class FibonacciSubsequenceServiceExpectedResults
        {
            public static List<object[]> ExpectedSequences => new()
            {
                new object[] { 0, 6, new[] { "0", "1", "1", "2", "3", "5", "8" } },
                new object[] { 5, 10, new[] { "5", "8", "13", "21", "34", "55" } }
            };
            
        }
    }
    
}

