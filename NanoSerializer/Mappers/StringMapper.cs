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

            var text = Encoding.UTF8.GetString(span);

            Setter(reader.Instance, text);
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
