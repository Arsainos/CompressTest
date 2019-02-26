using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Workers
{
    class WorkerProvider : Interfaces.IComputation
    {
        public Interfaces.IComputation ComputationStrategy { private get; set; }

        public WorkerProvider(Interfaces.IComputation computation)
        {
            ComputationStrategy = computation;
        }

        public void Start()
        {
            ComputationStrategy.Start();
        }

        public void Stop()
        {
            ComputationStrategy.Stop();
        }
    }
}
