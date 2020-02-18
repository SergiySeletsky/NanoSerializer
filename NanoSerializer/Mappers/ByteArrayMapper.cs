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

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                var length = stream.ReadLength();

                Span<byte> data = stackalloc byte[length];

                stream.Read(data);

                setter(item, data.ToArray());
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                ReadOnlySpan<byte> bytes = (byte[])item;
                ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)bytes.Length);

                stream.Write(length);
                stream.Write(bytes);
            };
        }
    }
}
