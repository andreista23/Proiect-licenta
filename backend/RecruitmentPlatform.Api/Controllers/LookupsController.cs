using Microsoft.AspNetCore.Mvc;
using RecruitmentPlatform.Application.DTOs.Lookups;
using RecruitmentPlatform.Domain.Enums;

namespace RecruitmentPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupsController : ControllerBase
{
    [HttpGet("options")]
    public ActionResult<LookupOptionsDto> GetOptions()
    {
        var options = new LookupOptionsDto
        {
            JobTypes = new List<LookupOptionDto>
            {
                new() { Value = (int)JobType.FullTime, Label = "Full-time" },
                new() { Value = (int)JobType.PartTime, Label = "Part-time" },
                new() { Value = (int)JobType.Internship, Label = "Internship" },
                new() { Value = (int)JobType.Contract, Label = "Contract" }
            },

            WorkModes = new List<LookupOptionDto>
            {
                new() { Value = (int)WorkMode.OnSite, Label = "On-site" },
                new() { Value = (int)WorkMode.Remote, Label = "Remote" },
                new() { Value = (int)WorkMode.Hybrid, Label = "Hybrid" }
            },

            ExperienceLevels = new List<LookupOptionDto>
            {
                new() { Value = (int)ExperienceLevel.EntryLevel, Label = "Entry Level" },
                new() { Value = (int)ExperienceLevel.Junior, Label = "Junior" },
                new() { Value = (int)ExperienceLevel.MidLevel, Label = "Mid-Level" },
                new() { Value = (int)ExperienceLevel.Senior, Label = "Senior" }
            },

            ApplicationStatuses = new List<LookupOptionDto>
            {
                new() { Value = (int)ApplicationStatus.Submitted, Label = "Trimisă" },
                new() { Value = (int)ApplicationStatus.InReview, Label = "În analiză" },
                new() { Value = (int)ApplicationStatus.Accepted, Label = "Acceptată" },
                new() { Value = (int)ApplicationStatus.Rejected, Label = "Respinsă" },
                new() { Value = (int)ApplicationStatus.Withdrawn, Label = "Retrasă" }
            },

            UserRoles = new List<LookupOptionDto>
            {
                new() { Value = (int)UserRole.Candidate, Label = "Candidat" },
                new() { Value = (int)UserRole.Recruiter, Label = "Recrutor" },
                new() { Value = (int)UserRole.Admin, Label = "Administrator" }
            }
        };

        return Ok(options);
    }
}