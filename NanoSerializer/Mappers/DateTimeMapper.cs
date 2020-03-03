using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class DateTimeMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(DateTime);
        }

        public override void Set(object obj, Stream stream)
        {
            Span<byte> span = new byte[sizeof(long)];
            stream.Read(span);

            var ticks = BitConverter.ToInt64(span);

            Setter(obj, new DateTime(ticks));
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (DateTime)Getter(obj);
            ReadOnlySpan<byte> span = BitConverter.GetBytes(prop.Ticks);
            stream.Write(span);
        }
    }
}
