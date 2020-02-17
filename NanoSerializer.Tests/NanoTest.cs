using System.Diagnostics;
using Xunit;

namespace NanoSerializer.Tests
{
    public class NanoTest : BaseTest
    {
        private Serializer serializer;

        public NanoTest()
        {
            serializer = new Serializer(typeof(TestContract));
        }

        [Fact]
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

        [Fact]
        public void TestNanoDeserialize()
        {
            byte[] data = serializer.Serialize(instance);

            Trace.WriteLine($"NANO Size: {data.Length} bytes.");

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var value = serializer.Deserialize<TestContract>(data);
            }
            sw.Stop();

            Trace.WriteLine($"NANO Deserialize: {sw.ElapsedMilliseconds} ms.\n");
        }
    }
}
