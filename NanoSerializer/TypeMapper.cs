using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NanoSerializer
{
    public abstract class TypeMapper
    {
        public abstract bool Can(Type type);

        public abstract Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter);

        public abstract Func<object, Stream, Task> Set(Func<object, object> getter);
    }
}
