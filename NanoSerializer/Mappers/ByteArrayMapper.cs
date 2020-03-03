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

        public override void Set(object obj, Stream stream)
        {
            var length = stream.ReadLength();

            Span<byte> span = stackalloc byte[length];

            stream.Read(span);

            Setter(obj, span.ToArray());
        }

        public override void Get(object obj, Stream stream)
        {
            ReadOnlySpan<byte> span = (byte[])Getter(obj);

            ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)span.Length);

            stream.Write(length);
            stream.Write(span);
        }
    }
}
