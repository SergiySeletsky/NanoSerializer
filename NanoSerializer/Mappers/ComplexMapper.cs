using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

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

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) => {

                var length = await stream.ReadLengthAsync();

                if (length != 0)
                {
                    var instance = Activator.CreateInstance(type);
                    await serializer.DeserializeAsync(instance, type, stream);
                    setter(item, instance);
                }
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);

                if (item != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await serializer.SerializeAsync(item, ms);

                        ReadOnlyMemory<byte> length = BitConverter.GetBytes((ushort)ms.Length);

                        await stream.WriteAsync(length);

                        ms.Position = 0;
                        await ms.CopyToAsync(stream);
                    }
                }
            };
        }
    }
}
