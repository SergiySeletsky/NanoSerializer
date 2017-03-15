using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Diagnostics;

namespace NanoSerializer.Tests
{
    [TestClass]
    public class JsonTest : BaseTest
    {
        [TestMethod]
        public void TestJsonSerialize()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var json = JsonConvert.SerializeObject(instance);
            }
            sw.Stop();

            Trace.WriteLine($"JSON Serialize: {sw.ElapsedMilliseconds} ms.");
        }

        [TestMethod]
        public void TestJsonDeserialize()
        {
            var json = JsonConvert.SerializeObject(instance);

            Trace.WriteLine($"JSON Size: {json.Length} bytes.");

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var value = JsonConvert.DeserializeObject<TestContract>(json);
            }
            sw.Stop();

            Trace.WriteLine($"JSON Deserialize: {sw.ElapsedMilliseconds} ms.\n");
        }
    }
}
