using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Data.AbstractClasses
{
    abstract class AbstractDataSource : IDisposable
    {
        public AbstractDataSource(object[] payload)
        {

        }

        public virtual void Dispose()
        {
            //
        }

        private void BasicValidation(string[] payload)
        {
            if (payload == null || payload.Length == 0)
            {
                throw new Exception(String.Format("Не переданы данные для инициализации коснструктора в класс - {0}", this.GetType()));
            }
        }

        public virtual void InputDirectionValidation(string[] payload)
        {
            BasicValidation(payload);
        }

        public virtual void OutputDirectionValidation(string[] payload)
        {
            BasicValidation(payload);
        }

        public virtual void InOutDirectionValidation(string[] payload)
        {
            BasicValidation(payload);
        }

    }
}
