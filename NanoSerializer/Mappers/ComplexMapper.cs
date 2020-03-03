using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NanoSerializer.Mappers
{
    internal class ComplexMapper : TypeMapper
    {
        private Serializer serializer;

        public void Use(Serializer serializer)
        {
            this.serializer = serializer;
        }

        public override bool Can(Type type)
        {
            return !type.IsPrimitive && type.IsClass && !type.Namespace.StartsWith("System");
        }

        public override void Set(ref NanoReader reader)
        {
            if (reader.Position == reader.Buffer.Length)
            {
                return;
            }

            var length = reader.ReadLength();

            if (length != 0)
            {
                var span = reader.Read(length);

                var innerInstance = Activator.CreateInstance(reader.Instance.GetType());
                serializer.Deserialize(innerInstance, span);
                Setter(reader.Instance, innerInstance);
            }
        }

        public override void Get(object obj, Stream stream)
        {
            var prop = Getter(obj);

            if (prop != null)
            {
                using (var innerStream = new MemoryStream())
                {
                    serializer.Serialize(prop, innerStream);

                    ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)innerStream.Length);

                    stream.Write(length);

                    innerStream.CopyTo(stream);
                }
            }
        }
    }
}
