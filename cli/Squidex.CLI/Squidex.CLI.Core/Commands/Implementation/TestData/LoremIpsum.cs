// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;

namespace Squidex.CLI.Commands.Implementation.TestData;

public static class LoremIpsum
{
    private const int SentencesPerParagraph = 3;

    private static readonly string[] Words =
    [
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "consectetuer",
        "adipiscing",
        "elit",
        "sed",
        "diam",
        "nonummy",
        "nibh",
        "euismod",
        "tincidunt",
        "ut",
        "laoreet",
        "dolore",
        "magna",
        "aliquam",
        "erat"
    ];

    public static string GetWord(Random random)
    {
        return Words[random.Next(0, Words.Length)];
    }

    public static string Text(int maxCharacters, bool html)
    {
        var sb = new StringBuilder();

        var nextWord = new StringBuilder();

        var sentences = 0;

        for (var i = 0; i < Words.Length; i++)
        {
            var word = Words[i];

            if (sb.Length > 0)
            {
                nextWord.Append(' ');
            }

            nextWord.Append(word);

            if (sb.Length + nextWord.Length < maxCharacters)
            {
                sb.Append(nextWord);

                nextWord.Clear();
            }
            else
            {
                break;
            }

            if (i == Words.Length - 1)
            {
                sentences++;

                var left = maxCharacters - 1;

                void Append(string value)
                {
                    if (left > value.Length)
                    {
                        sb.Append(value);
                        left -= value.Length;
                    }
                }

                Append(".");

                if (sentences % SentencesPerParagraph == 0)
                {
                    if (html)
                    {
                        Append("<br />");
                        Append("<br />");
                    }
                    else
                    {
                        Append("\n");
                        Append("\n");
                    }
                }

                i = -1;
            }
        }

        if (sb.Length == 0)
        {
            return Words[0][..maxCharacters];
        }

        while (sb.Length < maxCharacters)
        {
            sb.Append('.');
        }

        return sb.ToString();
    }
}
