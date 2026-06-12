using RecruitmentPlatform.Application.DTOs.Applications;

namespace RecruitmentPlatform.Application.Interfaces;

public interface IApplicationService
{
    Task<JobApplicationDto> ApplyAsync(Guid userId, Guid jobId, CreateJobApplicationDto request);

    Task<IEnumerable<JobApplicationDto>> GetMyApplicationsAsync(Guid userId);

    Task<IEnumerable<JobApplicationDto>> GetApplicationsForJobAsync(Guid userId, Guid jobId);

    Task<JobApplicationDto> UpdateStatusAsync(Guid userId, Guid applicationId, UpdateApplicationStatusDto request);
}