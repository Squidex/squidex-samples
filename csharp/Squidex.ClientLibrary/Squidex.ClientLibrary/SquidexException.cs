// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Runtime.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Squidex.ClientLibrary
{
    [Serializable]
    public class SquidexException : Exception
    {
        public SquidexException()
        {
        }

        public SquidexException(string message)
            : base(message)
        {
        }

        public SquidexException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SquidexException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
