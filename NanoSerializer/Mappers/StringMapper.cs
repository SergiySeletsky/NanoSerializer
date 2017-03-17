using System;
using System.Collections.Generic;
using System.Text;

namespace NanoSerializer.Mappers
{
    internal class StringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(string);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) =>
            {
                var length = BitConverter.ToInt16(buffer, source.Index);
                source.Index += lengthSize;

                var data = new byte[length];

                Buffer.BlockCopy(buffer, source.Index, data, 0, length);

                source.Index += length;

                var text = Encoding.UTF8.GetString(data);

                setter(item, text);
            };
        }

        public override Func<object, List<byte[]>, int> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                var text = (string)item;
                var bytes = Encoding.UTF8.GetBytes(text);
                var length = BitConverter.GetBytes((ushort)bytes.Length);

                blocks.Add(length);
                blocks.Add(bytes);

                return lengthSize + bytes.Length;
            };
        }
    }
}
