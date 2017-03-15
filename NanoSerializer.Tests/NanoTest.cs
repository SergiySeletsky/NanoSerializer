using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace NanoSerializer.Tests
{
    [TestClass]
    public class NanoTest : BaseTest
    {
        private Serializer.Builder<TestContract> serializer;

        [TestInitialize]
        public void Initialize()
        {
            serializer = Serializer.Build<TestContract>();
        }

        [TestMethod]
        public void TestNanoSerialize()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var data = serializer.Serialize(instance);
            }
            sw.Stop();

            Trace.WriteLine($"NANO Serialize: {sw.ElapsedMilliseconds} ms.");
        }

        [TestMethod]
        public void TestNanoDeserialize()
        {
            byte[] data = serializer.Serialize(instance);

            Trace.WriteLine($"NANO Size: {data.Length} bytes.");

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var value = serializer.Deserialize(data);
            }
            sw.Stop();

            Trace.WriteLine($"NANO Deserialize: {sw.ElapsedMilliseconds} ms.\n");
        }
    }
}
