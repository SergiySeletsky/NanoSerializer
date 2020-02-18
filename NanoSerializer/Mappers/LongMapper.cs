using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class LongMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(long);
        }

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                Span<byte> span = stackalloc byte[sizeof(long)];
                stream.Read(span);
                var number = BitConverter.ToInt64(span);

                setter(item, number);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                ReadOnlySpan<byte> span = BitConverter.GetBytes((long)item);
                stream.Write(span);
            };
        }
    }
}
