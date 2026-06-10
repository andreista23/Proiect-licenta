namespace RecruitmentPlatform.Domain.Entities;

public class CandidateProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Location { get; set; }

    public string? Bio { get; set; }

    public string? CvUrl { get; set; }

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

    public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();

    public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
}