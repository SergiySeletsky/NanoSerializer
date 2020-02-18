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

        public override Action<object, Stream> Get(Action<object, object> setter)
        {
            return (obj, stream) => {

                var length = stream.ReadLength();

                if (length != 0)
                {
                    var instance = Activator.CreateInstance(type);
                    serializer.Deserialize(instance, type, stream);
                    setter(obj, instance);
                }
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (obj, stream) => {
                var prop = getter(obj);

                if (prop != null)
                {
                    using (var innerStream = new MemoryStream())
                    {
                        serializer.Serialize(prop, innerStream);

                        ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)innerStream.Length);

                        stream.Write(length);

                        innerStream.Position = 0;
                        innerStream.CopyTo(stream);
                    }
                }
            };
        }
    }
}
