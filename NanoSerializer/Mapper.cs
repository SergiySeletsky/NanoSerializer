using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NanoSerializer
{
    /// <summary>
    /// Serializer mapper chain
    /// </summary>
    public class Mapper
    {
        internal List<Func<object, Stream, Task>> Getters = new List<Func<object, Stream, Task>>();

        internal List<Func<object, Stream, Task>> Setters = new List<Func<object, Stream, Task>>();
    }
}
