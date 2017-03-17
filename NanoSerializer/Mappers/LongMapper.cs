using System;
using System.Collections.Generic;

namespace NanoSerializer.Mappers
{
    internal class LongMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(long);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var number = BitConverter.ToInt64(buffer, source.Index);
                source.Index += sizeof(long);
                setter(item, number);
            };
        }

        public override Func<object, List<byte[]>, int> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                var bytes = BitConverter.GetBytes((long)item);
                blocks.Add(bytes);
                return sizeof(long);
            };
        }
    }
}
