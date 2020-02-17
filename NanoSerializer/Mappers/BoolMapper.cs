using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class BoolMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(bool);
        }

        public override Action<object, MemoryStream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                var buffer = new byte[sizeof(bool)];
                stream.Read(buffer, 0, sizeof(bool));

                var boolean = BitConverter.ToBoolean(buffer, 0);

                setter(item, boolean);
            };
        }

        public override Action<object, MemoryStream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                var bytes = BitConverter.GetBytes((bool)item);

                stream.Write(bytes, 0, bytes.Length);
            };
        }
    }
}
