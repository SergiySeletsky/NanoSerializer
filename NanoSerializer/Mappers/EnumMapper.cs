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

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                var value = (byte)stream.ReadByte();
                setter(item, value);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                stream.WriteByte((byte)item);
            };
        }
    }
}
