using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NanoSerializer.Mappers
{
    internal class ComplexMapper : TypeMapper
    {
        private Serializer serializer;
        private Type type;

        public void Use(Serializer serializer)
        {
            this.serializer = serializer;
        }

        public override bool Can(Type type)
        {
            this.type = type;
            return !type.IsPrimitive && type.IsClass && !type.Namespace.StartsWith("System");
        }

        public override Action<object, MemoryStream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {

                var buffer = new byte[lengthSize];
                stream.Read(buffer, 0, lengthSize);
                var length = BitConverter.ToInt16(buffer, 0);

                if (length != 0)
                {
                    var data = new byte[length];

                    stream.Read(data, 0, length);

                    var value = serializer.Deserialize(type, data);

                    setter(item, value);
                }
            };
        }

        public override Action<object, MemoryStream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                var bytes = serializer.Serialize(item);
                var length = BitConverter.GetBytes((ushort)bytes.Length);

                stream.Write(length, 0, length.Length);
                stream.Write(bytes, 0, bytes.Length);
            };
        }
    }
}
