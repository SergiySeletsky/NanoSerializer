using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace NanoSerializer
{
    public static class Serializer
    {
        const int lengthSize = 2;

        public class Builder<T>
        {
            public int Index = 0;

            public List<Action<T, byte[]>> Getters = new List<Action<T, byte[]>>();

            public List<Action<T, List<byte[]>>> Setters = new List<Action<T, List<byte[]>>>();
        }

        static Action<T, object> BuildSetAccessor<T>(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(T), "o");
            var value = Expression.Parameter(typeof(object));
            var convert = Expression.Convert(value, method.GetParameters()[0].ParameterType);
            var call = Expression.Call(Expression.Convert(obj, method.DeclaringType), method, convert);
            var expr = Expression.Lambda<Action<T, object>>(call, obj, value);
            return expr.Compile();
        }

        static Func<T, object> BuildGetAccessor<T>(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(T), "o");
            var call = Expression.Call(Expression.Convert(obj, method.DeclaringType), method);
            var convert = Expression.Convert(call, typeof(object));
            var expr = Expression.Lambda<Func<T, object>>(convert, obj);
            return expr.Compile();
        }

        public static Builder<T> Build<T>()
        {
            var builder = new Builder<T>();

            var properties = typeof(T).GetRuntimeProperties();

            foreach (var property in properties)
            {
                var setter = BuildSetAccessor<T>(property.SetMethod);
                builder.Getters.Add(Getter(builder, property.PropertyType, setter));

                var getter = BuildGetAccessor<T>(property.GetMethod);
                builder.Setters.Add(Setter(property.PropertyType, getter));
            }

            return builder;
        }

        public static Action<T, byte[]> Getter<T>(Builder<T> source, Type type, Action<T, object> setter)
        {
            Action<T, byte[]> method = null;
            if (type == typeof(string))
            {
                method = (item, buffer) =>
                {
                    var length = BitConverter.ToInt16(buffer, source.Index);
                    Interlocked.Add(ref source.Index, lengthSize);

                    var data = new byte[length];

                    Buffer.BlockCopy(buffer, source.Index, data, 0, length);

                    Interlocked.Add(ref source.Index, length);

                    var text = Encoding.UTF8.GetString(data);

                    setter(item, text);
                };
            }
            else if (type == typeof(byte[]))
            {
                method = (item, buffer) => {
                    var length = BitConverter.ToInt16(buffer, source.Index);

                    Interlocked.Add(ref source.Index, lengthSize);

                    var data = new byte[length];

                    Buffer.BlockCopy(buffer, source.Index, data, 0, length);

                    Interlocked.Add(ref source.Index, length);

                    setter(item, data);
                };
            }
            else if (type == typeof(int))
            {
                method = (item, buffer) => {
                    var number = BitConverter.ToInt32(buffer, source.Index);
                    Interlocked.Add(ref source.Index, sizeof(int));
                    setter(item, number);
                };
            }
            else if (type == typeof(long))
            {
                method = (item, buffer) => {
                    var number = BitConverter.ToInt64(buffer, source.Index);
                    Interlocked.Add(ref source.Index, sizeof(long));
                    setter(item, number);
                };
            }
            else if (type == typeof(bool))
            {
                method = (item, buffer) => {
                    var boolean = BitConverter.ToBoolean(buffer, source.Index);
                    Interlocked.Add(ref source.Index, sizeof(bool));
                    setter(item, boolean);
                };
            }
            else if (type == typeof(DateTime))
            {
                method = (item, buffer) => {
                    var ticks = BitConverter.ToInt64(buffer, source.Index);
                    Interlocked.Add(ref source.Index, sizeof(long));
                    setter(item, new DateTime(ticks));
                };
            }
            else if (type == typeof(List<string>))
            {
                method = (item, buffer) => {
                    var length = BitConverter.ToInt16(buffer, source.Index);

                    Interlocked.Add(ref source.Index, lengthSize);

                    var data = new byte[length];

                    Buffer.BlockCopy(buffer, source.Index, data, 0, length);

                    Interlocked.Add(ref source.Index, length);

                    var list = Encoding.UTF8.GetString(data).Split('|').ToList();

                    setter(item, list);
                };

            }
            else if (type.GetTypeInfo().BaseType == typeof(Enum))
            {
                method = (item, buffer) => {
                    var value = Buffer.GetByte(buffer, source.Index);
                    Interlocked.Add(ref source.Index, sizeof(byte));
                    setter(item, value);
                };
            }

            return method;
        }

        public static Action<T, List<byte[]>> Setter<T>(Type type, Func<T, object> getter)
        {
            Action<T, List<byte[]>> method = null;

            if (type == typeof(string))
            {
                method = (src, blocks) => {
                    var item = getter(src);
                    var text = (string)item;
                    var bytes = Encoding.UTF8.GetBytes(text);
                    var length = BitConverter.GetBytes((ushort)bytes.Length);

                    blocks.Add(length);
                    blocks.Add(bytes);
                };
            }
            else if (type == typeof(byte[]))
            {
                method = (src, blocks) => {
                    var item = getter(src);
                    var bytes = (byte[])item;
                    var length = BitConverter.GetBytes((ushort)bytes.Length);

                    blocks.Add(length);
                    blocks.Add(bytes);
                };
            }
            else if (type == typeof(int))
            {
                method = (src, blocks) => {
                    var item = getter(src);
                    var bytes = BitConverter.GetBytes((int)item);
                    blocks.Add(bytes);
                };
            }
            else if (type == typeof(long))
            {
                method = (src, blocks) => {
                    var item = getter(src);
                    var bytes = BitConverter.GetBytes((long)item);
                    blocks.Add(bytes);
                };
            }
            else if (type == typeof(bool))
            {
                method = (src, blocks) => {
                    var item = getter(src);
                    var bytes = BitConverter.GetBytes((bool)item);
                    blocks.Add(bytes);
                };
            }
            else if (type == typeof(DateTime))
            {
                method = (src, blocks) => {
                    var item = getter(src);
                    var dateTime = (DateTime)item;
                    var bytes = BitConverter.GetBytes(dateTime.Ticks);
                    blocks.Add(bytes);
                };
            }
            else if (type == typeof(List<string>))
            {
                method = (src, blocks) => {
                    var item = getter(src);

                    var list = (List<string>)item;

                    var text = list.Aggregate((i, j) => i + "|" + j);

                    var bytes = Encoding.UTF8.GetBytes(text);
                    var length = BitConverter.GetBytes((ushort)bytes.Length);

                    blocks.Add(length);
                    blocks.Add(bytes);
                };
            }
            else if (type.GetTypeInfo().BaseType == typeof(Enum))
            {
                method = (src, blocks) => {
                    var item = getter(src);
                    blocks.Add(new byte[1] { (byte)item });
                };
            }

            return method;
        }

        public static byte[] Serialize<T>(this Builder<T> source, T instance)
        {
            var blocks = new List<byte[]>();

            foreach (var setter in source.Setters)
            {
                setter(instance, blocks);
            }

            var length = blocks.Select(f => f.Length).Sum();
            var buffer = new byte[length];

            var offset = 0;
            foreach (var item in blocks)
            {
                Buffer.BlockCopy(item, 0, buffer, offset, item.Length);
                offset += item.Length;
            }

            return buffer;
        }

        public static T Deserialize<T>(this Builder<T> source, byte[] data) where T : new()
        {
            var item = new T();
            source.Index = 0;
            foreach (var getter in source.Getters)
            {
                getter(item, data);
            }
            return item;
        }
    }
}
