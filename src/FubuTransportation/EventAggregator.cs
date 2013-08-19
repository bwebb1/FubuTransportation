﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using System.Linq;
using FubuCore.Logging;

namespace FubuTransportation
{

    public class EventAggregator : IEventAggregator
    {
        private readonly Lazy<ILogger> _logger;
        private readonly List<object> _listeners = new List<object>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public EventAggregator(Func<ILogger> logger, IEnumerable<IListener> listeners)
        {
            _logger = new Lazy<ILogger>(logger);
            _listeners.AddRange(listeners);
        }

        public void SendMessage<T>(T message)
        {
            Task.Factory.StartNew(() => sendMessageToListeners(message));
        }

        private void sendMessageToListeners<T>(T message)
        {
            var listeners = _lock.Read(() => _listeners.OfType<IListener<T>>().ToArray());

            listeners.Each(x => {
                try
                {
                    x.Handle(message);
                    _logger.Value.Debug(
                        () => "Successfully processed event {0} with listener {1} in event aggregation".ToFormat(message, x));
                }
                catch (Exception e)
                {
                    _logger.Value.Error("Failed while trying to process event {0} with listener {1}".ToFormat(message, x), e);
                }
            });
        }

        public void SendMessage<T>() where T : new()
        {
            SendMessage(new T());
        }

        public void AddListener(object listener)
        {
            _lock.Write(() => _listeners.Fill(listener));
        }

        public void RemoveListener(object listener)
        {
            _lock.Write(() => _listeners.Remove(listener));
        }

        public IEnumerable<object> Listeners
        {
            get { return _lock.Read(() => _listeners.ToArray()); }
        }

        public void AddListeners(params object[] listeners)
        {
            _lock.Write(() => _listeners.Fill(listeners));
        }

        public bool HasListener(object listener)
        {
            return _lock.Read(() => _listeners.Contains(listener));
        }

        public void RemoveAllListeners()
        {
            _lock.Write(() => _listeners.Clear());
        }
    }
}