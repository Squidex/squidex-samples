// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Sample.Profile.Models
{
    public sealed class HomeVM
    {
        public BasicsData Basics { get; set; }

        public List<Skill> Skills { get; set; }

        public List<Project> Projects { get; set; }

        public List<Education> Education { get; set; }

        public List<Experience> Experiences { get; set; }

        public List<Publication> Publications { get; set; }

        public Func<string, string> BuildImageUrl { get; set; }
    }
}
