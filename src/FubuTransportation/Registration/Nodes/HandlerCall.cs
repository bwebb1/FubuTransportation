﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuTransportation.Configuration;
using FubuTransportation.Runtime;

namespace FubuTransportation.Registration.Nodes
{
    public class HandlerCall : ActionCallBase, IMayHaveInputType
    {
        public static bool IsCandidate(MethodInfo method)
        {
            if (method.DeclaringType.Equals(typeof(object))) return false;

            int parameterCount = method.GetParameters() == null ? 0 : method.GetParameters().Length;
            if (parameterCount != 1) return false;


            bool hasOutput = method.ReturnType != typeof(void);
            if (hasOutput && !method.ReturnType.IsClass) return false;

            if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")))
                return false;

            if (hasOutput) return true;

            return parameterCount == 1;
        }

        public static HandlerCall For<T>(Expression<Action<T>> method)
        {
            return new HandlerCall(typeof(T), ReflectionHelper.GetMethod(method));
        }

        public HandlerCall(Type handlerType, MethodInfo method)
            : base(handlerType, method)
        {
            if (!IsCandidate(method))
            {
                throw new ArgumentOutOfRangeException("method", method, "The method has to have exactly one input");
            }
        }

        public HandlerCall Clone()
        {
            return new HandlerCall(HandlerType, Method);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Call; }
        }

        protected override Type determineHandlerType()
        {
            if (HasOutput && HasInput)
            {
                return typeof (CascadingHandlerInvoker<,,>)
                    .MakeGenericType(
                        HandlerType,
                        Method.GetParameters().First().ParameterType,
                        Method.ReturnType);
            }

            if (!HasOutput && HasInput)
            {
                return typeof (SimpleHandlerInvoker<,>)
                    .MakeGenericType(
                        HandlerType,
                        Method.GetParameters().First().ParameterType);
            }

            throw new FubuException(1005,
                                    "The action '{0}' is invalid. Only methods that support the '1 in 1 out' or '1 in 0 out' patterns are valid as FubuTransportation handlers",
                                    Description);

        }

        public bool CouldHandleOtherMessageType(Type inputType)
        {
            if (inputType == InputType()) return false;

            return inputType.CanBeCastTo(InputType());
        }

        public void AddClone(HandlerChain chain)
        {
            chain.AddToEnd(Clone());
        }

        public override string ToString()
        {
            return "Handler: " + Description;
        }

        public bool Equals(HandlerCall other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.HandlerType == HandlerType && other.Method.Matches(Method);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(HandlerCall)) return false;
            return Equals((HandlerCall)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((HandlerType != null ? HandlerType.GetHashCode() : 0) * 397) ^
                       (Method != null ? Method.GetHashCode() : 0);
            }
        }


    }
}