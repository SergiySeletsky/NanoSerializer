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
            return (obj, stream) => {
                var length = stream.ReadLength();

                Span<byte> span = stackalloc byte[length];

                stream.Read(span);

                var list = Encoding.UTF8.GetString(span).Split('|').ToList();

                setter(obj, list);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (obj, stream) => {
                var prop = getter(obj);

                var list = (List<string>)prop;

                Span<byte> span = new byte[0];
                if (list.Any())
                {
                    var text = list.Aggregate((i, j) => i + "|" + j);
                    span = Encoding.UTF8.GetBytes(text);
                }
                ReadOnlySpan<byte> length = BitConverter.GetBytes((ushort)span.Length);

                stream.Write(length);
                stream.Write(span);
            };
        }
    }
}
