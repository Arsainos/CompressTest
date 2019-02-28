using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Workers
{
    class WorkerProvider : Data.AbstractClasses.AbstractWorkerProvider,Interfaces.IComputation, IDisposable
    {
        public Interfaces.IComputation ComputationStrategy { private get; set; }

        public WorkerProvider(object underlayingStructure) : base(underlayingStructure)
        {
            objectValidation<Interfaces.IComputation>(underlayingStructure);
            ComputationStrategy = (Interfaces.IComputation)underlayingStructure;
        }

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
