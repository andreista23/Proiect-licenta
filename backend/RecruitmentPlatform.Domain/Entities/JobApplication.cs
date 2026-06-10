using RecruitmentPlatform.Domain.Enums;

namespace RecruitmentPlatform.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; set; }

    public Guid JobPostId { get; set; }

    public JobPost JobPost { get; set; } = null!;

    public Guid CandidateId { get; set; }

    public CandidateProfile Candidate { get; set; } = null!;

    public string? CoverLetter { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}