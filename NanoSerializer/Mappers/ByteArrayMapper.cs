using System;
using System.Collections.Generic;
using System.IO;

namespace NanoSerializer.Mappers
{
    internal class ByteArrayMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(byte[]);
        }

        public override Action<object, Stream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {

                var buffer = new byte[lengthSize];

                stream.Read(buffer, 0, lengthSize);

                var length = BitConverter.ToInt16(buffer, 0);

                var data = new byte[length];

                stream.Read(data, 0, length);

                setter(item, data);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);
                var bytes = (byte[])item;
                var length = BitConverter.GetBytes((ushort)bytes.Length);

                stream.Write(length, 0, length.Length);
                stream.Write(bytes, 0, bytes.Length);
            };
        }
    }
}
