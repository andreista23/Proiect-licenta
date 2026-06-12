using Microsoft.EntityFrameworkCore;
using RecruitmentPlatform.Application.DTOs.Applications;
using RecruitmentPlatform.Application.Interfaces;
using RecruitmentPlatform.Domain.Entities;
using RecruitmentPlatform.Domain.Enums;
using RecruitmentPlatform.Infrastructure.Data;

namespace RecruitmentPlatform.Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _dbContext;

    public ApplicationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<JobApplicationDto> ApplyAsync(Guid userId, Guid jobId, CreateJobApplicationDto request)
    {
        var user = await _dbContext.Users
            .Include(user => user.CandidateProfile)
            .FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        if (user.Role != UserRole.Candidate)
        {
            throw new UnauthorizedAccessException("Only candidates can apply to jobs.");
        }

        if (user.CandidateProfile is null)
        {
            throw new InvalidOperationException("Candidate profile must be created before applying to jobs.");
        }

        var job = await _dbContext.JobPosts
            .Include(job => job.Company)
            .FirstOrDefaultAsync(job => job.Id == jobId && job.IsActive);

        if (job is null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        var alreadyApplied = await _dbContext.JobApplications
            .AnyAsync(application =>
                application.JobPostId == jobId &&
                application.CandidateId == user.CandidateProfile.Id);

        if (alreadyApplied)
        {
            throw new InvalidOperationException("You have already applied to this job.");
        }

        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            JobPostId = job.Id,
            CandidateId = user.CandidateProfile.Id,
            CoverLetter = request.CoverLetter?.Trim(),
            Status = ApplicationStatus.Submitted,
            AppliedAt = DateTime.UtcNow
        };

        _dbContext.JobApplications.Add(application);
        await _dbContext.SaveChangesAsync();

        application.JobPost = job;
        application.Candidate = user.CandidateProfile;
        application.Candidate.User = user;

        return MapToDto(application);
    }

    public async Task<IEnumerable<JobApplicationDto>> GetMyApplicationsAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(user => user.CandidateProfile)
            .FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        if (user.Role != UserRole.Candidate)
        {
            throw new UnauthorizedAccessException("Only candidates can access their applications.");
        }

        if (user.CandidateProfile is null)
        {
            return Enumerable.Empty<JobApplicationDto>();
        }

        var applications = await _dbContext.JobApplications
            .Include(application => application.JobPost)
            .Include(application => application.Candidate)
                .ThenInclude(candidate => candidate.User)
            .Where(application => application.CandidateId == user.CandidateProfile.Id)
            .OrderByDescending(application => application.AppliedAt)
            .ToListAsync();

        return applications.Select(MapToDto);
    }

    public async Task<IEnumerable<JobApplicationDto>> GetApplicationsForJobAsync(Guid userId, Guid jobId)
    {
        var user = await _dbContext.Users
            .Include(user => user.CompanyProfile)
            .FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        if (user.Role != UserRole.Recruiter)
        {
            throw new UnauthorizedAccessException("Only recruiters can view job applications.");
        }

        if (user.CompanyProfile is null)
        {
            throw new InvalidOperationException("Company profile must exist before viewing applications.");
        }

        var job = await _dbContext.JobPosts
            .FirstOrDefaultAsync(job => job.Id == jobId);

        if (job is null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        if (job.CompanyId != user.CompanyProfile.Id)
        {
            throw new UnauthorizedAccessException("You can view applications only for jobs created by your company.");
        }

        var applications = await _dbContext.JobApplications
            .Include(application => application.JobPost)
            .Include(application => application.Candidate)
                .ThenInclude(candidate => candidate.User)
            .Where(application => application.JobPostId == jobId)
            .OrderByDescending(application => application.AppliedAt)
            .ToListAsync();

        return applications.Select(MapToDto);
    }

    public async Task<JobApplicationDto> UpdateStatusAsync(Guid userId, Guid applicationId, UpdateApplicationStatusDto request)
    {
        var user = await _dbContext.Users
            .Include(user => user.CompanyProfile)
            .FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        if (user.Role != UserRole.Recruiter)
        {
            throw new UnauthorizedAccessException("Only recruiters can update application statuses.");
        }

        if (user.CompanyProfile is null)
        {
            throw new InvalidOperationException("Company profile must exist before updating applications.");
        }

        var application = await _dbContext.JobApplications
            .Include(application => application.JobPost)
            .Include(application => application.Candidate)
                .ThenInclude(candidate => candidate.User)
            .FirstOrDefaultAsync(application => application.Id == applicationId);

        if (application is null)
        {
            throw new KeyNotFoundException("Application not found.");
        }

        if (application.JobPost.CompanyId != user.CompanyProfile.Id)
        {
            throw new UnauthorizedAccessException("You can update only applications for jobs created by your company.");
        }

        application.Status = request.Status;

        await _dbContext.SaveChangesAsync();

        return MapToDto(application);
    }

    private static JobApplicationDto MapToDto(JobApplication application)
    {
        return new JobApplicationDto
        {
            Id = application.Id,
            JobPostId = application.JobPostId,
            JobTitle = application.JobPost?.Title ?? string.Empty,
            CandidateId = application.CandidateId,
            CandidateName = $"{application.Candidate?.FirstName} {application.Candidate?.LastName}".Trim(),
            CandidateEmail = application.Candidate?.User?.Email,
            CoverLetter = application.CoverLetter,
            Status = application.Status,
            AppliedAt = application.AppliedAt
        };
    }
}