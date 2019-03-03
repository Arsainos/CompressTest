using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Data.AbstractClasses
{
    /// <summary>
    /// Abstract class for data processing
    /// </summary>
    /// <remarks>
    /// Get the underlaying structure and check it for specific interface
    /// </remarks>
    abstract class AbstractDataProvider
    {
        /// <summary>
        /// Holder for structure
        /// </summary>
        protected object _underlayingStructure;
        /// <summary>
        /// Disposable interface to make call a dispose and using in all implementations
        /// </summary>
        private IDisposable disposable { get; set; }

        public AbstractDataProvider(object underlayingStructure)
        {
            innerValidation(underlayingStructure);
            _underlayingStructure = underlayingStructure;
            disposable = (IDisposable)underlayingStructure;
        }

        public virtual void Dispose()
        {
            disposable.Dispose();
        }

        /// <summary>
        /// Basic validation of underlaying structure for iDisposable intreface
        /// </summary>
        /// <param name="underlayingStructure">incoming structure</param>
        void innerValidation(object underlayingStructure)
        {
            if(!LookForSpecificInterface(underlayingStructure,"IDisposable"))
                throw new Exception(String.Format("Underlaying structure - {0}, don't have a IDisposable interface",
                        underlayingStructure.GetType()));
        }

        /// <summary>
        /// Validate object for specific interface 
        /// </summary>
        /// <typeparam name="T">Interface</typeparam>
        /// <param name="uderlayingStructure">Incoming structure</param>
        public virtual void objectValidation<T>(object uderlayingStructure)
        {
            if(!LookForSpecificInterface(uderlayingStructure,typeof(T).ToString()))
                throw new Exception(String.Format("Неккоректный тип данных в - {0}", this.GetType()));
        }

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
