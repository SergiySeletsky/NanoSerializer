using System;

namespace NanoSerializer
{
    public ref struct NanoReader
    {
        public object Instance;

        public int Position;

        public Span<byte> Data;
    }
}
