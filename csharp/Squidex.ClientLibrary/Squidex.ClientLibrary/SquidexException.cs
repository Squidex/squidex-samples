// ==========================================================================
//  SquidexException.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
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
    }
}
