using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class LongMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(long);
        }

        public override Action<object, MemoryStream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                var buffer = new byte[sizeof(long)];
                stream.Read(buffer, 0, sizeof(long));
                var number = BitConverter.ToInt64(buffer, 0);

                setter(item, number);
            };
        }

        public override Action<object, MemoryStream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                var bytes = BitConverter.GetBytes((long)item);
                stream.Write(bytes, 0, bytes.Length);
            };
        }
    }
}
