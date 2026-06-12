using RecruitmentPlatform.Domain.Enums;

namespace RecruitmentPlatform.Application.DTOs.Applications;

public class UpdateApplicationStatusDto
{
    public ApplicationStatus Status { get; set; }
}