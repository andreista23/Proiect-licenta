using RecruitmentPlatform.Application.DTOs.Companies;

namespace RecruitmentPlatform.Application.Interfaces;

public interface ICompanyService
{
    Task<CompanyProfileDto> GetMyProfileAsync(Guid userId);

    Task<CompanyProfileDto> UpdateMyProfileAsync(Guid userId, UpdateCompanyProfileDto request);
}