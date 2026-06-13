using Microsoft.EntityFrameworkCore;
using RecruitmentPlatform.Application.DTOs.Jobs;
using RecruitmentPlatform.Application.Interfaces;
using RecruitmentPlatform.Domain.Entities;
using RecruitmentPlatform.Domain.Enums;
using RecruitmentPlatform.Infrastructure.Data;
using System.Globalization;
using System.Text;

namespace RecruitmentPlatform.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _dbContext;

    private const string AccentInsensitiveCollation = "Romanian_100_CI_AI";
    public JobService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<JobPostDto>> GetAllAsync(JobFilterDto filter)
    {
        var jobs = await _dbContext.JobPosts
            .Include(job => job.Company)
            .Where(job => job.IsActive)
            .OrderByDescending(job => job.CreatedAt)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = NormalizeText(filter.Search);

            jobs = jobs
                .Where(job =>
                    NormalizeText(job.Title).Contains(search) ||
                    NormalizeText(job.Description).Contains(search) ||
                    NormalizeText(job.Requirements).Contains(search))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            var location = NormalizeText(filter.Location);

            jobs = jobs
                .Where(job => NormalizeText(job.Location).Contains(location))
                .ToList();
        }

        if (filter.JobType.HasValue)
        {
            jobs = jobs
                .Where(job => job.JobType == filter.JobType.Value)
                .ToList();
        }

        if (filter.WorkMode.HasValue)
        {
            jobs = jobs
                .Where(job => job.WorkMode == filter.WorkMode.Value)
                .ToList();
        }

        if (filter.ExperienceLevel.HasValue)
        {
            jobs = jobs
                .Where(job => job.ExperienceLevel == filter.ExperienceLevel.Value)
                .ToList();
        }

        return jobs.Select(MapToDto);
    }

    public async Task<JobPostDto> GetByIdAsync(Guid id)
    {
        var job = await _dbContext.JobPosts
            .Include(job => job.Company)
            .FirstOrDefaultAsync(job => job.Id == id && job.IsActive);

        if (job is null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        return MapToDto(job);
    }

    public async Task<JobPostDto> CreateAsync(Guid userId, CreateJobPostDto request)
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
            throw new UnauthorizedAccessException("Only recruiters can create jobs.");
        }

        if (user.CompanyProfile is null)
        {
            throw new InvalidOperationException("Company profile must be created before posting jobs.");
        }

        var job = new JobPost
        {
            Id = Guid.NewGuid(),
            CompanyId = user.CompanyProfile.Id,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Requirements = request.Requirements.Trim(),
            Location = request.Location.Trim(),
            JobType = request.JobType,
            WorkMode = request.WorkMode,
            ExperienceLevel = request.ExperienceLevel,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.JobPosts.Add(job);
        await _dbContext.SaveChangesAsync();

        job.Company = user.CompanyProfile;

        return MapToDto(job);
    }

    public async Task<JobPostDto> UpdateAsync(Guid userId, Guid jobId, UpdateJobPostDto request)
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
            throw new UnauthorizedAccessException("Only recruiters can update jobs.");
        }

        if (user.CompanyProfile is null)
        {
            throw new InvalidOperationException("Company profile must exist before updating jobs.");
        }

        var job = await _dbContext.JobPosts
            .Include(job => job.Company)
            .FirstOrDefaultAsync(job => job.Id == jobId);

        if (job is null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        if (job.CompanyId != user.CompanyProfile.Id)
        {
            throw new UnauthorizedAccessException("You can update only jobs created by your company.");
        }

        job.Title = request.Title.Trim();
        job.Description = request.Description.Trim();
        job.Requirements = request.Requirements.Trim();
        job.Location = request.Location.Trim();
        job.JobType = request.JobType;
        job.WorkMode = request.WorkMode;
        job.ExperienceLevel = request.ExperienceLevel;
        job.SalaryMin = request.SalaryMin;
        job.SalaryMax = request.SalaryMax;
        job.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        return MapToDto(job);
    }

    public async Task DeleteAsync(Guid userId, Guid jobId)
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
            throw new UnauthorizedAccessException("Only recruiters can delete jobs.");
        }

        if (user.CompanyProfile is null)
        {
            throw new InvalidOperationException("Company profile must exist before deleting jobs.");
        }

        var job = await _dbContext.JobPosts
            .FirstOrDefaultAsync(job => job.Id == jobId);

        if (job is null)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        if (job.CompanyId != user.CompanyProfile.Id)
        {
            throw new UnauthorizedAccessException("You can delete only jobs created by your company.");
        }

        job.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<JobPostDto>> GetMyJobsAsync(Guid userId)
    {
        var companyProfile = await _dbContext.CompanyProfiles
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (companyProfile == null)
        {
            throw new Exception("Profilul companiei nu există.");
        }

        var jobs = await _dbContext.JobPosts
            .Include(j => j.Company)
            .Where(j => j.CompanyId == companyProfile.Id)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new JobPostDto
            {
                Id = j.Id,
                CompanyId = j.CompanyId,
                CompanyName = j.Company.CompanyName,
                Title = j.Title,
                Description = j.Description,
                Requirements = j.Requirements,
                Location = j.Location,
                JobType = j.JobType,
                WorkMode = j.WorkMode,
                ExperienceLevel = j.ExperienceLevel,
                SalaryMin = j.SalaryMin,
                SalaryMax = j.SalaryMax,
                IsActive = j.IsActive,
                CreatedAt = j.CreatedAt
            })
            .ToListAsync();

        return jobs;
    }

    private static string NormalizeText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalizedString = text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var character in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(character);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
    private static JobPostDto MapToDto(JobPost job)
    {
        return new JobPostDto
        {
            Id = job.Id,
            CompanyId = job.CompanyId,
            CompanyName = job.Company?.CompanyName ?? string.Empty,
            Title = job.Title,
            Description = job.Description,
            Requirements = job.Requirements,
            Location = job.Location,
            JobType = job.JobType,
            WorkMode = job.WorkMode,
            ExperienceLevel = job.ExperienceLevel,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            IsActive = job.IsActive,
            CreatedAt = job.CreatedAt
        };
    }
}