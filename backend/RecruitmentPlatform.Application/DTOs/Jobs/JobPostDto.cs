using RecruitmentPlatform.Domain.Enums;

namespace RecruitmentPlatform.Application.DTOs.Jobs;

public class JobPostDto
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Requirements { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public JobType JobType { get; set; }

    public WorkMode WorkMode { get; set; }

    public ExperienceLevel ExperienceLevel { get; set; }

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}