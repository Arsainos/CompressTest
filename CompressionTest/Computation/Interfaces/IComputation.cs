using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Interfaces
{
    /// <summary>
    /// Basic interface for workers.
    /// </summary>
    /// <remarks>
    /// Use this interface for strategy for workers implementations.
    /// Main implementation will be using Woekres.Workers provider for high level implementation.
    /// </remarks>
    interface IComputation
    {
        /// <summary>
        /// Start computation
        /// </summary>
        void Start();
        /// <summary>
        /// Stop computation
        /// </summary>
        void Stop();
    }
}
