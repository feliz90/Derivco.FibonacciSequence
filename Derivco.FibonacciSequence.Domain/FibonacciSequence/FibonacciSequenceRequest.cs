using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Derivco.FibonacciSequence.API.Models.FibonacciSequence
{
    public class FibonacciSequenceRequest
    {
        /// <summary>
        /// The index of the first number in Fibonacci sequence 
        /// </summary>
        /// <example>1</example>
        [Required]
        public uint FirstIndex { get; set; }
        
        /// <summary>
        /// The index of the last number in Fibonacci sequence
        /// </summary>
        /// <example>10</example>
        [Required] 
        public uint LastIndex { get; set; }
        
        /// <summary>
        /// indicates whether it can use cache or not
        /// </summary>
        /// <example>true</example>
        public bool UseCache { get; set; }
        
        /// <summary>
        /// A time in milliseconds for how long it can run
        /// </summary>
        /// <example>50</example>
        [Required]
        [DefaultValue(int.MaxValue)]
        [Range(1, int.MaxValue)]
        public int Milliseconds { get; set; }
        
        
        /// <summary>
        /// A maximum amount of memory the program can use
        /// </summary>
        /// <example>10000</example>
        [Required]
        [Range(1, int.MaxValue)]
        [DefaultValue(int.MaxValue)]
        public long AmountOfMemory { get; set; }
    }
}