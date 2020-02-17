using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class IntMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(int);
        }

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                var buffer = new byte[sizeof(int)];
                stream.Read(buffer, 0, sizeof(int));
                var number = BitConverter.ToInt32(buffer, 0);

                setter(item, number);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                var bytes = BitConverter.GetBytes((int)item);
                stream.Write(bytes, 0, bytes.Length);
            };
        }
    }
}
