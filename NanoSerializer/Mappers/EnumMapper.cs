using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NanoSerializer.Mappers
{
    internal class EnumMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type.BaseType == typeof(Enum);
        }

        public override Action<object, Stream> Get(Action<object, object> setter)
        {
            return (obj, stream) => {
                var value = (byte)stream.ReadByte();
                setter(obj, value);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (obj, stream) => {
                var prop = (byte)getter(obj);
                stream.WriteByte(prop);
            };
        }
    }
}
