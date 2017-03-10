using System;
using System.Collections.Generic;

namespace NanoSerializer.Tests
{
    public class TestContract
    {
        public enum Test : byte
        {
            One,
            Two,
            Three
        }

        public string One { set; get; }

        public DateTime Two { set; get; }

        public byte[] Three { set; get; }

        public long Count { set; get; }

        public bool Active { get; set; }

        public List<string> Strings { get; set; } = new List<string>();

        public Test TestEnum { get; set; }
    }
}
