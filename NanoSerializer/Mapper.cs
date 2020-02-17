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
        internal List<Action<object, MemoryStream>> Getters = new List<Action<object, MemoryStream>>();

        internal List<Action<object, MemoryStream>> Setters = new List<Action<object, MemoryStream>>();
    }
}
