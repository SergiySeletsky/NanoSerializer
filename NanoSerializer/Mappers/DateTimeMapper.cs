using System;
using System.Collections.Generic;
using System.Threading;
using static NanoSerializer.Serializer;

namespace NanoSerializer.Mappers
{
    internal class DateTimeMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(DateTime);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var ticks = BitConverter.ToInt64(buffer, source.Index);
                source.Index += sizeof(long);
                setter(item, new DateTime(ticks));
            };
        }

        public override Action<object, List<byte[]>> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                var dateTime = (DateTime)item;
                var bytes = BitConverter.GetBytes(dateTime.Ticks);
                blocks.Add(bytes);
            };
        }
    }
}
