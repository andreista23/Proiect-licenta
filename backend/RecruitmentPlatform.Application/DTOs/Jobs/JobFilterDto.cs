using RecruitmentPlatform.Domain.Enums;

namespace RecruitmentPlatform.Application.DTOs.Jobs;

public class JobFilterDto
{
    public string? Search { get; set; }

    public string? Location { get; set; }

    public JobType? JobType { get; set; }

    public WorkMode? WorkMode { get; set; }

    public ExperienceLevel? ExperienceLevel { get; set; }
}