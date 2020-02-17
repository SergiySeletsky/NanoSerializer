using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class DateTimeMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(DateTime);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var ticks = BitConverter.ToInt64(buffer, source.Index);
                source.Index += sizeof(long);
                setter(item, new DateTime(ticks));
            };
        }

        public override Action<object, MemoryStream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                var dateTime = (DateTime)item;
                var bytes = BitConverter.GetBytes(dateTime.Ticks);
                stream.Write(bytes, 0, bytes.Length);
            };
        }
    }
}
