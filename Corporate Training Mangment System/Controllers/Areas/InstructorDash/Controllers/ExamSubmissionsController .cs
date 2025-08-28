using DataAccess.IRepository;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.Areas.InstructorDash.Controllers
{
    [Area("InstructorDash")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]
    public class ExamSubmissionsController : ControllerBase
    {
        private readonly IRepository<ExamSubmission> _submissionRepo;
        private readonly IRepository<Instructor> _instructorRepo;

        public ExamSubmissionsController(
            IRepository<ExamSubmission> submissionRepo,
            IRepository<Instructor> instructorRepo)
        {
            _submissionRepo = submissionRepo;
            _instructorRepo = instructorRepo;
        }
       


        [HttpGet("{examId}/submissions")]
        public async Task<IActionResult> GetSubmissionsForExam(int examId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var instructor = _instructorRepo.GetOne(i => i.ApplicationUserId == userId);

            if (instructor == null)
                return Unauthorized("Instructor not found");

           
            var submissions = await _submissionRepo.GetAsync(
                s => s.ExamId == examId,
                includes: [
                    s => s.Employee,
            s => s.Employee.ApplicationUser,  
            s => s.Exam,
            s => s.Exam.Chapter,
            s => s.Exam.Chapter.Course        
                ]
            );

           
            var ownedSubmissions = submissions.Where(s =>
                s.Exam?.Chapter?.Course?.InstructorId == instructor.Id
            );

            // i am sure all employees must have applicationuser id i can get UserName and Email so we do not need for the following check
            var result = ownedSubmissions.Select(s => new
            {
                EmployeeName = s.Employee?.ApplicationUser?.UserName ?? "Unknown",
                EmployeeEmail = s.Employee?.ApplicationUser?.Email ?? "Unknown",
                Grade = s.Grade,
                SubmittedAt = s.SubmittedAt
            });

            return Ok(result);
        }

    }
}
