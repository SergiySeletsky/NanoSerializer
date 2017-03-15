using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace NanoSerializer
{
    /// <summary>
    /// NanoSerializer is super fast and compact binary data contract serializer
    /// </summary>
    public sealed class Serializer
    {
        private readonly Dictionary<Type, Mapper> runtime = new Dictionary<Type, Mapper>();

        const int lengthSize = 2;

        public Serializer(Type type)
        {
            var types = type.GetTypeInfo().Assembly.DefinedTypes.Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(DataContractAttribute)));

            foreach (var typeInfo in types)
            {
                Register(typeInfo.AsType());
            }
        }

        /// <summary>
        /// Serializer mapper chain
        /// </summary>
        public class Mapper
        {
            public int Index = 0;

            public List<Action<object, byte[]>> Getters = new List<Action<object, byte[]>>();

            public List<Action<object, List<byte[]>>> Setters = new List<Action<object, List<byte[]>>>();
        }

        private static Action<object, object> BuildSetAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), "o");
            var value = Expression.Parameter(typeof(object));
            var convert = Expression.Convert(value, method.GetParameters()[0].ParameterType);
            var call = Expression.Call(Expression.Convert(obj, method.DeclaringType), method, convert);
            var expr = Expression.Lambda<Action<object, object>>(call, obj, value);
            return expr.Compile();
        }

        private static Func<object, object> BuildGetAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), "o");
            var call = Expression.Call(Expression.Convert(obj, method.DeclaringType), method);
            var convert = Expression.Convert(call, typeof(object));
            var expr = Expression.Lambda<Func<object, object>>(convert, obj);
            return expr.Compile();
        }

        /// <summary>
        /// Builds serializer
        /// </summary>
        private void Register(Type type)
        {
            var builder = new Mapper();

            var properties = type.GetRuntimeProperties().OrderBy(f => f.GetCustomAttribute<DataMemberAttribute>().Order);

            foreach (var property in properties)
            {
                var setter = BuildSetAccessor(property.SetMethod);
                builder.Getters.Add(Getter(builder, property.PropertyType, setter));

                var getter = BuildGetAccessor(property.GetMethod);
                builder.Setters.Add(Setter(property.PropertyType, getter));
            }

            runtime.Add(type, builder);
        }

        private static Action<object, byte[]> Getter(Mapper source, Type type, Action<object, object> setter)
        {
            Action<object, byte[]> method = null;
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

        private static Action<object, List<byte[]>> Setter(Type type, Func<object, object> getter)
        {
            Action<object, List<byte[]>> method = null;

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

        /// <summary>
        /// Serialize data contract to byte array
        /// </summary>
        /// <typeparam name="T">Serializable type</typeparam>
        /// <param name="source">Serializer build model</param>
        /// <param name="instance">Instance of serializable type</param>
        /// <returns>Byte array</returns>
        public byte[] Serialize(object instance)
        {
            var source = runtime[instance.GetType()];

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
                Interlocked.Add(ref offset, item.Length);
            }

            return buffer;
        }

        /// <summary>
        /// Deserialize type from byte array
        /// </summary>
        /// <typeparam name="T">Serialization type</typeparam>
        /// <param name="data">Byte array</param>
        /// <returns>New instance of deserialized contract</returns>
        public T Deserialize<T>(byte[] data) where T : new()
        {
            var item = new T();

            return (T)Deserialize(item, typeof(T), data);
        }

        /// <summary>
        /// Deserialize type from byte array
        /// </summary>
        /// <param name="type">Type of object</param>
        /// <param name="data">Byte array</param>
        /// <returns>New instance of deserialized contract</returns>
        public object Deserialize(Type type, byte[] data)
        {
            var instance = Activator.CreateInstance(type);

            return Deserialize(instance, type, data);
        }

        private object Deserialize(object instance, Type type, byte[] data)
        {
            var source = runtime[type];

            source.Index = 0;
            foreach (var getter in source.Getters)
            {
                getter(instance, data);
            }

            return instance;
        }
    }
}
