using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;

namespace NanoSerializer.Tests
{
    [TestClass]
    public class SerializerTest
    {
        private const int count = 100000;
        private TestContract instance;
        private Serializer.Builder<TestContract> serializer;

        [TestInitialize]
        public void Initialize()
        {
            serializer = Serializer.Build<TestContract>();

            instance = new TestContract()
            {
                One = "sdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdf",
                Count = 35346457567,
                Three = new byte[400],
                Two = DateTime.Now,
                Active = true,
                Strings = new List<string> { "", "one", "two", "one", "two", "one", "two", "one", "two" },
                TestEnum = TestContract.Test.Three
            };
        }

        [TestMethod]
        public void TestNanoSerialize()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var bytes = serializer.Serialize(instance);
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
                var cls = serializer.Deserialize(data);
            }
            sw.Stop();

            Trace.WriteLine($"NANO Deserialize: {sw.ElapsedMilliseconds} ms.\n");
        }
    }
}
