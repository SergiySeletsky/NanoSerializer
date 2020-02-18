using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Reflection;
using NanoSerializer.Mappers;
using System.IO;
using System.Threading.Tasks;

namespace NanoSerializer
{
    /// <summary>
    /// NanoSerializer is super fast and compact binary data contract serializer
    /// </summary>
    public sealed class Serializer
    {
        private readonly Dictionary<Type, Mapper> runtime = new Dictionary<Type, Mapper>();
        private static readonly List<TypeMapper> mappers = new List<TypeMapper>();

        static Serializer()
        {
            mappers.Add(new BoolMapper());
            mappers.Add(new ByteArrayMapper());
            mappers.Add(new ComplexMapper());
            mappers.Add(new DateTimeMapper());
            mappers.Add(new EnumMapper());
            mappers.Add(new IntMapper());
            mappers.Add(new ListStringMapper());
            mappers.Add(new LongMapper());
            mappers.Add(new StringMapper());
        }

        /// <summary>
        /// Register your custom type mapper
        /// </summary>
        /// <param name="mapper">Instance of your type mapper</param>
        public static void RegisterTypeMapper(TypeMapper mapper)
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            mappers.Add(mapper);
        }


        /// <summary>
        /// Builds instance of serializer for all data contracts in type assembly
        /// </summary>
        /// <param name="type">Any type from your data contracts assembly</param>
        public Serializer(Type type)
        {
            var types = type.Assembly.DefinedTypes.Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(DataContractAttribute)));

            foreach (var typeItem in types)
            {
                Register(typeItem);
            }
        }

        private static Action<object, object> BuildSetAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), string.Empty);
            var value = Expression.Parameter(typeof(object));
            var convert = Expression.Convert(value, method.GetParameters()[0].ParameterType);
            var call = Expression.Call(Expression.Convert(obj, method.DeclaringType), method, convert);
            var expr = Expression.Lambda<Action<object, object>>(call, obj, value);
            return expr.Compile();
        }

        private static Func<object, object> BuildGetAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), string.Empty);
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
                var getter = BuildGetAccessor(property.GetMethod);
                foreach (var mapper in mappers)
                {
                    if (mapper is ComplexMapper)
                    {
                        (mapper as ComplexMapper).Use(this);
                    }
                    if (mapper.Can(property.PropertyType))
                    {
                        var getMapper = mapper.Get(builder, setter);
                        builder.Getters.Add(getMapper);

                        var setMapper = mapper.Set(getter);
                        builder.Setters.Add(setMapper);
                        break;
                    }
                }
            }

            runtime.Add(type, builder);
        }

        /// <summary>
        /// Serialize data contract to byte array
        /// </summary>
        /// <typeparam name="T">Serializable type</typeparam>
        /// <param name="source">Serializer build model</param>
        /// <param name="instance">Instance of serializable type</param>
        /// <returns>Byte array</returns>
        public async Task SerializeAsync(object instance, Stream stream)
        {
            var source = runtime[instance.GetType()];
            await Task.WhenAll(source.Setters.Select(setter => setter(instance, stream)));
        }

        /// <summary>
        /// Serialize data contract to byte array
        /// </summary>
        /// <typeparam name="T">Serializable type</typeparam>
        /// <param name="source">Serializer build model</param>
        /// <param name="instance">Instance of serializable type</param>
        /// <returns>Byte array</returns>
        public async Task<byte[]> SerializeAsync(object instance)
        {
            using (var stream = new MemoryStream())
            {
                await SerializeAsync(instance, stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserialize type from byte array
        /// </summary>
        /// <typeparam name="T">Serialization type</typeparam>
        /// <param name="data">Byte array</param>
        /// <returns>New instance of deserialized contract</returns>
        public async Task<T> DeserializeAsync<T>(byte[] data) where T : new()
        {
            var item = new T();

            using (var ms = new MemoryStream(data))
            {
                await DeserializeAsync(item, typeof(T), ms);
            }

            return item;
        }

        internal async Task DeserializeAsync(object instance, Type type, Stream data)
        {
            var source = runtime[type];
            await Task.WhenAll(source.Getters.Select(getter => getter(instance, data)));
        }
    }
}
