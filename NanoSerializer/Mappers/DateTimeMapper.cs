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
            return (obj, stream) => {
                Span<byte> span = new byte[sizeof(long)];
                stream.Read(span);

                var ticks = BitConverter.ToInt64(span);

                setter(obj, new DateTime(ticks));
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (obj, stream) => {
                var prop = (DateTime)getter(obj);
                ReadOnlySpan<byte> span = BitConverter.GetBytes(prop.Ticks);
                stream.Write(span);
            };
        }
    }
}
