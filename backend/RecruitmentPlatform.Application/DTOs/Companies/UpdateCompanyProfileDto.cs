namespace RecruitmentPlatform.Application.DTOs.Companies;

public class UpdateCompanyProfileDto
{
    public string CompanyName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? Location { get; set; }
}