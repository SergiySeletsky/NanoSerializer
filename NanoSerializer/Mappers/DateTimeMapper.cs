using System;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class DateTimeMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(DateTime);
        }

        public override void Set(ref NanoReader reader)
        {
            var span = reader.Read(sizeof(long));

            var ticks = BitConverter.ToInt64(span);

            Setter(reader.Instance, new DateTime(ticks));
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (DateTime)Getter(obj);
            ReadOnlySpan<byte> span = BitConverter.GetBytes(prop.Ticks);

            stream.Write(span);
        }
    }
}
