using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NanoSerializer
{
    public abstract class TypeMapper
    {
        protected const int lengthSize = 2;

        public abstract bool Can(Type type);

        public abstract Action<object, Stream> Get(Mapper source, Action<object, object> setter);

        public abstract Action<object, Stream> Set(Func<object, object> getter);
    }
}
