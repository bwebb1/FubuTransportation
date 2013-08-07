﻿using FubuCore;

namespace FubuTransportation.Testing.TestSupport
{
    public class MatchingMessageStep<T> : IScenarioStep where T : Message
    {
        private readonly IOriginatingMessage _message;
        private readonly NodeConfiguration _receiver;
        private readonly NodeConfiguration _node;

        public MatchingMessageStep(IOriginatingMessage message, NodeConfiguration receiver)
        {
            _message = message;
            _receiver = receiver;
        }

        public void PreviewAct(IScenarioWriter writer)
        {

        }

        public void PreviewAssert(IScenarioWriter writer)
        {
            if (_message.Description.IsEmpty())
            {
                writer.WriteLine("Expecting message of type {0} to be received by node {1} as a result of message of type {2} being handled", typeof(T).Name, _receiver.Name, _message.Message.GetType().Name);
            }
            else
            {
                writer.WriteLine("Expecting message of type {0} to be received by node {1} as a result of message of type {2} ({3}) being handled", typeof(T).Name, _receiver.Name, _message.Message.GetType().Name, _message.Description);
            }
        }

        public void Act(IScenarioWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Assert(IScenarioWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public bool MatchesMessage(MessageProcessed processed)
        {
            throw new System.NotImplementedException();
        }
    }
}