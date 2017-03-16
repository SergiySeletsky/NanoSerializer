using System;
using System.Collections.Generic;
using System.Threading;
using static NanoSerializer.Serializer;

namespace NanoSerializer.Mappers
{
    internal class BoolMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(bool);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var boolean = BitConverter.ToBoolean(buffer, source.Index);
                Interlocked.Add(ref source.Index, sizeof(bool));
                setter(item, boolean);
            };
        }

        public override Action<object, List<byte[]>> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                var bytes = BitConverter.GetBytes((bool)item);
                blocks.Add(bytes);
            };
        }
    }
}
