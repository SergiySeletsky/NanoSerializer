using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class IntMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(int);
        }

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                Span<byte> span = stackalloc byte[sizeof(int)];
                stream.Read(span);
                var number = BitConverter.ToInt32(span);

                setter(item, number);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                ReadOnlySpan<byte> span = BitConverter.GetBytes((int)item);
                stream.Write(span);
            };
        }
    }
}
