using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class DateTimeMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(DateTime);
        }

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                Span<byte> span = new byte[sizeof(long)];
                stream.Read(span);

                var ticks = BitConverter.ToInt64(span);

                setter(item, new DateTime(ticks));
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                var dateTime = (DateTime)item;
                ReadOnlySpan<byte> span = BitConverter.GetBytes(dateTime.Ticks);
                stream.Write(span);
            };
        }
    }
}
