using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NanoSerializer.Mappers
{
    internal class StringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(string);
        }

        public override void Set(object obj, Stream stream)
        {
            var length = stream.ReadLength();

            Span<byte> span = stackalloc byte[length];

            stream.Read(span);

            var text = Encoding.UTF8.GetString(span);

            Setter(obj, text);
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (string)Getter(obj);

            ReadOnlySpan<byte> span = Encoding.UTF8.GetBytes(prop);
            ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)span.Length);

            stream.Write(length);
            stream.Write(span);
        }
    }
}
