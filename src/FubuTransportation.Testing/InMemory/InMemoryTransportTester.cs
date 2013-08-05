﻿using System;
using FubuTransportation.InMemory;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuTransportation.Testing.InMemory
{
    [TestFixture]
    public class InMemoryTransportTester
    {
        [Test]
        public void to_in_memory()
        {
            var settings = InMemoryTransport.ToInMemory<NodeSettings>();

            settings.Inbound.ShouldEqual(new Uri("memory://Node/Inbound"));
            settings.Outbound.ShouldEqual(new Uri("memory://Node/Outbound"));
        }
    }

    public class NodeSettings
    {
        public Uri Inbound { get; set; }
        public Uri Outbound { get; set; }

        public string Something { get; set; }
    }
}