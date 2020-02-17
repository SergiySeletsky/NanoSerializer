using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NanoSerializer
{
    /// <summary>
    /// Serializer mapper chain
    /// </summary>
    public class Mapper
    {
        internal List<Action<object, Stream>> Getters = new List<Action<object, Stream>>();

        internal List<Action<object, Stream>> Setters = new List<Action<object, Stream>>();
    }
}
