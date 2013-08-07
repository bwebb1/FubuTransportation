﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuTransportation.Configuration;

namespace FubuTransportation.Runtime
{
    public interface IEnvelopeSerializer
    {
        void Deserialize(Envelope envelope);
        void Serialize(Envelope envelope);

        // TODO -- maybe?
        // string ContentTypeFor<T>() ??
    }

    public class EnvelopeSerializer : IEnvelopeSerializer
    {
        // TODO -- throw if unrecognized content-type

        private readonly ChannelGraph _graph;
        private readonly IEnumerable<IMessageSerializer> _serializers;

        public EnvelopeSerializer(ChannelGraph graph, IEnumerable<IMessageSerializer> serializers)
        {
            _graph = graph;
            _serializers = serializers;
        }

        public void Deserialize(Envelope envelope)
        {
            if (envelope.Data == null) throw new InvalidOperationException("No data on this envelope to deserialize");


            var serializer = selectSerializer(envelope);
            
            using (var stream = new MemoryStream(envelope.Data))
            {
                envelope.Message = serializer.Deserialize(stream);
            }
        }

        private IMessageSerializer selectSerializer(Envelope envelope)
        {

            // TODO -- what to do w/ unknown content-type?
            return _serializers.FirstOrDefault(x => x.ContentType.EqualsIgnoreCase(envelope.ContentType));
        }

        public void Serialize(Envelope envelope)
        {
            if (envelope.Message == null) throw new InvalidOperationException("No message on this envelope to serialize");

            if (envelope.ContentType.IsEmpty())
            {
                envelope.ContentType = _graph.DefaultContentType;
            }

            var serializer = selectSerializer(envelope);
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(envelope.Message, stream);
                stream.Position = 0;

                envelope.Data = stream.ReadAllBytes();
            }
        }
    }
}