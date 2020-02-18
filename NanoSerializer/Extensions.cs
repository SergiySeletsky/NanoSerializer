using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NanoSerializer
{
    internal static class Extensions
    {
        internal static short ReadLength(this Stream stream)
        {
            Span<byte> span = stackalloc byte[sizeof(short)];

            stream.Read(span);

            return BitConverter.ToInt16(span);
        }
    }
}
