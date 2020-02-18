using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NanoSerializer
{
    internal static class Extensions
    {
        internal const int lengthSize = 2;

        internal static async Task<short> ReadLengthAsync(this Stream stream)
        {
            Memory<byte> memory = new byte[lengthSize];

            await stream.ReadAsync(memory);

            return BitConverter.ToInt16(memory.Span);
        }
    }
}
