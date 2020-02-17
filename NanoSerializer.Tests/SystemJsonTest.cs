using System.Diagnostics;
using System.Text.Json;
using Xunit;

namespace NanoSerializer.Tests
{
    public class SystemJsonTest : BaseTest
    {
        [Fact]
        public void TestJsonSerialize()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var json = JsonSerializer.Serialize(instance);
            }
            sw.Stop();

            Trace.WriteLine($"JSON Serialize: {sw.ElapsedMilliseconds} ms.");
        }

        [Fact]
        public void TestJsonDeserialize()
        {
            var json = JsonSerializer.Serialize(instance);

            Trace.WriteLine($"JSON Size: {json.Length} bytes.");

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var value = JsonSerializer.Deserialize<TestContract>(json);
            }
            sw.Stop();

            Trace.WriteLine($"JSON Deserialize: {sw.ElapsedMilliseconds} ms.\n");
        }
    }
}
