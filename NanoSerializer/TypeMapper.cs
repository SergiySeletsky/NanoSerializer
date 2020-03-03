using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NanoSerializer
{
    public abstract class TypeMapper
    {
        protected Action<object, object> Setter { get; private set; }
        protected Func<object, object> Getter { get; private set; }

        public void Init(Func<object, object> getter, Action<object, object> setter)
        {
            Getter = getter;
            Setter = setter;
        }

        public abstract bool Can(Type type);

        public abstract void Get(object obj, Stream stream);

        public abstract void Set(object obj, Stream stream);
    }
}
