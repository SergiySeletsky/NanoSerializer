using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NanoSerializer.Mappers
{
    internal class StringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(string);
        }

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) =>
            {
                var length = await stream.ReadLengthAsync();

                Memory<byte> memory = new byte[length];

                await stream.ReadAsync(memory);

                var text = Encoding.UTF8.GetString(memory.Span);

                setter(item, text);
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);
                var text = (string)item;
                ReadOnlyMemory<byte> bytes = Encoding.UTF8.GetBytes(text);
                ReadOnlyMemory<byte> length = BitConverter.GetBytes((ushort)bytes.Length);

                await stream.WriteAsync(length);
                await stream.WriteAsync(bytes);
            };
        }
    }
}
