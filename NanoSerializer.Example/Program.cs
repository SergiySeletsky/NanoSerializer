using NanoSerializer.Tests;
using NanoSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;

namespace NanoSerializer.Example
{
    class Program
    {
        static TestContract instance = new TestContract()
        {
            Text = "NanoSerializer is super fast and compact binary data contract serializer",
            Count = 35346457567,
            Bytes = new byte[400],
            Date = DateTime.Now,
            Number = 111222333,
            Active = true,
            Strings = new List<string> { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" },
            TestEnum = TestContract.Test.Three,
            Contract = new TestContract() //Complex type
            {
                Text = "NanoSerializer is super fast and compact binary data contract serializer",
                Count = 35346457567,
                Bytes = new byte[400],
                Date = DateTime.Now,
                Number = 111222333,
                Active = true
            }
        };

        static void Main(string[] args)
        {
            var serializer = new Serializer(typeof(TestContract));

            for (var i = 0; i < 1000000; i++)
            {
                var data = serializer.Serialize(instance);
                //var value = serializer.Deserialize<TestContract>(data);
            }

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
