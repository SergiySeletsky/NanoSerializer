using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NanoSerializer.Mappers
{
    internal class DateTimeMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(DateTime);
        }

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) => {
                Memory<byte> memory = new byte[sizeof(long)];
                await stream.ReadAsync(memory);

                var ticks = BitConverter.ToInt64(memory.Span);

                setter(item, new DateTime(ticks));
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);
                var dateTime = (DateTime)item;
                ReadOnlyMemory<byte> bytes = BitConverter.GetBytes(dateTime.Ticks);
                await stream.WriteAsync(bytes);
            };
        }
    }
}
