using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NanoSerializer.Mappers
{
    internal class LongMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(long);
        }

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) => {
                Memory<byte> memory = new byte[sizeof(long)];
                await stream.ReadAsync(memory);
                var number = BitConverter.ToInt64(memory.Span);

                setter(item, number);
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);
                ReadOnlyMemory<byte> memory = BitConverter.GetBytes((long)item);
                await stream.WriteAsync(memory);
            };
        }
    }
}
