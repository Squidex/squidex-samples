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
        public static List<T> Sort<T>(HashSet<T> nodes, HashSet<(T To, T From)> edges) where T : IEquatable<T>
        {
            // Empty list that will contain the sorted elements
            var sorted = new List<T>();

            // Set of all nodes with no incoming edges
            var incomingEdges = new HashSet<T>(nodes.Where(n => edges.All(e => !e.From.Equals(n))));

            while (incomingEdges.Any())
            {
                var first = incomingEdges.First();

                incomingEdges.Remove(first);

                sorted.Add(first);

                foreach (var edge in edges.Where(e => e.To.Equals(first)).ToList())
                {
                    var from = edge.From;

                    edges.Remove(edge);

                    if (edges.All(me => !me.From.Equals(from)))
                    {
                        incomingEdges.Add(from);
                    }
                }
            }

            if (edges.Any())
            {
                return null;
            }
            else
            {
                return sorted;
            }
        }
    }
}
