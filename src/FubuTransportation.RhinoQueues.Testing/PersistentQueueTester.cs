﻿using System.IO;
using System.Net;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuTransportation.RhinoQueues.Testing
{
    [TestFixture]
    public class PersistentQueueTester
    {
        [SetUp]
        public void Setup()
        {
            if (Directory.Exists("fubutransportation.esent"))
                Directory.Delete("fubutransportation.esent", true);
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "Esent won't work on linux / mono")]
        public void creates_queues_when_started()
        {
            using (var queues = new PersistentQueues())
            {
                queues.Start(new RhinoUri[]
                {
                    new RhinoUri("rhino.queues://localhost:2424/some_queue"), 
                    new RhinoUri("rhino.queues://localhost:2424/other_queue"), 
                    new RhinoUri("rhino.queues://localhost:2424/third_queue"), 
                });

                queues.ManagerFor(new IPEndPoint(IPAddress.Loopback, 2424))
                    .Queues.ShouldHaveTheSameElementsAs("some_queue", "other_queue", "third_queue");
            }
        }
    }
}