using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
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
    public class EmployeeAssignmentsController : ControllerBase
    {
        private readonly IRepository<EmployeeAssignment> _employeeAssignmentRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<Instructor> _instructorRepository;

        public EmployeeAssignmentsController(IRepository<EmployeeAssignment> employeeAssignmentRepository, IRepository<Employee> employeeRepository,
            IRepository<Assignment> assignmentRepository, IRepository<Instructor> instructorRepository)
        {
            _employeeAssignmentRepository = employeeAssignmentRepository;
            this._employeeRepository = employeeRepository;
            this._assignmentRepository = assignmentRepository;
            _instructorRepository = instructorRepository;
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var employeeAssignments = await _employeeAssignmentRepository.GetAsync(
        //        includes: [ec => ec.Assignment, ec => ec.Employee],
        //        expression: ec => ec.Assignment.Instructor.ApplicationUserId == userId
        //    );

        //    return Ok(employeeAssignments.Adapt<List<EmployeeCourseResponse>>());
        //}

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
           
            var applicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(applicationUserId))
                return Unauthorized("User not authenticated.");

           
            var instructor = _instructorRepository.GetOne(i => i.ApplicationUserId == applicationUserId);
            if (instructor == null)
                return Ok("No instructor found for this user.");


            var employeeAssignments = await _employeeAssignmentRepository.GetAsync(
     expression: ea => ea.Assignment.InstructorId == instructor.Id,
     includes: new Expression<Func<EmployeeAssignment, object>>[]
     {
               ea => ea.Employee,
               ea => ea.Assignment,
               ea => ea.Assignment.Course
     });

            //       var response = employeeAssignments.Adapt<List<InstuctoeEmployeeAssignmentResponse>>();

            var response = employeeAssignments.Select(ea => new InstuctoeEmployeeAssignmentResponse
            {
                Id = ea.Id,
                EmployeeId = ea.EmployeeId,
                AssignmentId = ea.AssignmentId,
                AssignedAt = ea.AssignedAt,
               
                IsCompleted = ea.IsCompleted,
                AssignmentDeadline = ea.Assignment.Deadline  
            }).ToList();

            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var instructor =  _instructorRepository.GetOne(i => i.ApplicationUserId == userId);
            if (instructor == null)
                return Forbid("User is not an instructor.");

            var employeeAssignment = (await _employeeAssignmentRepository.GetAsync(
                expression: ea => ea.Id == id,
                includes: new Expression<Func<EmployeeAssignment, object>>[] { ea => ea.Assignment }
            )).FirstOrDefault();

            if (employeeAssignment == null)
                return NotFound("EmployeeAssignment not found.");

           
            if (employeeAssignment.Assignment.InstructorId != instructor.Id)
                return Forbid("You do not have permission to view this assignment.");

            var response = new InstuctoeEmployeeAssignmentResponse
            {
                Id = employeeAssignment.Id,
                EmployeeId = employeeAssignment.EmployeeId,
                AssignmentId = employeeAssignment.AssignmentId,
                AssignedAt = employeeAssignment.AssignedAt,
                IsCompleted = employeeAssignment.IsCompleted,
                AssignmentDeadline = employeeAssignment.Assignment?.Deadline
            };

            return Ok(response);
        }



        //[HttpPost("assign/{assignmentId}")]
        //public async Task<IActionResult> AssignEmployeesToAssignment(int assignmentId, [FromBody] AssignEmployeesRequest request)
        //{
        //    var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(instructorId))
        //        return Unauthorized("Instructor ID not found.");

        //    var assignment = await _assignmentRepository.GetAsync(
        //        expression: a => a.Id == assignmentId,
        //        includes: [a => a.Course]
        //    ).ContinueWith(t => t.Result.FirstOrDefault());

        //    if (assignment == null)
        //        return NotFound("Assignment not found.");

        //    if (assignment.Instructor.ApplicationUserId != instructorId)
        //        return Ok("You do not have permission to assign employees to this assignment.");

        //    var existingAssignments = await _employeeAssignmentRepository.GetAsync(
        //        expression: ea => ea.AssignmentId == assignmentId
        //    );

        //    var alreadyAssignedIds = existingAssignments
        //        .Select(ea => ea.EmployeeId)
        //        .ToHashSet();

        //    var newAssignments = request.EmployeeIds
        //        .Where(id => !alreadyAssignedIds.Contains(id))
        //        .Select(id => new EmployeeAssignment
        //        {
        //            AssignmentId = assignmentId,
        //            EmployeeId = id,
        //            AssignedAt = DateTime.UtcNow,
        //            IsCompleted = false
        //        }).ToList();

        //    foreach (var ea in newAssignments)
        //    {
        //        await _employeeAssignmentRepository.CreateAsync(ea);
        //    }

        //    return Ok(new
        //    {
        //        Message = "Employees assigned successfully.",
        //        AssignedCount = newAssignments.Count
        //    });
        //}

        [HttpPost("assign/{assignmentId}")]
        public async Task<IActionResult> AssignEmployeesToAssignment(int assignmentId, [FromBody] AssignEmployeesRequest request)
        {
            var applicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(applicationUserId))
                return Unauthorized("User not authenticated.");

          
            var instructor = _instructorRepository.GetOne(i => i.ApplicationUserId == applicationUserId);
            if (instructor == null)
                return Forbid("No instructor found for this user.");

            var assignment = (await _assignmentRepository.GetAsync(
                expression: a => a.Id == assignmentId,
                includes: [a => a.Course]
            )).FirstOrDefault();

            if (assignment == null)
                return NotFound("Assignment not found.");

          
            if (assignment.InstructorId != instructor.Id)
                return Forbid("You do not have permission to assign employees to this assignment.");

         
            var employees = await _employeeRepository.GetAsync(e => request.EmployeeIds.Contains(e.Id));
            if (employees.Count() != request.EmployeeIds.Count)
                return BadRequest("One or more employees not found.");

           
            var existingAssignments = await _employeeAssignmentRepository.GetAsync(ea =>
                ea.AssignmentId == assignmentId && request.EmployeeIds.Contains(ea.EmployeeId)
            );

            var alreadyAssignedEmployeeIds = existingAssignments.Select(ea => ea.EmployeeId).ToHashSet();

            var newAssignments = new List<EmployeeAssignment>();
            foreach (var employeeId in request.EmployeeIds)
            {
                if (!alreadyAssignedEmployeeIds.Contains(employeeId))
                {
                    newAssignments.Add(new EmployeeAssignment
                    {
                        AssignmentId = assignmentId,
                        EmployeeId = employeeId,
                        AssignedAt = DateTime.UtcNow,
                        IsCompleted = false
                    });
                }
            }

            foreach (var newAssignment in newAssignments)
            {
                await _employeeAssignmentRepository.CreateAsync(newAssignment);
            }

            return Ok(new { Message = "Employees assigned successfully.", AssignedCount = newAssignments.Count });
        }




        [HttpPatch("{employeeAssignmentId}")]
        public async Task<IActionResult> EditEmployeeAssignment(int employeeAssignmentId, [FromBody] EditEmployeeAssignmentRequest request)
        {
            var employeeAssignment =  _employeeAssignmentRepository.GetOne(ea => ea.Id == employeeAssignmentId);
            if (employeeAssignment == null)
                return NotFound("Employee assignment not found.");


            if (!string.IsNullOrEmpty(request.EmployeeId) && request.EmployeeId != employeeAssignment.EmployeeId)
            {
                var newEmployee =  _employeeRepository.GetOne(e => e.Id == request.EmployeeId);
                if (newEmployee == null)
                    return BadRequest("New employee not found.");

               
                var exists = _employeeAssignmentRepository.GetOne(ea => ea.AssignmentId == employeeAssignment.AssignmentId && ea.EmployeeId == request.EmployeeId);
                if (exists != null)
                    return BadRequest("This employee is already assigned to this assignment.");

                employeeAssignment.EmployeeId = request.EmployeeId;
            }

          
            if (request.IsCompleted.HasValue)
                employeeAssignment.IsCompleted = request.IsCompleted.Value;

            if (request.CompletedAt.HasValue)
                employeeAssignment.CompletedAt = request.CompletedAt.Value;

            await _employeeAssignmentRepository.EditAsync(employeeAssignment);

            return Ok(new { Message = "Employee assignment updated successfully." });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAssignment([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var instructor = _instructorRepository.GetOne(i => i.ApplicationUserId == userId);
            if (instructor == null)
                return Forbid("User is not an instructor.");


            var employeeAssignment = (await _employeeAssignmentRepository.GetAsync(
                ea => ea.Id == id,
                includes: new Expression<Func<EmployeeAssignment, object>>[] { ea => ea.Assignment }
            )).FirstOrDefault();

            if (employeeAssignment == null)
                return NotFound("Employee assignment not found.");


            if (employeeAssignment.Assignment.InstructorId != instructor.Id)
                return Ok("You do not have permission to delete this assignment.");


            await _employeeAssignmentRepository.DeleteAsync(employeeAssignment);

            return NoContent();

        }

        // i want when instrucror select a specified employee view which assignment assign for him and if he complete it or not

        [HttpGet("assignments/{employeeId}")]
        public async Task<IActionResult> GetEmployeeAssignments(string employeeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var instructor = _instructorRepository.GetOne(i => i.ApplicationUserId == userId);
            if (instructor == null)
                return Unauthorized();

            
            var employeeAssignments = await _employeeAssignmentRepository.GetAsync(
                ea => ea.EmployeeId == employeeId && ea.Assignment.Course.InstructorId == instructor.Id,
                includes: new Expression<Func<EmployeeAssignment, object>>[]
                {
            ea => ea.Assignment,
            ea => ea.Assignment.Course
                });

            if (!employeeAssignments.Any())
                return NotFound("No assignments found for this employee under your instruction.");

            var response = employeeAssignments.Select(ea => new EmployeeAssignmentDetailResponse
            {
                AssignmentId = ea.AssignmentId,
                AssignmentTitle = ea.Assignment.Title,
                AssignedAt = ea.AssignedAt,
                IsCompleted = ea.IsCompleted,
                CompletedAt = ea.CompletedAt,
                Deadline = ea.Assignment.Deadline  
            });

            return Ok(response);
        }





        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var employeeCourse = (await _employeeCourseRepository.GetAsync(
        //        expression: ec => ec.Id == id,
        //        includes: [ec => ec.Course.Company]
        //    )).FirstOrDefault();

        //    if (employeeCourse == null)
        //        return NotFound("EmployeeCourse not found.");

        //    if (employeeCourse.Course.Company.ApplicationUserId != userId)
        //        return Forbid();

        //    return Ok(employeeCourse.Adapt<EmployeeCourseResponse>());
        //}

        //// company can not add mor than the max employee or mx coyrses in it's plan but it can assign the same employee to more than one course 
        ////like we have plan contain of 2 course and 3 employess ==>assign 2 employees for the first cours
        ////==>assign 1 from the first for the second course else 
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] AssignCourseRequest request)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized("User ID not found in token.");

        //    var course = (await _courseRepository.GetAsync(
        //        expression: c => c.Id == request.CourseId,
        //        includes: [c => c.Company]
        //    )).FirstOrDefault();

        //    if (course is null)
        //        return NotFound("Course not found.");

        //    var employee = _employeeRepository.GetOne(e => e.Id == request.EmployeeId);
        //    if (employee is null)
        //        return NotFound("Employee not found.");

        //    if (course.Company?.ApplicationUserId != userId)
        //        return Forbid("Only the company admin can assign students to this course.");

        //    var alreadyAssigned = (await _employeeCourseRepository.GetAsync(
        //        ec => ec.CourseId == request.CourseId && ec.EmployeeId == request.EmployeeId
        //    )).Any();

        //    if (alreadyAssigned)
        //        return BadRequest("Employee already assigned to this course.");

        //    var employeeCourse = new EmployeeCourse
        //    {
        //        CourseId = request.CourseId,
        //        EmployeeId = request.EmployeeId,
        //        AssignedAt = DateTime.UtcNow,
        //        CompletedAt = DateTime.UtcNow.AddDays(request.DeadlineDays),
        //        IsCompleted = false
        //    };

        //    var newEmployeeCourse = await _employeeCourseRepository.CreateAsync(employeeCourse);
        //    return Created(
        //            $"{Request.Scheme}://{Request.Host}/api/CompanyAdmin/EmployeeCourses/{newEmployeeCourse.Id}",
        //            newEmployeeCourse.Adapt<EmployeeCourseResponse>()
        //        );
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit(int id, [FromBody] UpdateCourseEmployeeRequest request)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var employeeCourse = (await _employeeCourseRepository.GetAsync(
        //        ec => ec.Id == id,
        //        includes: [ec => ec.Course.Company]
        //    )).FirstOrDefault();

        //    if (employeeCourse == null)
        //        return NotFound("Not found.");

        //    if (employeeCourse.Course.Company.ApplicationUserId != userId)
        //        return Forbid();

        //    employeeCourse.IsCompleted = request.IsCompleted;
        //    employeeCourse.CompletedAt = DateTime.UtcNow.AddDays(request.DeadlineDays);

        //    var updated = await _employeeCourseRepository.EditAsync(employeeCourse);
        //    return Ok(updated.Adapt<EmployeeCourseResponse>());
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var employeeCourse = (await _employeeCourseRepository.GetAsync(
        //        ec => ec.Id == id,
        //        includes: [ec => ec.Course.Company]
        //    )).FirstOrDefault();

        //    if (employeeCourse == null)
        //        return NotFound("Not found.");

        //    if (employeeCourse.Course.Company.ApplicationUserId != userId)
        //        return Forbid();

        //    await _employeeCourseRepository.DeleteAsync(employeeCourse);
        //    return NoContent();
        //}


    }
}
