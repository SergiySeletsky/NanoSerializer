using System;
using System.Collections.Generic;

namespace NanoSerializer
{
    /// <summary>
    /// Serializer mapper chain
    /// </summary>
    public class Mapper
    {
        /// <summary>
        /// Current buffer index
        /// </summary>
        public int Index = 0;

        internal List<Action<object, byte[]>> Getters = new List<Action<object, byte[]>>();

        internal List<Action<object, List<byte[]>>> Setters = new List<Action<object, List<byte[]>>>();
    }
}
