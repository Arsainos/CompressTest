using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Workers
{
    /// <summary>
    /// High level worker provider.
    /// </summary>
    class WorkerProvider : Data.AbstractClasses.AbstractWorkerProvider,Interfaces.IComputation, IDisposable
    {
        /// <summary>
        /// Strategy
        /// </summary>
        public Interfaces.IComputation ComputationStrategy { private get; set; }

        public WorkerProvider(object underlayingStructure) : base(underlayingStructure)
        {
            objectValidation<Interfaces.IComputation>(underlayingStructure);
            ComputationStrategy = (Interfaces.IComputation)underlayingStructure;
        }
        /// <summary>
        /// Validation of input types and their interfaces implementations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uderlayingStructure"></param>
        public override void objectValidation<T>(object uderlayingStructure)
        {
            base.objectValidation<T>(uderlayingStructure);
        }

        public void Start()
        {
            ComputationStrategy.Start();
        }

        public void Stop()
        {
            ComputationStrategy.Stop();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
