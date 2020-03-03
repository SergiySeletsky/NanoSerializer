using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace NanoSerializer
{
    internal static class Extensions
    {
        const int LengthSize = sizeof(ushort);

        internal static ushort ReadLength(this ref NanoReader reader)
        {
            var span = reader.Buffer.Slice(reader.Position, LengthSize);
            reader.Position += LengthSize;

            return BitConverter.ToUInt16(span);
        }

        internal static ReadOnlySpan<byte> Read(this ref NanoReader reader, int size)
        {
            var span = reader.Buffer.Slice(reader.Position, size);
            reader.Position += size;
            return span;
        }
    }
}
