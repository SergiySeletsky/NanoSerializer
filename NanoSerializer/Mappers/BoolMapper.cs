using System;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class BoolMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(bool);
        }

        public override void Set(ref NanoReader reader)
        {
            var span = reader.Read(sizeof(bool));

            var boolean = BitConverter.ToBoolean(span);

            Setter(reader.Instance, boolean);
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (bool)Getter(obj);
            ReadOnlySpan<byte> span = BitConverter.GetBytes(prop);
            stream.Write(span);
        }
    }
}
