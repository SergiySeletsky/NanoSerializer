using System;
using System.Collections.Generic;
using System.Reflection;

namespace NanoSerializer.Mappers
{
    internal class EnumMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type.BaseType == typeof(Enum);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var value = Buffer.GetByte(buffer, source.Index);
                source.Index += sizeof(byte);
                setter(item, value);
            };
        }

        public override Func<object, List<byte[]>, int> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                blocks.Add(new byte[1] { (byte)item });
                return sizeof(byte);
            };
        }
    }
}
