// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
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
