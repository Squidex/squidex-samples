// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public static class TopologicalSort
    {
        public static List<T> Sort<T>(HashSet<T> nodes, HashSet<(T From, T To)> edges)
        {
            var ordered = new List<T>();

            var incomingEdges = new HashSet<T>(nodes.Where(n => edges.All(e => e.To.Equals(n) == false)));

            while (incomingEdges.Any())
            {
                var n = incomingEdges.First();

                incomingEdges.Remove(n);

                ordered.Add(n);

                foreach (var e in edges.Where(e => e.From.Equals(n)).ToList())
                {
                    var m = e.To;

                    edges.Remove(e);

                    if (edges.All(me => me.To.Equals(m) == false))
                    {
                        incomingEdges.Add(m);
                    }
                }
            }

            if (edges.Any())
            {
                return null;
            }
            else
            {
                return ordered;
            }
        }
    }
}
