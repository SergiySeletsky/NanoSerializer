using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class BoolMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(bool);
        }

        public override void Set(object obj, Stream stream)
        {
            Span<byte> span = stackalloc byte[sizeof(bool)];
            stream.Read(span);

            var boolean = BitConverter.ToBoolean(span);

            Setter(obj, boolean);
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (bool)Getter(obj);
            ReadOnlySpan<byte> span = BitConverter.GetBytes(prop);
            stream.Write(span);
        }
    }
}
