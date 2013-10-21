﻿using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuTransportation.Configuration;
using FubuTransportation.Logging;
using FubuTransportation.Runtime.Serializers;

namespace FubuTransportation.Runtime
{
    public class EnvelopeSender : IEnvelopeSender
    {
        private readonly ISubscriptions _router;
        private readonly IEnvelopeSerializer _serializer;
        private readonly ILogger _logger;
        private readonly IEnumerable<IEnvelopeModifier> _modifiers;

        public EnvelopeSender(ISubscriptions router, IEnvelopeSerializer serializer, ILogger logger, IEnumerable<IEnvelopeModifier> modifiers)
        {
            _router = router;
            _serializer = serializer;
            _logger = logger;
            _modifiers = modifiers;
        }

        // virtual for testing
        public string Send(Envelope envelope)
        {
            envelope.Headers[Envelope.MessageTypeKey] = envelope.Message.GetType().FullName;

            _modifiers.Each(x => x.Modify(envelope));
            _serializer.Serialize(envelope);
            

            var channels = _router.FindChannels(envelope).ToArray();

            if (!channels.Any())
            {
                throw new Exception("No channels match this message ({0})".ToFormat(envelope));
            }

            channels.Each(x => {
                try
                {
                    sendToChannel(envelope, x);
                }
                catch (Exception e)
                {
                    _logger.Error(envelope.CorrelationId, "Failed trying to send message {0} to channel {1}".ToFormat(envelope, x.Uri), e);
                }
            });

            return envelope.CorrelationId;
        }

        private void sendToChannel(Envelope envelope, ChannelNode node)
        {
            // TODO -- I say we change this to returning a Reply Uri and not worrying
            // about having a full node
            var replyNode = _router.ReplyNodeFor(node);

            var headers = node.Send(envelope, replyNode: replyNode);
            _logger.InfoMessage(() => new EnvelopeSent(new EnvelopeToken
            {
                Headers = headers,
                Message = envelope.Message
            }, node));
        }
    }
}