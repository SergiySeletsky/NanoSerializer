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
            return (obj, stream) => {
                Span<byte> span = stackalloc byte[sizeof(int)];
                stream.Read(span);
                var number = BitConverter.ToInt32(span);

                setter(obj, number);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (obj, stream) => {
                var prop = (int)getter(obj);
                ReadOnlySpan<byte> span = BitConverter.GetBytes(prop);
                stream.Write(span);
            };
        }
    }
}
