﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Reflection;
using System.Threading;
using NanoSerializer.Mappers;

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
            var typeMapper = typeof(TypeMapper);
            var mapperTypes = typeMapper.GetTypeInfo().Assembly.DefinedTypes.Where(f => f.BaseType == typeMapper);
            foreach(var mapperType in mapperTypes)
            {
                var mapper = (TypeMapper)Activator.CreateInstance(mapperType.AsType());
                mappers.Add(mapper);
            }
        }

        /// <summary>
        /// Register your custom type mapper
        /// </summary>
        /// <param name="mapper">Instance of your type mapper</param>
        public static void RegisterTypeMapper(TypeMapper mapper)
        {
            if(mapper is null)
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
            var types = type.GetTypeInfo().Assembly.DefinedTypes.Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(DataContractAttribute)));

            foreach (var typeInfo in types)
            {
                Register(typeInfo.AsType());
            }
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
                var getter = BuildGetAccessor(property.GetMethod);
                foreach (var mapper in mappers)
                {
                    if(mapper is ComplexMapper)
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
        public byte[] Serialize(object instance)
        {
            if(instance is null)
            {
                return new byte[0];
            }

            var source = runtime[instance.GetType()];

            var blocks = new List<byte[]>();

            var length = 0;
            foreach (var setter in source.Setters)
            {
                length += setter(instance, blocks);
            }

            var buffer = new byte[length];

            var offset = 0;
            foreach (var block in blocks)
            {
                Buffer.BlockCopy(block, 0, buffer, offset, block.Length);
                offset += block.Length;
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
