using RecruitmentPlatform.Application.DTOs.Candidates;

namespace RecruitmentPlatform.Application.Interfaces;

public interface ICandidateService
{
    Task<CandidateProfileDto> GetMyProfileAsync(Guid userId);

    Task<CandidateProfileDto> UpdateMyProfileAsync(Guid userId, UpdateCandidateProfileDto request);
}