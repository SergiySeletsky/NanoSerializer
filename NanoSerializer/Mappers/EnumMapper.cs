using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace NanoSerializer.Mappers
{
    internal class EnumMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type.BaseType == typeof(Enum);
        }

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) => {
                Memory<byte> memory = new byte[sizeof(byte)];
                await stream.ReadAsync(memory);

                var value = memory.Span[0];
                setter(item, value);
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);

                ReadOnlyMemory<byte> memory = new byte[] { (byte)item };

                await stream.WriteAsync(memory);
            };
        }
    }
}
