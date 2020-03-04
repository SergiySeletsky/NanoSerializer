using System;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class LongMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(long);
        }

        public override void Set(ref NanoReader reader)
        {
            var span = reader.Read(sizeof(long));

            var number = BitConverter.ToInt64(span);

            Setter(reader.Instance, number);
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (long)Getter(obj);
            ReadOnlySpan<byte> span = BitConverter.GetBytes(prop);
            stream.Write(span);
        }
    }
}
