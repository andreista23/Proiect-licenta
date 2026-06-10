namespace RecruitmentPlatform.Domain.Entities;

public class SavedJob
{
    public Guid CandidateId { get; set; }

    public CandidateProfile Candidate { get; set; } = null!;

    public Guid JobPostId { get; set; }

    public JobPost JobPost { get; set; } = null!;

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}