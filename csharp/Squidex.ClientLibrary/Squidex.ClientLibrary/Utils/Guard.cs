// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Squidex.ClientLibrary.Utils
{
    internal static class Guard
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty(string target, string parameterName)
        {
            NotNull(target, parameterName);

            if (string.IsNullOrWhiteSpace(target))
            {
                throw new ArgumentException("String parameter cannot be null or empty and cannot contain only blanks.", parameterName);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty<TType>(IReadOnlyCollection<TType> enumerable, string parameterName)
        {
            NotNull(enumerable, parameterName);

            if (enumerable.Count == 0)
            {
                throw new ArgumentException("Collection does not contain an item.", parameterName);
            }
        }
    }
}
