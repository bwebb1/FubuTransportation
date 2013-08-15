﻿using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Logging;
using FubuTransportation.Configuration;
using FubuTransportation.Runtime;

namespace FubuTransportation
{
    public class TransportActivator : IActivator
    {
        private readonly IEnumerable<ITransport> _transports;
        private readonly ChannelGraph _graph;
        private readonly IServiceLocator _services;
        private readonly IMessageInvoker _invoker;

        public TransportActivator(IEnumerable<ITransport> transports, ChannelGraph graph, IServiceLocator services, IMessageInvoker invoker)
        {
            _transports = transports;
            _graph = graph;
            _services = services;
            _invoker = invoker;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _graph.ReadSettings(_services);
            _transports.Each(x => x.OpenChannels(_graph));

            _graph.StartReceiving(_invoker);
        }
    }
}