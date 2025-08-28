using DataAccess.IRepository;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.DTOs.Response;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.InstructorDash.Controllers
{
    [Area("InstructorDash")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]
    public class AssignmentSubmissionsController : ControllerBase
    {
        private readonly IRepository<EmployeeAssignmentSubmission> _submissionRepo;
        private readonly IRepository<Assignment> _assignmentRepo;
        private readonly IRepository<Instructor> _instructorRepo;
        public AssignmentSubmissionsController(
        IRepository<EmployeeAssignmentSubmission> submissionRepo,
        IRepository<Assignment> assignmentRepo,
        IRepository<Instructor> instructorRepo)
        {
            _submissionRepo = submissionRepo;
            _assignmentRepo = assignmentRepo;
            _instructorRepo = instructorRepo;
        }


        [HttpGet("by-assignment/{assignmentId}")]
        public async Task<IActionResult> GetSubmissionsForAssignment(int assignmentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor == null) return Unauthorized();

            // Check instructor owns the assignment
            var assignment = _assignmentRepo.GetOne(a => a.Id == assignmentId && a.InstructorId == instructor.Id);
            if (assignment == null)
                return Forbid("You do not own this assignment.");

            var submissions = await _submissionRepo.GetAsync(
                s => s.AssignmentId == assignmentId,
                includes: new Expression<Func<EmployeeAssignmentSubmission, object>>[]
                {
                s => s.Employee,
                s => s.Employee.ApplicationUser,
                s => s.Assignment
                });

            var response = submissions.Select(s => new EmployeeAssignmentSubmissionResponse
            {
                Id = s.Id,
                AssignmentId = s.AssignmentId,
                AssignmentTitle = s.Assignment.Title,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.ApplicationUser.UserName,
                FileUrl = s.FileUrl,
                SubmittedAt = s.SubmittedAt
            });

            return Ok(response);
        }


        [HttpPatch("{submissionId}/grade")]
        public async Task<IActionResult> GradeSubmission(int submissionId, [FromBody] GradeSubmissionRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);
            if (instructor == null) return Unauthorized();

            // Get submission including the assignment
            var submission = _submissionRepo.GetOne(s =>
                s.Id == submissionId && s.Assignment.InstructorId == instructor.Id,
                includes: [s => s.Assignment]);

            if (submission == null)
                return NotFound("Submission not found or not related to your assignment.");

            submission.Grade = request.Grade;

            await _submissionRepo.EditAsync(submission);

            return Ok(new { Message = "Grade assigned successfully.", Grade = request.Grade });
        }

    }
}
