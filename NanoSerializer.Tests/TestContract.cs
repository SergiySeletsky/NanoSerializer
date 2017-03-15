using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NanoSerializer.Tests
{
    [DataContract]
    public class TestContract
    {
        public enum Test : byte
        {
            One,
            Two,
            Three
        }

        [DataMember(Order = 1)]
        public string Text { set; get; }

        [DataMember(Order = 2)]
        public DateTime Date { set; get; }

        [DataMember(Order = 3)]
        public byte[] Bytes { set; get; }

        [DataMember(Order = 4)]
        public long Count { set; get; }

        [DataMember(Order = 5)]
        public int Number { set; get; }

        [DataMember(Order = 6)]
        public bool Active { get; set; }

        [DataMember(Order = 7)]
        public List<string> Strings { get; set; } = new List<string>();

        [DataMember(Order = 8)]
        public Test TestEnum { get; set; }
    }
}
