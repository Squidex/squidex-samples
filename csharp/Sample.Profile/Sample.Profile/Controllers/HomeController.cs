using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sample.Profile.Models;
using Squidex.ClientLibrary;

namespace Sample.Profile.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly SquidexClientManager clientManager;
        private readonly IOptions<SquidexOptions> options;

        public HomeController(SquidexClientManager clientManager, IOptions<SquidexOptions> options)
        {
            this.clientManager = clientManager;
            this.options = options;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeVM
            {
                BuildImageUrl = id =>
                {
                    return $"{options.Value.Url}/api/assets/{id}";
                }
            };

            await Task.WhenAll(
                LoadBasics(vm),
                LoadProjectsAsync(vm),
                LoadEducationAsync(vm),
                LoadExperienceAsync(vm),
                LoadPublicationsAsync(vm),
                LoadSkillsAsync(vm));

            return View(vm);
        }

        private async Task LoadSkillsAsync(HomeVM vm)
        {
            var records = await clientManager.GetClient<Skill, SkillData>("skills").GetAsync();

            vm.Skills = records.Items;
        }

        private async Task LoadProjectsAsync(HomeVM vm)
        {
            var records = await clientManager.GetClient<Project, ProjectData>("projects").GetAsync();

            vm.Projects = records.Items;
        }

        private async Task LoadPublicationsAsync(HomeVM vm)
        {
            var records = await clientManager.GetClient<Publication, PublicationData>("publications").GetAsync();

            vm.Publications = records.Items;
        }

        private async Task LoadExperienceAsync(HomeVM vm)
        {
            var records = await clientManager.GetClient<Experience, ExperienceData>("experience").GetAsync();

            vm.Experiences = records.Items;
        }

        private async Task LoadEducationAsync(HomeVM vm)
        {
            var records = await clientManager.GetClient<Education, EducationData>("education").GetAsync();

            vm.Education = records.Items;
        }

        private async Task LoadBasics(HomeVM vm)
        {
            var records = await clientManager.GetClient<Basics, BasicsData>("basics").GetAsync(top: 1);

            vm.Basics = records.Items.FirstOrDefault()?.Data ?? new BasicsData();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
