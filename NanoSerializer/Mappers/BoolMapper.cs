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

        public override Action<object, Stream> Get(Action<object, object> setter)
        {
            return (obj, stream) => {
                Span<byte> span = stackalloc byte[sizeof(bool)];
                stream.Read(span);

                var boolean = BitConverter.ToBoolean(span);

                setter(obj, boolean);
            };
        }

        public override Action<object, Stream> Set(Func<object, object> getter)
        {
            return (obj, stream) => {
                var prop = (bool)getter(obj);
                ReadOnlySpan<byte> span = BitConverter.GetBytes(prop);
                stream.Write(span);
            };
        }
    }
}
