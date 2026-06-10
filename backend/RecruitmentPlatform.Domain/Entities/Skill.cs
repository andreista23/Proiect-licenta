namespace RecruitmentPlatform.Domain.Entities;

public class Skill
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
}