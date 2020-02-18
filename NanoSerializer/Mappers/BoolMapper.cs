using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class BoolMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(bool);
        }

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                Span<byte> span = stackalloc byte[sizeof(bool)];
                stream.Read(span);

                var boolean = BitConverter.ToBoolean(span);

                setter(item, boolean);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                ReadOnlySpan<byte> bytes = BitConverter.GetBytes((bool)item);

                stream.Write(bytes);
            };
        }
    }
}
