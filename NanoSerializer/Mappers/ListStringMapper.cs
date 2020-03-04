using System;
using System.Collections.Generic;
using System.IO;
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

            var list = new List<string>();
            var position = 0;
            while(position < span.Length)
            {
                var spanLength = span.Slice(position, sizeof(ushort));
                var segmentLength = BitConverter.ToUInt16(spanLength);

                position += sizeof(ushort);

                var bytes = span.Slice(position, segmentLength);

                position += segmentLength;

                var text = bytes.ToText();

                list.Add(text);
            }

            Setter(reader.Instance, list);
        }

        public override void Get(object obj, Stream stream)
        {
            var list = (List<string>)Getter(obj);

            var bufferSize = 0;
            foreach(ReadOnlySpan<char> item in list)
            {
                bufferSize += sizeof(ushort);
                bufferSize += Encoding.UTF8.GetByteCount(item);
            }

            Span<byte> bufferSpan = stackalloc byte[bufferSize];

            int position = 0;
            foreach (ReadOnlySpan<char> item in list)
            {
                ReadOnlySpan<byte> bytes = item.ToBytes();

                ReadOnlySpan<byte> segmentLength = BitConverter.GetBytes((ushort)bytes.Length);
                Span<byte> spanLength = bufferSpan.Slice(position, sizeof(ushort));

                position += sizeof(ushort);

                segmentLength.CopyTo(spanLength);

                Span<byte> span = bufferSpan.Slice(position, bytes.Length);

                position += bytes.Length;

                bytes.CopyTo(span);
            }

            ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)bufferSize);

            stream.Write(length);
            stream.Write(bufferSpan);
        }
    }
}
