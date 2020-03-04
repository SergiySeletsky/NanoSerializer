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

        public override void Set(ref NanoReader reader)
        {
            var length = reader.ReadLength();

            var span = reader.Read(length);

            var text = span.ToText();

            Setter(reader.Instance, text);
        }

        public override void Get(object obj, Stream stream)
        {
            ReadOnlySpan<char> chars = (string)Getter(obj);

            Span<byte> span = chars.ToBytes();

            ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)span.Length);

            stream.Write(length);
            stream.Write(span);
        }
    }
}
