using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ProtoBuf.Meta;
using System.IO;

namespace NanoSerializer.Tests
{
    [TestClass]
    public class ProtoTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            RuntimeTypeModel.Default.Add(typeof(TestContract), true);
            RuntimeTypeModel.Default.CompileInPlace();
        }

        [TestMethod]
        public void TestProtoSerialize()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                using (var ms = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, instance);
                    var data = ms.ToArray();
                }      
            }
            sw.Stop();

            Trace.WriteLine($"PROTO Serialize: {sw.ElapsedMilliseconds} ms.");
        }

        [TestMethod]
        public void TestProtoDeserialize()
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, instance);
                data = ms.ToArray();
            }

            Trace.WriteLine($"PROTO Size: {data.Length} bytes.");

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                using (var ms = new MemoryStream(data))
                {
                    var value = ProtoBuf.Serializer.Deserialize<TestContract>(ms);
                }
            }
            sw.Stop();

            Trace.WriteLine($"PROTO Deserialize: {sw.ElapsedMilliseconds} ms.\n");
        }
    }
}
