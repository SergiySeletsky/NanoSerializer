using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class IntMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(int);
        }

        public override void Set(object obj, Stream stream)
        {
            Span<byte> span = stackalloc byte[sizeof(int)];
            stream.Read(span);
            var number = BitConverter.ToInt32(span);

            Setter(obj, number);
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (int)Getter(obj);
            ReadOnlySpan<byte> span = BitConverter.GetBytes(prop);
            stream.Write(span);
        }
    }
}
