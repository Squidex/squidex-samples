// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace CodeGeneration
{
    public static class CodeCleaner
    {
        public static string UseFixedVersion(this string code)
        {
            code = code.Replace("13.18.2.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v12.0.0.0))", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v9.0.0.0))");

            return code;
        }

        public static string UseCloudUrl(this string code)
        {
            code = code.Replace("https://localhost:5001", "https://cloud.squidex.io");

            return code;
        }
    }
}
