using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NanoSerializer.Mappers
{
    internal class ListStringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(List<string>);
        }

        public override void Set(ref NanoReader reader)
        {
            var length = reader.ReadLength();

            var span = reader.Read(length);

            var list = Encoding.UTF8.GetString(span).Split('|').ToList();

            Setter(reader.Instance, list);
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (List<string>)Getter(obj);

            Span<byte> span = new byte[0];
            if (prop.Any())
            {
                var text = prop.Aggregate((i, j) => i + "|" + j);
                span = Encoding.UTF8.GetBytes(text);
            }
            ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)span.Length);

            stream.Write(length);
            stream.Write(span);
        }
    }
}
