using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NanoSerializer.Mappers
{
    internal class ByteArrayMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(byte[]);
        }

        public override Func<object, Stream, Task> Get(Mapper source, Action<object, object> setter)
        {
            return async (item, stream) => {
                var length = await stream.ReadLengthAsync();

                Memory<byte> memory = new byte[length];

                await stream.ReadAsync(memory);

                setter(item, memory.ToArray());
            };
        }

        public override Func<object, Stream, Task> Set(Func<object, object> getter)
        {
            return async (src, stream) => {
                var item = getter(src);
                ReadOnlyMemory<byte> bytes = (byte[])item;
                ReadOnlyMemory<byte> length = BitConverter.GetBytes((ushort)bytes.Length);

                await stream.WriteAsync(length);
                await stream.WriteAsync(bytes);
            };
        }
    }
}
