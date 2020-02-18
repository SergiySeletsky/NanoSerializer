using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NanoSerializer.Mappers
{
    internal class ListStringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(List<string>);
        }

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                var length = stream.ReadLength();

                Span<byte> span = stackalloc byte[length];

                stream.Read(span);

                var list = Encoding.UTF8.GetString(span).Split('|').ToList();

                setter(item, list);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);

                var list = (List<string>)item;

                Span<byte> bytes = new byte[0];
                if (list.Any())
                {
                    var text = list.Aggregate((i, j) => i + "|" + j);
                    bytes = Encoding.UTF8.GetBytes(text);
                }
                ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)bytes.Length);

                stream.Write(length);
                stream.Write(bytes);
            };
        }
    }
}
