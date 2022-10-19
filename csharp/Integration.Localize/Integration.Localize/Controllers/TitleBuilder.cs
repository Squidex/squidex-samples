// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Integration.Localize.Controllers
{
    public static class TitleBuilder
    {
        public static string BuildTitle(DynamicContent content, List<FieldDto> listFields, string masterLanguage)
        {
            var sb = new StringBuilder();

            void Add(JToken value)
            {
                if (value.Type is not JTokenType.Array or JTokenType.Object)
                {
                    var text = value.ToString();

                    if (text == null)
                    {
                        return;
                    }

                    if (sb.Length > 0)
                    {
                        sb.Append(',');
                        sb.Append(' ');
                    }

                    sb.Append(text);
                }
            }

            foreach (var listField in listFields)
            {
                if (!content.Data.TryGetValue(listField.Name, out var temp) || temp is not JObject value)
                {
                    continue;
                }

                if (value.TryGetValue("iv", StringComparison.OrdinalIgnoreCase, out var invariant))
                {
                    Add(invariant);
                }
                else if (value.TryGetValue(masterLanguage, StringComparison.OrdinalIgnoreCase, out var master))
                {
                    Add(master);
                }
                else if (value.Count > 0)
                {
                    Add(value.Properties().First().Value);
                }
            }

            var title = sb.ToString();

            if (title.Length > 100)
            {
                title = title.Substring(0, 97) + "...";
            }

            return title;
        }
    }
}
