// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;

namespace Squidex.CLI.Commands.Implementation.Sync;

public sealed class MarkdownWriter
{
    private readonly TextWriter writer;

    public MarkdownWriter(TextWriter writer)
    {
        this.writer = writer;
    }

    public MarkdownWriter H1(string text)
    {
        writer.Write("# ");
        writer.WriteLine(text);
        writer.WriteLine();

        return this;
    }

    public MarkdownWriter H2(string text)
    {
        writer.Write("## ");
        writer.WriteLine(text);
        writer.WriteLine();

        return this;
    }

    public MarkdownWriter H3(string text)
    {
        writer.Write("### ");
        writer.WriteLine(text);
        writer.WriteLine();

        return this;
    }

    public MarkdownWriter Paragraph(string text)
    {
        writer.WriteLine(text);
        writer.WriteLine();

        return this;
    }

    public MarkdownWriter OrderedList(params string[] lines)
    {
        if (lines.Length == 0)
        {
            return this;
        }

        var i = 1;

        foreach (var line in lines)
        {
            writer.Write(' ');
            writer.Write(i);
            writer.Write(". ");
            writer.WriteLine(line);
            i++;
        }

        writer.WriteLine();

        return this;
    }

    public MarkdownWriter BulletPoints(params string[] lines)
    {
        if (lines.Length == 0)
        {
            return this;
        }

        foreach (var line in lines)
        {
            writer.Write(" * ");
            writer.WriteLine(line);
        }

        writer.WriteLine();

        return this;
    }

    public MarkdownWriter Code(params string[] lines)
    {
        if (lines.Length == 0)
        {
            return this;
        }

        writer.WriteLine("```");

        foreach (var line in lines)
        {
            writer.WriteLine(line);
        }

        writer.WriteLine("```");
        writer.WriteLine();

        return this;
    }

    public MarkdownWriter Table(object[] header, object?[][] rows)
    {
        var allRows = Enumerable.Repeat(header, 1).Union(rows);

        var rowTexts = allRows.Select(x => x.Select(y => Convert.ToString(y, CultureInfo.InvariantCulture) ?? string.Empty).ToArray()).ToArray();

        var columnsCount = allRows.Max(x => x.Length);
        var columnsWidth = new int[columnsCount];

        foreach (var row in rowTexts)
        {
            for (var i = 0; i < row.Length; i++)
            {
                columnsWidth[i] = Math.Max(columnsWidth[i], row[i].Length);
            }
        }

        void WriteSeparator()
        {
            writer.Write('|');

            foreach (var width in columnsWidth)
            {
                writer.Write(' ');

                for (var i = 0; i < width; i++)
                {
                    writer.Write('-');
                }

                writer.Write(' ');
                writer.Write("|");
            }

            writer.WriteLine();
        }

        void WriteLine(string[] line)
        {
            writer.Write('|');

            for (var column = 0; column < columnsCount; column++)
            {
                var text = column < line.Length ? line[column] : string.Empty;

                writer.Write(' ');
                writer.Write(text);

                var width = columnsWidth[column];

                for (var i = text.Length; i < width; i++)
                {
                    writer.Write(' ');
                }

                writer.Write(' ');
                writer.Write("|");
            }

            writer.WriteLine();
        }

        WriteLine(rowTexts[0]);
        WriteSeparator();

        foreach (var row in rowTexts.Skip(1))
        {
            WriteLine(row);
        }

        writer.WriteLine();

        return this;
    }
}
