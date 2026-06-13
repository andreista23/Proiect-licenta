namespace RecruitmentPlatform.Application.DTOs.Lookups;

public class LookupOptionsDto
{
    public List<LookupOptionDto> JobTypes { get; set; } = new();
    public List<LookupOptionDto> WorkModes { get; set; } = new();
    public List<LookupOptionDto> ExperienceLevels { get; set; } = new();
    public List<LookupOptionDto> ApplicationStatuses { get; set; } = new();
    public List<LookupOptionDto> UserRoles { get; set; } = new();
}