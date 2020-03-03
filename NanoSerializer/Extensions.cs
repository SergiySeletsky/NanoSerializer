using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NanoSerializer
{
    internal static class Extensions
    {
        internal static short ReadLength(this ref NanoReader reader)
        {
            var span = reader.Buffer.Slice(reader.Position, sizeof(short));
            reader.Position += sizeof(short);

            return BitConverter.ToInt16(span);
        }

        internal static ReadOnlySpan<byte> Read(this ref NanoReader reader, int size)
        {
            var span = reader.Buffer.Slice(reader.Position, size);
            reader.Position += size;
            return span;
        }
    }
}
