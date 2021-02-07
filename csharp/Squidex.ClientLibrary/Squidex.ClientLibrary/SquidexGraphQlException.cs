// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Squidex.ClientLibrary.Utils;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Squidex.ClientLibrary
{
    [Serializable]
    public class SquidexGraphQlException : SquidexException
    {
        public IReadOnlyCollection<GraphQlError> Errors { get; }

        public SquidexGraphQlException(IReadOnlyCollection<GraphQlError> errors)
            : base(FormatMessage(errors))
        {
            Errors = errors;
        }

        public SquidexGraphQlException(IReadOnlyCollection<GraphQlError> errors, Exception inner)
            : base(FormatMessage(errors), inner)
        {
            Errors = errors;
        }

        protected SquidexGraphQlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Errors = (List<GraphQlError>)info.GetValue("Errors", typeof(List<GraphQlError>));
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Errors", Errors.ToList());
        }

        private static string FormatMessage(IReadOnlyCollection<GraphQlError> errors)
        {
            Guard.NotNull(errors, nameof(errors));

            var sb = new StringBuilder();

            sb.AppendLine("An GraphQl error occurred.");

            foreach (var error in errors)
            {
                sb.Append(" * ");
                sb.AppendLine(error.Message);
            }

            return sb.ToString();
        }
    }
}
