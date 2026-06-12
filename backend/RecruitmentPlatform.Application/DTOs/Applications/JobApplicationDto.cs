using RecruitmentPlatform.Domain.Enums;

namespace RecruitmentPlatform.Application.DTOs.Applications;

public class JobApplicationDto
{
    public Guid Id { get; set; }

    public Guid JobPostId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public Guid CandidateId { get; set; }

    public string CandidateName { get; set; } = string.Empty;

    public string? CandidateEmail { get; set; }

    public string? CoverLetter { get; set; }

    public ApplicationStatus Status { get; set; }

    public DateTime AppliedAt { get; set; }
}