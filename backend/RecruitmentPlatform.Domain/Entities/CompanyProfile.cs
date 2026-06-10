namespace RecruitmentPlatform.Domain.Entities;

public class CompanyProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public string CompanyName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? Location { get; set; }

    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}