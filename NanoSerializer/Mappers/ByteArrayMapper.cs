using System;
using System.Collections.Generic;
using System.Threading;

namespace NanoSerializer.Mappers
{
    internal class ByteArrayMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(byte[]);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var length = BitConverter.ToInt16(buffer, source.Index);

                source.Index += lengthSize;

                var data = new byte[length];

                Buffer.BlockCopy(buffer, source.Index, data, 0, length);

                source.Index += length;

                setter(item, data);
            };
        }

        public override Action<object, List<byte[]>> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                var bytes = (byte[])item;
                var length = BitConverter.GetBytes((ushort)bytes.Length);

                blocks.Add(length);
                blocks.Add(bytes);
            };
        }
    }
}
