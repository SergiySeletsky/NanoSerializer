using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NanoSerializer.Mappers
{
    internal class BoolMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(bool);
        }

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) => {
                Memory<byte> memory = new byte[sizeof(bool)];
                await stream.ReadAsync(memory);

                var boolean = BitConverter.ToBoolean(memory.Span);

                setter(item, boolean);
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);
                ReadOnlyMemory<byte> memory = BitConverter.GetBytes((bool)item);

                await stream.WriteAsync(memory);
            };
        }
    }
}
