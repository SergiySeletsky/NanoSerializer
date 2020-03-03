using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NanoSerializer.Mappers
{
    internal class EnumMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type.BaseType == typeof(Enum);
        }

        public override void Set(ref NanoReader reader)
        {
            var value = reader.Read(sizeof(byte))[0];

            Setter(reader.Instance, value);
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = (byte)Getter(obj);
            stream.WriteByte(prop);
        }
    }
}
