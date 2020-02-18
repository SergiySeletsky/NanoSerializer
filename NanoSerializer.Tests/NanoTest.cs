using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace NanoSerializer.Tests
{
    public class NanoTest : BaseTest
    {
        private readonly Serializer serializer;

        public NanoTest()
        {
            // var types = type.Assembly.DefinedTypes.Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(DataContractAttribute)));
            serializer = new Serializer(typeof(TestContract));
        }

        [Fact]
        public void TestNanoFeatures()
        {
            ReadOnlySpan<byte> data = serializer.Serialize(instance);
            var value = serializer.Deserialize<TestContract>(data);

            Assert.Equal(instance.Active, value.Active);
            Assert.Equal(instance.Bytes, value.Bytes);
            Assert.Equal(instance.Contract.Text, value.Contract.Text);
            Assert.Equal(instance.Contract2.Text, value.Contract2.Text);
            Assert.Equal(instance.Count, value.Count);
            Assert.Equal(instance.Date, value.Date);
            Assert.Equal(instance.Number, value.Number);
            Assert.Equal(instance.Strings, value.Strings);
            Assert.Equal(instance.TestEnum, value.TestEnum);
            Assert.Equal(instance.Text, value.Text);
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

        [Fact]
        public void TestNanoParallelize()
        {
            var sw = Stopwatch.StartNew();
            Parallel.For(0, count, i =>
            {
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(instance, stream);
                    var value = serializer.Deserialize<TestContract>(stream);
                }
            });
            sw.Stop();

            Trace.WriteLine($"NANO Deserialize: {sw.ElapsedMilliseconds} ms.\n");
        }

        [Fact]
        public void TestNanoStream()
        {
            var sw = Stopwatch.StartNew();
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(instance, stream);
                var value = serializer.Deserialize<TestContract>(stream);

                Assert.Equal(instance.Active, value.Active);
                Assert.Equal(instance.Bytes, value.Bytes);
                Assert.Equal(instance.Contract2.Text, value.Contract2.Text);
                Assert.Equal(instance.Count, value.Count);
                Assert.Equal(instance.Date, value.Date);
                Assert.Equal(instance.Number, value.Number);
                Assert.Equal(instance.Strings, value.Strings);
                Assert.Equal(instance.TestEnum, value.TestEnum);
                Assert.Equal(instance.Text, value.Text);
            }
            sw.Stop();

            Trace.WriteLine($"NANO Deserialize: {sw.ElapsedMilliseconds} ms.\n");
        }
    }
}
