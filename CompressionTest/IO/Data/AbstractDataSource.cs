using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompressionTest.IO.Data
{
    abstract class AbstractDataSource : IDisposable
    {
        public AbstractDataSource(string[] payload)
        {

        }

        public virtual void Dispose()
        {
            //
        }

        public virtual void InputValidation(string[] payload)
        {
            if(payload==null || payload.Length==0)
            {
                throw new Exception(String.Format("Не переданы данные для инициализации коснструктора в класс - {0}",this.GetType()));
            }
        }

    }
}
