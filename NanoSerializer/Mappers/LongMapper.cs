using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class LongMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(long);
        }

        public override void Set(object obj, Stream stream)
        {
            Span<byte> span = stackalloc byte[sizeof(long)];
            stream.Read(span);
            var number = BitConverter.ToInt64(span);

            Setter(obj, number);
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (long)Getter(obj);
            ReadOnlySpan<byte> span = BitConverter.GetBytes(prop);
            stream.Write(span);
        }
    }
}
