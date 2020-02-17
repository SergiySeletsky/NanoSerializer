using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NanoSerializer.Mappers
{
    internal class ListStringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(List<string>);
        }

        public override Action<object, MemoryStream> Get(Mapper source, Action<object, object> setter)
        {
            return (item, stream) => {
                var buffer = new byte[lengthSize];
                stream.Read(buffer, 0, lengthSize);
                var length = BitConverter.ToInt16(buffer, 0);

                var data = new byte[length];

                stream.Read(data, 0, length);

                var list = Encoding.UTF8.GetString(data).Split('|').ToList();

                setter(item, list);
            };
        }

        public override Action<object, MemoryStream> Set(Func<object, object> getter)
        {
            return (src, stream) => {
                var item = getter(src);

                var list = (List<string>)item;

                byte[] bytes = new byte[0];
                if (list.Any())
                {
                    var text = list.Aggregate((i, j) => i + "|" + j);
                    bytes = Encoding.UTF8.GetBytes(text);
                }
                var length = BitConverter.GetBytes((ushort)bytes.Length);

                stream.Write(length, 0, length.Length);
                stream.Write(bytes, 0, bytes.Length);
            };
        }
    }
}
