using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class ByteArrayMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(byte[]);
        }

        public override Action<object, Stream> Get(Action<object, object> setter)
        {
            return (obj, stream) => {
                var length = stream.ReadLength();

                Span<byte> span = stackalloc byte[length];

                stream.Read(span);

                setter(obj, span.ToArray());
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (obj, stream) => {
                ReadOnlySpan<byte> span = (byte[])getter(obj);

                ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)span.Length);

                stream.Write(length);
                stream.Write(span);
            };
        }
    }
}
