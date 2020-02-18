using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NanoSerializer.Mappers
{
    internal class StringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(string);
        }

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) =>
            {
                var length = stream.ReadLength();

                Span<byte> data = stackalloc byte[length];

                stream.Read(data);

                var text = Encoding.UTF8.GetString(data);

                setter(item, text);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                var text = (string)item;
                ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(text);
                ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)bytes.Length);

                stream.Write(length);
                stream.Write(bytes);
            };
        }
    }
}
