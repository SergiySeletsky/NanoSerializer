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

        /// <summary>
        /// Complex type
        /// </summary>
        [DataMember(Order = 9)]
        public TestContract Contract { get; set; }

        /// <summary>
        /// Complex type 2
        /// </summary>
        [DataMember(Order = 10)]
        public TestContract Contract2 { get; set; }
    }
}
