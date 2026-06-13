using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPlatform.Application.DTOs.Jobs;
using RecruitmentPlatform.Application.Interfaces;
using RecruitmentPlatform.Application.DTOs.Applications;

namespace RecruitmentPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly IApplicationService _applicationService;

    public JobsController(IJobService jobService, IApplicationService applicationService)
    {
        _jobService = jobService;
        _applicationService = applicationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] JobFilterDto filter)
    {
        var jobs = await _jobService.GetAllAsync(filter);
        return Ok(jobs);
    }

    [Authorize(Roles = "Recruiter")]
    [HttpGet("my")]
    public async Task<ActionResult<List<JobPostDto>>> GetMyJobs()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid token."
            });
        }

        try
        {
            var jobs = await _jobService.GetMyJobsAsync(userId.Value);
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var job = await _jobService.GetByIdAsync(id);
            return Ok(job);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateJobPostDto request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid token."
            });
        }

        try
        {
            var job = await _jobService.CreateAsync(userId.Value, request);
            return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateJobPostDto request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid token."
            });
        }

        try
        {
            var job = await _jobService.UpdateAsync(userId.Value, id, request);
            return Ok(job);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid token."
            });
        }

        try
        {
            await _jobService.DeleteAsync(userId.Value, id);
            return NoContent();
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/apply")]
    public async Task<IActionResult> Apply(Guid id, CreateJobApplicationDto request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid token."
            });
        }

        try
        {
            var application = await _applicationService.ApplyAsync(userId.Value, id, request);
            return Ok(application);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}