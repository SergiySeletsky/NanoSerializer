﻿using System;
using System.Collections.Generic;

namespace NanoSerializer.Mappers
{
    internal class IntMapper : TypeMapper
    {
        public override bool Can(Type type)
        {
            return type == typeof(int);
        }

        public override Action<object, byte[]> Get(Mapper source, Action<object, object> setter)
        {
            return (item, buffer) => {
                var number = BitConverter.ToInt32(buffer, source.Index);
                source.Index += sizeof(int);
                setter(item, number);
            };
        }

        public override Func<object, List<byte[]>, int> Set(Func<object, object> getter)
        {
            return (src, blocks) => {
                var item = getter(src);
                var bytes = BitConverter.GetBytes((int)item);
                blocks.Add(bytes);
                return sizeof(int);
            };
        }
    }
}
