using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Data.AbstractClasses
{
    abstract class AbstractDataProvider
    {
        protected object _underlayingStructure;
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

        void innerValidation(object underlayingStructure)
        {
            if(!LookForSpecificInterface(underlayingStructure,"IDisposable"))
                throw new Exception(String.Format("Нижележащая структруа - {0}, не реализует интерфейс IDisposable",
                        underlayingStructure.GetType()));
        }


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
