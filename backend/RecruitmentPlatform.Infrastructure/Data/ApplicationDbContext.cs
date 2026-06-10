using Microsoft.EntityFrameworkCore;
using RecruitmentPlatform.Domain.Entities;

namespace RecruitmentPlatform.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();
    public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
    public DbSet<JobPost> JobPosts => Set<JobPost>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();
    public DbSet<SavedJob> SavedJobs => Set<SavedJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureApplicationUser(modelBuilder);
        ConfigureCandidateSkill(modelBuilder);
        ConfigureSavedJob(modelBuilder);
        ConfigureJobApplication(modelBuilder);
        ConfigureJobPost(modelBuilder);
    }

    private static void ConfigureApplicationUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(user => user.CandidateProfile)
            .WithOne(profile => profile.User)
            .HasForeignKey<CandidateProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(user => user.CompanyProfile)
            .WithOne(profile => profile.User)
            .HasForeignKey<CompanyProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureCandidateSkill(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CandidateSkill>()
            .HasKey(candidateSkill => new
            {
                candidateSkill.CandidateId,
                candidateSkill.SkillId
            });

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(candidateSkill => candidateSkill.Candidate)
            .WithMany(candidate => candidate.CandidateSkills)
            .HasForeignKey(candidateSkill => candidateSkill.CandidateId);

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(candidateSkill => candidateSkill.Skill)
            .WithMany(skill => skill.CandidateSkills)
            .HasForeignKey(candidateSkill => candidateSkill.SkillId);
    }

    private static void ConfigureSavedJob(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavedJob>()
            .HasKey(savedJob => new
            {
                savedJob.CandidateId,
                savedJob.JobPostId
            });

        modelBuilder.Entity<SavedJob>()
            .HasOne(savedJob => savedJob.Candidate)
            .WithMany(candidate => candidate.SavedJobs)
            .HasForeignKey(savedJob => savedJob.CandidateId);

        modelBuilder.Entity<SavedJob>()
            .HasOne(savedJob => savedJob.JobPost)
            .WithMany(jobPost => jobPost.SavedJobs)
            .HasForeignKey(savedJob => savedJob.JobPostId);
    }

    private static void ConfigureJobApplication(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobApplication>()
            .HasIndex(application => new
            {
                application.JobPostId,
                application.CandidateId
            })
            .IsUnique();

        modelBuilder.Entity<JobApplication>()
            .HasOne(application => application.JobPost)
            .WithMany(jobPost => jobPost.Applications)
            .HasForeignKey(application => application.JobPostId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<JobApplication>()
            .HasOne(application => application.Candidate)
            .WithMany(candidate => candidate.Applications)
            .HasForeignKey(application => application.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureJobPost(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobPost>()
            .Property(jobPost => jobPost.SalaryMin)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<JobPost>()
            .Property(jobPost => jobPost.SalaryMax)
            .HasColumnType("decimal(18,2)");
    }
}