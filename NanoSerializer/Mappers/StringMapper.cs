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
            return (obj, stream) =>
            {
                var length = stream.ReadLength();

                Span<byte> span = stackalloc byte[length];

                stream.Read(span);

                var text = Encoding.UTF8.GetString(span);

                setter(obj, text);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (obj, stream) => {
                var prop = (string)getter(obj);

                ReadOnlySpan<byte> span = Encoding.UTF8.GetBytes(prop);
                ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)span.Length);

                stream.Write(length);
                stream.Write(span);
            };
        }
    }
}
