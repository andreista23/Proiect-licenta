using Microsoft.EntityFrameworkCore;
using RecruitmentPlatform.Application.DTOs.Candidates;
using RecruitmentPlatform.Application.Interfaces;
using RecruitmentPlatform.Domain.Entities;
using RecruitmentPlatform.Domain.Enums;
using RecruitmentPlatform.Infrastructure.Data;

namespace RecruitmentPlatform.Infrastructure.Services;

public class CandidateService : ICandidateService
{
    private readonly ApplicationDbContext _dbContext;

    public CandidateService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CandidateProfileDto> GetMyProfileAsync(Guid userId)
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
            throw new UnauthorizedAccessException("Only candidates can access this profile.");
        }

        if (user.CandidateProfile is null)
        {
            var profile = new CandidateProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FirstName = string.Empty,
                LastName = string.Empty
            };

            _dbContext.CandidateProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            return MapToDto(profile);
        }

        return MapToDto(user.CandidateProfile);
    }

    public async Task<CandidateProfileDto> UpdateMyProfileAsync(Guid userId, UpdateCandidateProfileDto request)
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
            throw new UnauthorizedAccessException("Only candidates can update this profile.");
        }

        var profile = user.CandidateProfile;

        if (profile is null)
        {
            profile = new CandidateProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };

            _dbContext.CandidateProfiles.Add(profile);
        }

        profile.FirstName = request.FirstName.Trim();
        profile.LastName = request.LastName.Trim();
        profile.Phone = request.Phone?.Trim();
        profile.Location = request.Location?.Trim();
        profile.Bio = request.Bio?.Trim();

        await _dbContext.SaveChangesAsync();

        return MapToDto(profile);
    }

    private static CandidateProfileDto MapToDto(CandidateProfile profile)
    {
        return new CandidateProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Phone = profile.Phone,
            Location = profile.Location,
            Bio = profile.Bio,
            CvUrl = profile.CvUrl
        };
    }
}