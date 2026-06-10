using RecruitmentPlatform.Domain.Enums;

namespace RecruitmentPlatform.Domain.Entities;

public class JobPost
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public CompanyProfile Company { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Requirements { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public JobType JobType { get; set; }

    public WorkMode WorkMode { get; set; }

    public ExperienceLevel ExperienceLevel { get; set; }

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

    public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
}