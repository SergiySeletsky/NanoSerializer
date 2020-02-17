using System;
using System.Collections.Generic;
using System.Text;

namespace NanoSerializer
{
    public abstract class TypeMapper
    {
        protected const int lengthSize = 2;

        public abstract bool Can(Type type);

        public abstract Action<object, byte[]> Get(Mapper source, Action<object, object> setter);

        public abstract Func<object, List<byte[]>, int> Set(Func<object, object> getter);
    }
}
