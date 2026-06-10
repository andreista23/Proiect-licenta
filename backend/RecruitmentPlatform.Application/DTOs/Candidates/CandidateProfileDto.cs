namespace RecruitmentPlatform.Application.DTOs.Candidates;

public class CandidateProfileDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Location { get; set; }

    public string? Bio { get; set; }

    public string? CvUrl { get; set; }
}