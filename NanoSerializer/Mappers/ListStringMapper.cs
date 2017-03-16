﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static NanoSerializer.Serializer;

namespace NanoSerializer.Mappers
{
    internal class ListStringMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(List<string>);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var length = BitConverter.ToInt16(buffer, source.Index);

                Interlocked.Add(ref source.Index, lengthSize);

                var data = new byte[length];

                Buffer.BlockCopy(buffer, source.Index, data, 0, length);

                Interlocked.Add(ref source.Index, length);

                var list = Encoding.UTF8.GetString(data).Split('|').ToList();

                setter(item, list);
            };
        }

        public override Action<object, List<byte[]>> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);

                var list = (List<string>)item;

                var text = list.Aggregate((i, j) => i + "|" + j);

                var bytes = Encoding.UTF8.GetBytes(text);
                var length = BitConverter.GetBytes((ushort)bytes.Length);

                blocks.Add(length);
                blocks.Add(bytes);
            };
        }
    }
}