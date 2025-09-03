using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Request;
using Service.Utility.Progress;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Corporate_Training_Mangment_System.Controllers.EmployeesCompany.Controllers
{
    [Area("EmployeeDash")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    public class AssignmentSubmissionsController : ControllerBase
    {
        private readonly IRepository<EmployeeAssignmentSubmission> _submissionRepo;
        private readonly IRepository<Employee> _employeeRepo;
        private readonly IRepository<EmployeeAssignment> _employeeAssignmentRepo;
        private readonly IProgressService _progressService;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<EmployeeCourseProgress> _employeeCourseProgressRepository;
        private readonly IRepository<EmployeeCourse> _employeeCourseRepository; 


        public AssignmentSubmissionsController(
            IRepository<EmployeeAssignmentSubmission> submissionRepo,
            IRepository<Employee> employeeRepo,
            IRepository<EmployeeAssignment> employeeAssignmentRepo, IProgressService progressService
            , IRepository<Assignment> assignmentRepository,
            IRepository<EmployeeCourseProgress> employeecourseprogressRepository,IRepository<EmployeeCourse> employeeCourseRepository)
        {
            _submissionRepo = submissionRepo;
            _employeeRepo = employeeRepo;
            _employeeAssignmentRepo = employeeAssignmentRepo;
            _progressService = progressService;
            _assignmentRepository = assignmentRepository;
           
            _employeeCourseProgressRepository = employeecourseprogressRepository;
            _employeeCourseRepository = employeeCourseRepository;

        }

        //My submission without if progress >85 mark course as completed

        //        [HttpPost]
        //        [Consumes("multipart/form-data")]
        //        public async Task<IActionResult> SubmitAssignment([FromForm] EmployeeAssignmentSubmissionRequest request)
        //        {
        //            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //            var employee = _employeeRepo.GetOne(e => e.ApplicationUserId == userId);
        //            if (employee is null) return Unauthorized("Employee not found.");


        //            //var employeeAssignment = _employeeAssignmentRepo.GetOne(ea =>
        //            //    ea.EmployeeId == employee.Id && ea.AssignmentId == request.AssignmentId);

        //            //for progress
        //            var employeeAssignment = _employeeAssignmentRepo.GetOne(
        //    ea => ea.EmployeeId == employee.Id && ea.AssignmentId == request.AssignmentId,
        //    includes: [ea => ea.Assignment, ea => ea.Assignment.Lesson, ea => ea.Assignment.Lesson.Chapter]
        //);

        //            if (employeeAssignment is null)
        //                return BadRequest("This assignment is not assigned to you.");




        //            var existing = _submissionRepo.GetOne(s =>
        //                s.EmployeeId == employee.Id && s.AssignmentId == request.AssignmentId);
        //            if (existing != null)
        //                return BadRequest("You have already submitted this assignment.");

        //            string fileUrl = null;
        //            if (request.File != null)
        //            {
        //                var allowedExtensions = new[] { ".pdf", ".docx", ".doc", ".txt" };
        //                var ext = Path.GetExtension(request.File.FileName).ToLower();
        //                if (!allowedExtensions.Contains(ext))
        //                    return BadRequest("Invalid file type.");

        //                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Submissions");
        //                Directory.CreateDirectory(folder);
        //                var fileName = $"{Guid.NewGuid()}{ext}";
        //                var filePath = Path.Combine(folder, fileName);
        //                using var stream = new FileStream(filePath, FileMode.Create);
        //                await request.File.CopyToAsync(stream);

        //                fileUrl = fileName;
        //            }

        //            var submission = new EmployeeAssignmentSubmission
        //            {
        //                AssignmentId = request.AssignmentId,
        //                EmployeeId = employee.Id,
        //                FileUrl = fileUrl,
        //                SubmittedAt = DateTime.UtcNow
        //            };

        //            await _submissionRepo.CreateAsync(submission);
        //            //for progress
        //            //        var assignment = _assignmentRepository.GetOne(
        //            //a => a.Id == request.AssignmentId,
        //            //includes: [a => a.Lesson, a => a.Lesson.Chapter]);

        //            //        if (assignment != null)
        //            //        {
        //            //            await _progressService.UpdateEmployeeCourseProgress(employee.Id, assignment.Lesson.Chapter.CourseId);
        //            //        }



        //            // ✅ Update or create EmployeeAssignment as completed
        //            if (employeeAssignment == null)
        //            {
        //                var newEmployeeAssignment = new EmployeeAssignment
        //                {
        //                    EmployeeId = employee.Id,
        //                    AssignmentId = request.AssignmentId,

        //                    CompletedAt = DateTime.UtcNow,
        //                    IsCompleted = true
        //                };

        //                await _employeeAssignmentRepo.CreateAsync(newEmployeeAssignment);
        //            }
        //            else
        //            {
        //                employeeAssignment.CompletedAt = DateTime.UtcNow;
        //                employeeAssignment.IsCompleted = true;

        //                await _employeeAssignmentRepo.EditAsync(employeeAssignment);
        //            }

        //            // Now update progress
        //            var assignment = _assignmentRepository.GetOne(
        //                a => a.Id == request.AssignmentId,
        //                includes: [a => a.Lesson, a => a.Lesson.Chapter]);



        //            // i add this because if assignment deadline is finish he can not submit
        //            if (assignment == null)
        //                return BadRequest("Invalid assignment.");

        //            if (assignment.Deadline < DateTime.UtcNow)
        //                return Ok("Submission deadline has passed. You cannot submit this assignment.");


        //            if (assignment != null)
        //            {
        //                await _progressService.UpdateEmployeeCourseProgress(employee.Id, assignment.Lesson.Chapter.CourseId);


        //                var progress = _employeeCourseProgressRepository.GetOne(p =>
        //    p.EmployeeId == employee.Id && p.CourseId == assignment.Lesson.Chapter.CourseId);

        //                if (progress is not null && progress.TotalProgress >= 85)
        //                {
        //                    var employeeCourse = _employeeCourseRepository.GetOne(ec =>
        //                        ec.EmployeeId == employee.Id && ec.CourseId == assignment.Lesson.Chapter.CourseId);

        //                    if (employeeCourse is not null && !employeeCourse.IsCompleted&& employeeCourse.CompletedAt<DateTime.UtcNow)
        //                    {
        //                        employeeCourse.IsCompleted = true;


        //                        await _employeeCourseRepository.EditAsync(employeeCourse);
        //                    }
        //                }

        //            }



        //            return Ok(new { Message = "Submission uploaded successfully." });
        //        }




        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubmitAssignment([FromForm] EmployeeAssignmentSubmissionRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = _employeeRepo.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null)
                return Unauthorized("Employee not found.");

            // Load assignment and course
            var assignment = _assignmentRepository.GetOne(
                a => a.Id == request.AssignmentId,
                includes: [a => a.Lesson, a => a.Lesson.Chapter]
            );

            if (assignment == null)
                return BadRequest("Invalid assignment.");

            // 🔒 Check: Assignment deadline
            if (assignment.Deadline < DateTime.UtcNow)
                return Forbid("Submission deadline has passed. You cannot submit this assignment.");

            // 🔒 Check: Course is locked
            var courseId = assignment.Lesson.Chapter.CourseId;

            var employeeCourse = _employeeCourseRepository.GetOne(ec =>
                ec.EmployeeId == employee.Id &&
                ec.CourseId == courseId
            );

            if (employeeCourse != null && employeeCourse.IsCompleted && employeeCourse.CompletedAt < DateTime.UtcNow)
                return Forbid("Course is locked. You cannot submit any more assignments.");

            // Load employee assignment
            var employeeAssignment = _employeeAssignmentRepo.GetOne(
                ea => ea.EmployeeId == employee.Id && ea.AssignmentId == request.AssignmentId,
                includes: [ea => ea.Assignment, ea => ea.Assignment.Lesson, ea => ea.Assignment.Lesson.Chapter]
            );

            if (employeeAssignment is null)
                return BadRequest("This assignment is not assigned to you.");

            // Check for existing submission
            var existing = _submissionRepo.GetOne(s =>
                s.EmployeeId == employee.Id && s.AssignmentId == request.AssignmentId);
            if (existing != null)
                return BadRequest("You have already submitted this assignment.");

            // Save file
            string fileUrl = null;
            if (request.File != null)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".doc", ".txt" };
                var ext = Path.GetExtension(request.File.FileName).ToLower();
                if (!allowedExtensions.Contains(ext))
                    return BadRequest("Invalid file type.");

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Submissions");
                Directory.CreateDirectory(folder);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(folder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await request.File.CopyToAsync(stream);

                fileUrl = fileName;
            }

            // Save submission
            var submission = new EmployeeAssignmentSubmission
            {
                AssignmentId = request.AssignmentId,
                EmployeeId = employee.Id,
                FileUrl = fileUrl,
                SubmittedAt = DateTime.UtcNow
            };

            await _submissionRepo.CreateAsync(submission);

            // ✅ Update or create EmployeeAssignment as completed
            if (employeeAssignment == null)
            {
                var newEmployeeAssignment = new EmployeeAssignment
                {
                    EmployeeId = employee.Id,
                    AssignmentId = request.AssignmentId,
                    CompletedAt = DateTime.UtcNow,
                    IsCompleted = true
                };

                await _employeeAssignmentRepo.CreateAsync(newEmployeeAssignment);
            }
            else
            {
                employeeAssignment.CompletedAt = DateTime.UtcNow;
                employeeAssignment.IsCompleted = true;
                await _employeeAssignmentRepo.EditAsync(employeeAssignment);
            }

            // Update course progress
            await _progressService.UpdateEmployeeCourseProgress(employee.Id, courseId);

            // Refresh course progress and possibly complete course
            //var progress = _employeeCourseProgressRepository.GetOne(p =>
            //    p.EmployeeId == employee.Id && p.CourseId == courseId);

            //if (progress is not null && progress.TotalProgress >= 70)
            //{
            //    if (employeeCourse is not null && !employeeCourse.IsCompleted && employeeCourse.CompletedAt > DateTime.UtcNow)
            //    {
            //        employeeCourse.IsCompleted = true;
            //        await _employeeCourseRepository.EditAsync(employeeCourse);
            //    }
            //}
            //else if (progress is not null && progress.TotalProgress < 70)
            //{
            //    if (employeeCourse is not null && employeeCourse.IsCompleted && employeeCourse.CompletedAt > DateTime.UtcNow)
            //    {
            //        employeeCourse.IsCompleted = false;
            //        await _employeeCourseRepository.EditAsync(employeeCourse);
            //    }
            //}


            //===============><=======================
            // i test it for debuge and i found [Debug] TotalProgress: 88.31
          //  [Debug] CompletedAt(deadline): 9 / 16 / 2025 6:56:33 PM, Now: 9 / 3 / 2025 1:27:24 PM
           // [Debug] Conditions not met for marking as completed
          //  [Debug] IsCompleted: True
          //but in my database not mark as true
            

                        var progress = _employeeCourseProgressRepository.GetOne(p =>
    p.EmployeeId == employee.Id && p.CourseId == courseId);

            Console.WriteLine($"[Debug] TotalProgress: {progress?.TotalProgress}");
            Console.WriteLine($"[Debug] CompletedAt (deadline): {employeeCourse?.CompletedAt}, Now: {DateTime.UtcNow}");

            if (progress is not null && progress.TotalProgress >= 70)
            {
                if (employeeCourse is not null && !employeeCourse.IsCompleted && employeeCourse.CompletedAt > DateTime.UtcNow)
                {
                    employeeCourse.IsCompleted = true;
                    await _employeeCourseRepository.EditAsync(employeeCourse);
                    Console.WriteLine("[Debug] EmployeeCourse marked completed = true");
                }
                else
                {
                    Console.WriteLine("[Debug] Conditions not met for marking as completed");
                    Console.WriteLine($"[Debug] IsCompleted: {employeeCourse.IsCompleted}");

                }
            }



            return Ok(new { Message = "Submission uploaded successfully." });
        }









        [HttpGet("my-grade/{assignmentId}")]
     
        public async Task<IActionResult> GetMyAssignmentGrade([FromRoute]int assignmentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            
            var employee = _employeeRepo.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null)
                return Unauthorized("Employee not found.");

            
            var submission = (await _submissionRepo.GetAsync(
                s => s.AssignmentId == assignmentId && s.EmployeeId == employee.Id,
                includes: [s => s.Assignment]
            )).FirstOrDefault();

            if (submission is null)
                return NotFound("No submission found for this assignment.");

            return Ok(new
            {
                AssignmentTitle = submission.Assignment?.Title,
                SubmittedAt = submission.SubmittedAt,
                FileUrl = submission.FileUrl,
                Grade = submission.Grade,
                Status = submission.Grade.HasValue ? "Graded" : "Pending"
            });
        }



        //        [HttpPut("{assignmentId}")]
        //        [Consumes("multipart/form-data")]
        //        public async Task<IActionResult> UpdateSubmission([FromRoute] int assignmentId, [FromForm] EmployeeAssignmentSubmissionRequest request)
        //        {
        //            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //            var employee = _employeeRepo.GetOne(e => e.ApplicationUserId == userId);
        //            if (employee is null) return Unauthorized("Employee not found.");




        //            // i add this because if assignment deadline is finish he can not edit your submission
        //            var assignment = _assignmentRepository.GetOne(a => a.Id == assignmentId);
        //            if (assignment == null)
        //                return BadRequest("Invalid assignment.");

        //            if (assignment.Deadline < DateTime.UtcNow)
        //                return Ok("Submission deadline has passed. You cannot update this submission.");



        //            //var submission = _submissionRepo.GetOne(
        //            //    s => s.EmployeeId == employee.Id && s.AssignmentId == assignmentId
        //            //);

        //            //for progress
        //            var submission = _submissionRepo.GetOne(
        //    s => s.EmployeeId == employee.Id && s.AssignmentId == assignmentId,
        //    includes: [s => s.Assignment, s => s.Assignment.Lesson, s => s.Assignment.Lesson.Chapter]
        //);

        //            if (submission is null)
        //                return NotFound("No existing submission found to update.");








        //            if (request.File == null)
        //                return BadRequest("No new file provided.");

        //            var allowedExtensions = new[] { ".pdf", ".docx", ".doc", ".txt" };
        //            var ext = Path.GetExtension(request.File.FileName).ToLower();
        //            if (!allowedExtensions.Contains(ext))
        //                return BadRequest("Invalid file type.");

        //            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Submissions");
        //            Directory.CreateDirectory(folder);

        //            var newFileName = $"{Guid.NewGuid()}{ext}";
        //            var filePath = Path.Combine(folder, newFileName);
        //            using var stream = new FileStream(filePath, FileMode.Create);
        //            await request.File.CopyToAsync(stream);


        //            if (!string.IsNullOrEmpty(submission.FileUrl))
        //            {
        //                var oldFilePath = Path.Combine(folder, submission.FileUrl);
        //                if (System.IO.File.Exists(oldFilePath))
        //                    System.IO.File.Delete(oldFilePath);
        //            }

        //            submission.FileUrl = newFileName;
        //            submission.SubmittedAt = DateTime.UtcNow;
        //            submission.Grade = null; 

        //            await _submissionRepo.EditAsync(submission);

        //            // for progress
        //            //            await _progressService.UpdateEmployeeCourseProgress(
        //            //    employee.Id,
        //            //    submission.Assignment.Lesson.Chapter.CourseId
        //            //);

        //            // ✅ Update or create EmployeeAssignment record
        //            var employeeAssignment = _employeeAssignmentRepo.GetOne(
        //                ea => ea.EmployeeId == employee.Id && ea.AssignmentId == assignmentId
        //            );

        //            if (employeeAssignment == null)
        //            {
        //                var newEmployeeAssignment = new EmployeeAssignment
        //                {
        //                    EmployeeId = employee.Id,
        //                    AssignmentId = assignmentId,

        //                    CompletedAt = DateTime.UtcNow,
        //                    IsCompleted = true
        //                };

        //                await _employeeAssignmentRepo.CreateAsync(newEmployeeAssignment);
        //            }
        //            else
        //            {

        //                employeeAssignment.CompletedAt = DateTime.UtcNow;
        //                employeeAssignment.IsCompleted = true;

        //                await _employeeAssignmentRepo.EditAsync(employeeAssignment);
        //                //for progress
        //                await _progressService.UpdateEmployeeCourseProgress(
        //    employee.Id,
        //    submission.Assignment.Lesson.Chapter.CourseId
        //); var progress = _employeeCourseProgressRepository.GetOne(p =>
        //    p.EmployeeId == employee.Id &&
        //    p.CourseId == submission.Assignment.Lesson.Chapter.CourseId
        //);
        //                //for course completed or not
        //                if (progress is not null)
        //                {
        //                    var employeeCourse = _employeeCourseRepository.GetOne(ec =>
        //                        ec.EmployeeId == employee.Id &&
        //                        ec.CourseId == submission.Assignment.Lesson.Chapter.CourseId
        //                    );

        //                    if (employeeCourse is not null)
        //                    {
        //                        if (progress.TotalProgress >= 85 && !employeeCourse.IsCompleted&& employeeCourse.CompletedAt<DateTime.UtcNow)
        //                        {
        //                            employeeCourse.IsCompleted = true;

        //                            await _employeeCourseRepository.EditAsync(employeeCourse);
        //                        }
        //                        else if (progress.TotalProgress < 85 && employeeCourse.IsCompleted&&employeeCourse.CompletedAt < DateTime.UtcNow)
        //                        {
        //                            employeeCourse.IsCompleted = false;

        //                            await _employeeCourseRepository.EditAsync(employeeCourse);
        //                        }
        //                    }
        //                }
        //            }

        //            return Ok(new { Message = "Submission updated successfully." });
        //        }







        [HttpPut("{assignmentId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateSubmission([FromRoute] int assignmentId, [FromForm] EmployeeAssignmentSubmissionRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = _employeeRepo.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null) return Unauthorized("Employee not found.");

            var assignment = _assignmentRepository.GetOne(a => a.Id == assignmentId);
            if (assignment == null)
                return BadRequest("Invalid assignment.");

            if (assignment.Deadline < DateTime.UtcNow)
                return Ok("Submission deadline has passed. You cannot update this submission.");

            var submission = _submissionRepo.GetOne(
                s => s.EmployeeId == employee.Id && s.AssignmentId == assignmentId,
                includes: [s => s.Assignment, s => s.Assignment.Lesson, s => s.Assignment.Lesson.Chapter]
            );

            if (submission is null)
                return NotFound("No existing submission found to update.");

            if (request.File == null)
                return BadRequest("No new file provided.");

            var allowedExtensions = new[] { ".pdf", ".docx", ".doc", ".txt" };
            var ext = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Invalid file type.");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Submissions");
            Directory.CreateDirectory(folder);

            var newFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, newFileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.File.CopyToAsync(stream);

            if (!string.IsNullOrEmpty(submission.FileUrl))
            {
                var oldFilePath = Path.Combine(folder, submission.FileUrl);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            submission.FileUrl = newFileName;
            submission.SubmittedAt = DateTime.UtcNow;
            submission.Grade = null;

            await _submissionRepo.EditAsync(submission);

            // ✅ Update or create EmployeeAssignment
            var employeeAssignment = _employeeAssignmentRepo.GetOne(
                ea => ea.EmployeeId == employee.Id && ea.AssignmentId == assignmentId
            );

            if (employeeAssignment == null)
            {
                var newEmployeeAssignment = new EmployeeAssignment
                {
                    EmployeeId = employee.Id,
                    AssignmentId = assignmentId,
                    CompletedAt = DateTime.UtcNow,
                    IsCompleted = true
                };

                await _employeeAssignmentRepo.CreateAsync(newEmployeeAssignment);
            }
            else
            {
                employeeAssignment.CompletedAt = DateTime.UtcNow;
                employeeAssignment.IsCompleted = true;

                await _employeeAssignmentRepo.EditAsync(employeeAssignment);
            }

            // ✅ Update progress
            await _progressService.UpdateEmployeeCourseProgress(
                employee.Id,
                submission.Assignment.Lesson.Chapter.CourseId
            );

            var progress = _employeeCourseProgressRepository.GetOne(p =>
                p.EmployeeId == employee.Id &&
                p.CourseId == submission.Assignment.Lesson.Chapter.CourseId
            );

            var employeeCourse = _employeeCourseRepository.GetOne(ec =>
                ec.EmployeeId == employee.Id &&
                ec.CourseId == submission.Assignment.Lesson.Chapter.CourseId
            );

            // ✅ Only mark/unmark course completion if the course is still open
            if (employeeCourse is not null && employeeCourse.CompletedAt > DateTime.UtcNow)
            {
                if (progress is not null && progress.TotalProgress >= 85 && !employeeCourse.IsCompleted)
                {
                    employeeCourse.IsCompleted = true;
                    await _employeeCourseRepository.EditAsync(employeeCourse);
                }
                else if (progress is not null && progress.TotalProgress < 85 && employeeCourse.IsCompleted)
                {
                    employeeCourse.IsCompleted = false;
                    await _employeeCourseRepository.EditAsync(employeeCourse);
                }
            }

            return Ok(new { Message = "Submission updated successfully." });
        }




        //        [HttpDelete("{assignmentId}")]
        //        public async Task<IActionResult> DeleteSubmission([FromRoute] int assignmentId)
        //        {
        //            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //            var employee = _employeeRepo.GetOne(e => e.ApplicationUserId == userId);
        //            if (employee is null) return Unauthorized("Employee not found.");




        //            // i add this because if assignment deadline is finish he can not delete your submission
        //            var assignment = _assignmentRepository.GetOne(a => a.Id == assignmentId);
        //            if (assignment == null)
        //                return BadRequest("Invalid assignment.");

        //            if (assignment.Deadline < DateTime.UtcNow)
        //                return Ok("Submission deadline has passed. You cannot delete this submission.");



        //            //var submission = _submissionRepo.GetOne(
        //            //    s => s.EmployeeId == employee.Id && s.AssignmentId == assignmentId
        //            //);

        //            //for progress
        //            var submission = _submissionRepo.GetOne(
        //    s => s.EmployeeId == employee.Id && s.AssignmentId == assignmentId,
        //    includes: [s => s.Assignment, s => s.Assignment.Lesson, s => s.Assignment.Lesson.Chapter]
        //);

        //            if (submission is null)
        //                return NotFound("Submission not found.");








        //            if (!string.IsNullOrEmpty(submission.FileUrl))
        //            {
        //                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Submissions");
        //                var fullPath = Path.Combine(folder, submission.FileUrl);
        //                if (System.IO.File.Exists(fullPath))
        //                    System.IO.File.Delete(fullPath);
        //            }

        //            await _submissionRepo.DeleteAsync(submission);

        //            await _progressService.UpdateEmployeeCourseProgress(
        //       employee.Id,
        //       submission.Assignment.Lesson.Chapter.CourseId
        //   );

        //            var progress = _employeeCourseProgressRepository.GetOne(p =>
        //        p.EmployeeId == employee.Id && p.CourseId == submission.Assignment.Lesson.Chapter.CourseId);

        //            if (progress is not null && progress.TotalProgress >= 85)
        //            {
        //                var employeeCourse = _employeeCourseRepository.GetOne(ec =>
        //                    ec.EmployeeId == employee.Id && ec.CourseId == submission.Assignment.Lesson.Chapter.CourseId);

        //                if (employeeCourse is not null && !employeeCourse.IsCompleted&& employeeCourse.CompletedAt < DateTime.UtcNow)
        //                {
        //                    employeeCourse.IsCompleted = true;


        //                    await _employeeCourseRepository.EditAsync(employeeCourse);
        //                }
        //            }
        //            else if (progress is not null && progress.TotalProgress < 85)
        //            {
        //                var employeeCourse = _employeeCourseRepository.GetOne(ec =>
        //                    ec.EmployeeId == employee.Id && ec.CourseId == submission.Assignment.Lesson.Chapter.CourseId);

        //                if (employeeCourse is not null && employeeCourse.IsCompleted&& employeeCourse.CompletedAt < DateTime.UtcNow)
        //                {
        //                    employeeCourse.IsCompleted = false;


        //                    await _employeeCourseRepository.EditAsync(employeeCourse);
        //                }
        //            }

        //            //            await _progressService.UpdateEmployeeCourseProgress(
        //            //    employee.Id,
        //            //    submission.Assignment.Lesson.Chapter.CourseId
        //            //);


        //            //  Update or create EmployeeAssignment record
        //            var employeeAssignment = _employeeAssignmentRepo.GetOne(
        //                ea => ea.EmployeeId == employee.Id && ea.AssignmentId == assignmentId
        //            );

        //            if (employeeAssignment == null)
        //            {
        //                var newEmployeeAssignment = new EmployeeAssignment
        //                {
        //                    EmployeeId = employee.Id,
        //                    AssignmentId = assignmentId,

        //                    CompletedAt = DateTime.UtcNow,
        //                    IsCompleted = true
        //                };

        //                await _employeeAssignmentRepo.CreateAsync(newEmployeeAssignment);
        //            }
        //            else
        //            {

        //                employeeAssignment.CompletedAt = DateTime.UtcNow;
        //                employeeAssignment.IsCompleted = true;

        //                await _employeeAssignmentRepo.EditAsync(employeeAssignment);
        //            }
        //            return Ok(new { Message = "Submission deleted successfully." });
        //        }




        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> DeleteSubmission([FromRoute] int assignmentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employee = _employeeRepo.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null) return Unauthorized("Employee not found.");

            var assignment = _assignmentRepository.GetOne(a => a.Id == assignmentId);
            if (assignment == null)
                return BadRequest("Invalid assignment.");

            if (assignment.Deadline < DateTime.UtcNow)
                return Ok("Submission deadline has passed. You cannot delete this submission.");

            var submission = _submissionRepo.GetOne(
                s => s.EmployeeId == employee.Id && s.AssignmentId == assignmentId,
                includes: [s => s.Assignment, s => s.Assignment.Lesson, s => s.Assignment.Lesson.Chapter]
            );

            if (submission is null)
                return NotFound("Submission not found.");

            // Delete file if exists
            if (!string.IsNullOrEmpty(submission.FileUrl))
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Submissions");
                var fullPath = Path.Combine(folder, submission.FileUrl);
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            // Delete submission from DB
            await _submissionRepo.DeleteAsync(submission);

            // Update progress
            await _progressService.UpdateEmployeeCourseProgress(
                employee.Id,
                submission.Assignment.Lesson.Chapter.CourseId
            );

            var progress = _employeeCourseProgressRepository.GetOne(p =>
                p.EmployeeId == employee.Id &&
                p.CourseId == submission.Assignment.Lesson.Chapter.CourseId
            );

            var employeeCourse = _employeeCourseRepository.GetOne(ec =>
                ec.EmployeeId == employee.Id &&
                ec.CourseId == submission.Assignment.Lesson.Chapter.CourseId
            );

            // Mark/unmark course completion only if course is open (not locked)
            if (employeeCourse is not null && employeeCourse.CompletedAt > DateTime.UtcNow)
            {
                if (progress is not null && progress.TotalProgress >= 85 && !employeeCourse.IsCompleted)
                {
                    employeeCourse.IsCompleted = true;
                    await _employeeCourseRepository.EditAsync(employeeCourse);
                }
                else if (progress is not null && progress.TotalProgress < 85 && employeeCourse.IsCompleted)
                {
                    employeeCourse.IsCompleted = false;
                    await _employeeCourseRepository.EditAsync(employeeCourse);
                }
            }

            // Update EmployeeAssignment to not completed
            var employeeAssignment = _employeeAssignmentRepo.GetOne(
                ea => ea.EmployeeId == employee.Id && ea.AssignmentId == assignmentId
            );

            if (employeeAssignment != null)
            {
                employeeAssignment.IsCompleted = false;
                employeeAssignment.CompletedAt = null;
                await _employeeAssignmentRepo.EditAsync(employeeAssignment);
            }

            return Ok(new { Message = "Submission deleted successfully." });
        }


    }
}
