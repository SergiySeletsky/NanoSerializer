using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoSerializer.Mappers
{
    internal class ListStringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(List<string>);
        }

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) => {
                var length = await stream.ReadLengthAsync();

                Memory<byte> memory = new byte[length];

                await stream.ReadAsync(memory);

                var list = Encoding.UTF8.GetString(memory.Span).Split('|').ToList();

                setter(item, list);
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);

                var list = (List<string>)item;

                Memory<byte> bytes = new byte[0];
                if (list.Any())
                {
                    var text = list.Aggregate((i, j) => i + "|" + j);
                    bytes = Encoding.UTF8.GetBytes(text);
                }
                ReadOnlyMemory<byte> length = BitConverter.GetBytes((ushort)bytes.Length);

                await stream.WriteAsync(length);
                await stream.WriteAsync(bytes);
            };
        }
    }
}
