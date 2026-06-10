namespace RecruitmentPlatform.Application.DTOs.Candidates;

public class UpdateCandidateProfileDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Location { get; set; }

    public string? Bio { get; set; }
}