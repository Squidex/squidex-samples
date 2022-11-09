// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks.Dataflow;

namespace Squidex.CLI.Commands.Implementation.Utils;

public static class TaskExtensions
{
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
    public static async void BidirectionalLinkTo<T>(this ISourceBlock<T> source, ITargetBlock<T> target)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
    {
        source.LinkTo(target, new DataflowLinkOptions
        {
            PropagateCompletion = true
        });

        try
        {
            await target.Completion.ConfigureAwait(false);
        }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
        catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
        {
            // we do not want to change the stacktrace of the exception.
        }

        if (target.Completion.IsFaulted && target.Completion.Exception != null)
        {
            source.Fault(target.Completion.Exception.Flatten());
        }
    }
}
