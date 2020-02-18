using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NanoSerializer
{
    internal static class Extensions
    {
        internal const int lengthSize = 2;

        internal static short ReadLength(this Stream stream)
        {
            Span<byte> span = stackalloc byte[lengthSize];

            stream.Read(span);

            return BitConverter.ToInt16(span);
        }
    }
}
