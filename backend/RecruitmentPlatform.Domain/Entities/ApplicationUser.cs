using RecruitmentPlatform.Domain.Enums;

namespace RecruitmentPlatform.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CandidateProfile? CandidateProfile { get; set; }

    public CompanyProfile? CompanyProfile { get; set; }
}