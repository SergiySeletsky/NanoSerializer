﻿using System;
using System.Collections.Generic;
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
            var typeInfo = type.GetTypeInfo();
            return !typeInfo.IsPrimitive && typeInfo.IsClass && !type.Namespace.StartsWith("System");
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var length = BitConverter.ToInt16(buffer, source.Index);

                source.Index += lengthSize;

                if (length != 0)
                {
                    var data = new byte[length];

                    Buffer.BlockCopy(buffer, source.Index, data, 0, length);

                    source.Index += length;

                    var value = serializer.Deserialize(type, data);

                    setter(item, value);
                }
            };
        }

        public override Action<object, List<byte[]>> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                var bytes = serializer.Serialize(item);
                var length = BitConverter.GetBytes((ushort)bytes.Length);
                blocks.Add(length);
                blocks.Add(bytes);
            };
        }
    }
}
