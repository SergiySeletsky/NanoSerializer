using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NanoSerializer.Mappers
{
    internal class IntMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(int);
        }

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) => {
                Memory<byte> memory = new byte[sizeof(int)];
                await stream.ReadAsync(memory);
                var number = BitConverter.ToInt32(memory.Span);

                setter(item, number);
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);
                ReadOnlyMemory<byte> memory = BitConverter.GetBytes((int)item);
                await stream.WriteAsync(memory);
            };
        }
    }
}
