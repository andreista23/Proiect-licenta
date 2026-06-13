using RecruitmentPlatform.Application.DTOs.Jobs;

namespace RecruitmentPlatform.Application.Interfaces;

public interface IJobService
{
    Task<IEnumerable<JobPostDto>> GetAllAsync(JobFilterDto filter);

    Task<JobPostDto> GetByIdAsync(Guid id);

    Task<List<JobPostDto>> GetMyJobsAsync(Guid userId);

    Task<JobPostDto> CreateAsync(Guid userId, CreateJobPostDto request);

    Task<JobPostDto> UpdateAsync(Guid userId, Guid jobId, UpdateJobPostDto request);

    Task DeleteAsync(Guid userId, Guid jobId);
}