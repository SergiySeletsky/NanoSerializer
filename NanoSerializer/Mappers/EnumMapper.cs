using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using static NanoSerializer.Serializer;

namespace NanoSerializer.Mappers
{
    internal class EnumMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type.GetTypeInfo().BaseType == typeof(Enum);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var value = Buffer.GetByte(buffer, source.Index);
                Interlocked.Add(ref source.Index, sizeof(byte));
                setter(item, value);
            };
        }

        public override Action<object, List<byte[]>> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                blocks.Add(new byte[1] { (byte)item });
            };
        }
    }
}
