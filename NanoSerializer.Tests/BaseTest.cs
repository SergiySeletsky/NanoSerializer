using System;
using System.Collections.Generic;

namespace NanoSerializer.Tests
{
    public abstract class BaseTest
    {
        protected const int count = 1000000;
        protected TestContract instance = new TestContract()
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
            },
            Contract2 = new TestContract() //Complex type
            {
                Text = "NanoSerializer is super fast and compact binary data contract serializer",
                Count = 35346457567,
                Bytes = new byte[400],
                Date = DateTime.Now,
                Number = 111222333,
                Active = true,
                Contract = new TestContract() //Complex type
                {
                    Text = "NanoSerializer is super fast and compact binary data contract serializer",
                    Count = 35346457567,
                    Bytes = new byte[400],
                    Date = DateTime.Now,
                    Number = 111222333,
                    Active = true
                }
            }
        };
    }
}
