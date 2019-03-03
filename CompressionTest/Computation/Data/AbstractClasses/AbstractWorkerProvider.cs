using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.Computation.Data.AbstractClasses
{
    /// <summary>
    /// Abstract class for all workers
    /// </summary>
    /// <remarks>
    /// This class is made for using as a base for any realization of worker provider.
    /// </remarks>
    abstract class AbstractWorkerProvider : IDisposable
    {
        /// <summary>
        /// underlaying structre for provider
        /// </summary>
        protected object _underlayingStructure;
        /// <summary>
        /// disposable interface of undelaying structure
        /// </summary>
        private IDisposable disposable { get; set; }

        public AbstractWorkerProvider(object underlayingStructure)
        {
            innerValidation(underlayingStructure);
            _underlayingStructure = underlayingStructure;
            disposable = (IDisposable)underlayingStructure;
        }

        /// <summary>
        /// Get rid of nasty links
        /// </summary>
        public virtual void Dispose()
        {
            disposable.Dispose();
            _underlayingStructure = null;
        }

        /// <summary>
        /// Some basic validation for providers.
        /// </summary>
        /// <param name="underlayingStructure">Underlaying structure realization</param>
        void innerValidation(object underlayingStructure)
        {
            if (!LookForSpecificInterface(underlayingStructure, "IDisposable"))
                throw new Exception(String.Format("Underlaying structure - {0}, don't have interface - IDisposable",
                        underlayingStructure.GetType()));
        }

        /// <summary>
        /// Validation for specific type
        /// </summary>
        /// <typeparam name="T">We need check for specific type in underlaying structure</typeparam>
        /// <param name="uderlayingStructure"></param>
        public virtual void objectValidation<T>(object uderlayingStructure)
        {
            if (!LookForSpecificInterface(uderlayingStructure, typeof(T).ToString()))
                throw new Exception(String.Format("Incorect type of data - {0}", this.GetType()));
        }

        /// <summary>
        /// Lookup for interfaces in structure
        /// </summary>
        /// <param name="underlayingStructure"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        bool LookForSpecificInterface(object underlayingStructure, string search)
        {
            bool contains = false;
            foreach (Type type in underlayingStructure.GetType().GetInterfaces())
            {
                if (type.ToString().Contains(search))
                {
                    contains = true;
                }
            }

            return contains;
        }
    }
}
