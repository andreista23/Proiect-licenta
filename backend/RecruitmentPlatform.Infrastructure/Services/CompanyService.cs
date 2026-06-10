using Microsoft.EntityFrameworkCore;
using RecruitmentPlatform.Application.DTOs.Companies;
using RecruitmentPlatform.Application.Interfaces;
using RecruitmentPlatform.Domain.Entities;
using RecruitmentPlatform.Domain.Enums;
using RecruitmentPlatform.Infrastructure.Data;

namespace RecruitmentPlatform.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _dbContext;

    public CompanyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CompanyProfileDto> GetMyProfileAsync(Guid userId)
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
            throw new UnauthorizedAccessException("Only recruiters can access this profile.");
        }

        if (user.CompanyProfile is null)
        {
            var profile = new CompanyProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CompanyName = string.Empty
            };

            _dbContext.CompanyProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            return MapToDto(profile);
        }

        return MapToDto(user.CompanyProfile);
    }

    public async Task<CompanyProfileDto> UpdateMyProfileAsync(Guid userId, UpdateCompanyProfileDto request)
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
            throw new UnauthorizedAccessException("Only recruiters can update this profile.");
        }

        var profile = user.CompanyProfile;

        if (profile is null)
        {
            profile = new CompanyProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };

            _dbContext.CompanyProfiles.Add(profile);
        }

        profile.CompanyName = request.CompanyName.Trim();
        profile.Description = request.Description?.Trim();
        profile.Website = request.Website?.Trim();
        profile.Location = request.Location?.Trim();

        await _dbContext.SaveChangesAsync();

        return MapToDto(profile);
    }

    private static CompanyProfileDto MapToDto(CompanyProfile profile)
    {
        return new CompanyProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            CompanyName = profile.CompanyName,
            Description = profile.Description,
            Website = profile.Website,
            Location = profile.Location
        };
    }
}