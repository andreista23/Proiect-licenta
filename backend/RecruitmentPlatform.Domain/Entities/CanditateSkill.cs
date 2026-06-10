namespace RecruitmentPlatform.Domain.Entities;

public class CandidateSkill
{
    public Guid CandidateId { get; set; }

    public CandidateProfile Candidate { get; set; } = null!;

    public Guid SkillId { get; set; }

    public Skill Skill { get; set; } = null!;
}