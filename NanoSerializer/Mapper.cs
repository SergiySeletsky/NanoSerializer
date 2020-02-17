using System;
using System.Collections.Generic;
using System.Text;

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

        internal List<Func<object, List<byte[]>, int>> Setters = new List<Func<object, List<byte[]>, int>>();
    }
}
