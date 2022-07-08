// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Runtime.Serialization;
using System.Text;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Exception for GraphQL errors.
    /// </summary>
    [Serializable]
    public class SquidexGraphQlException : SquidexException
    {
        /// <summary>
        /// Provides the GraphQL error details.
        /// </summary>
        public IReadOnlyCollection<GraphQlError> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SquidexGraphQlException"/> class with the GraphQL error details.
        /// </summary>
        /// <param name="errors">The GraphQL errors.</param>
        /// <param name="statusCode">The HTTP status code if available.</param>
        public SquidexGraphQlException(IReadOnlyCollection<GraphQlError> errors, int statusCode)
            : base(FormatMessage(errors), statusCode, null)
        {
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SquidexGraphQlException"/> class with the GraphQL error details and inner exception.
        /// </summary>
        /// <param name="errors">The GraphQL errors.</param>
        /// <param name="statusCode">The HTTP status code if available.</param>
        /// <param name="inner">The inner exception.</param>
        public SquidexGraphQlException(IReadOnlyCollection<GraphQlError> errors, int statusCode, Exception inner)
            : base(FormatMessage(errors), statusCode, null, inner)
        {
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SquidexGraphQlException"/> class with the serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext "/>  that contains contextual information about the source or destination.</param>
        protected SquidexGraphQlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Errors = (List<GraphQlError>)info.GetValue("Errors", typeof(List<GraphQlError>))!;
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
