using System.Linq.Expressions;
using System.Security.Claims;
using Corporate_Training_Mangment_System.Controllers.EmployeeCompany;
using DataAccess.IRepository;
using DataAccess.Repository;
using Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.DTOs.Request;
using Service.DTOs.Response;
using Service.Utility.Progress;

namespace Corporate_Training_Mangment_System.Controllers.EmployeesCompany.Controllers
{

    [Area("EmployeesCompany")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    // [Area("EmployeesCompany")]
    //[Route("api/EmployeesCompany")]
    public class EmployeesController : Controller
    {
        private readonly IRepository<EmployeeCourse> _employeeCourseRepo;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<EmployeeAssignment> _employeeAssignmentRepository;
        private readonly IRepository<Exam> _examRepository;
       
        private readonly IRepository<ExamSubmission> _examSubmissionRepo;
        private readonly IRepository<EmployeeCourseProgress> _courseProgressRepo;
        private readonly IRepository<EmployeeLessonProgress> _lessonProgressRepo;
        private readonly IProgressService _progressService;
        private readonly IRepository<EmployeeCourse> _employeecourseRepository;

        public EmployeesController(IRepository<EmployeeCourse> employeeCourseRepo, IRepository<Chapter> chapterRepository, IEmployeeRepository employeeRepository
            , IRepository<Course> courseRepository, IRepository<Lesson> lessonRepository,IRepository<Assignment> assignmentRepository, IRepository<EmployeeAssignment> employeeAssignmentRepository
            , IRepository<Exam> examRepository, IRepository<ExamSubmission> examSubmissionRepo, IRepository<EmployeeCourseProgress> courseProgressRepo,
    IRepository<EmployeeLessonProgress> lessonProgressRepo, IProgressService progressService, IRepository<EmployeeCourse>employeecourseRepository)
        {
            _employeeCourseRepo = employeeCourseRepo;
            _chapterRepository = chapterRepository;
            _employeeRepository = employeeRepository;
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
            _assignmentRepository = assignmentRepository;
            _employeeAssignmentRepository = employeeAssignmentRepository;
            _examRepository = examRepository;
            _examSubmissionRepo = examSubmissionRepo;
            _courseProgressRepo = courseProgressRepo;
            _lessonProgressRepo = lessonProgressRepo;
            _progressService = progressService;
            _employeecourseRepository = employeecourseRepository;
        }
        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var courses = await _employeeCourseRepo.GetAsync(
                includes: [e => e.Course],
                expression: e => e.Employee.ApplicationUserId == userId
            );

            var result = courses.Select(e => new EmployeeCourseShortResponse
            {
                CourseId = e.Course.Id,
                Title = e.Course.Title,
                IsCompleted = e.IsCompleted,
                AssignedAt = e.AssignedAt
            });

            return Ok(result);
        }

   //when i try to get all chapters
        //[HttpGet("All-chapters/{id}")]
        //public async Task<IActionResult> GetAllChapters([FromRoute] int id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized();

        //    var chapters = await _chapterRepository.GetAsync(
        //        includes: [e => e.CourseId],
        //        expression: e => e.CourseId == id
        //    );

        //    var result = chapters.Select(e => new CourseChaptersResponse
        //    {
        //        CourseId = e.Course.Id,
        //        Title = e.Course.Title,
        //        Order = e.Order,

        //    });

        //    return Ok(result);
        //}



        //[HttpGet("All-chapters/{id}")]
        //public async Task<IActionResult> GetAllChapters([FromRoute] int id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized();

        //    var chapters = await _chapterRepository.GetAsync(
        //        includes: [c => c.Course],
        //        expression: c => c.CourseId == id &&
        //                         c.Course.EmployeeCourses.Any(ec => ec.Employee.ApplicationUserId == userId)
        //    );

        //    var result = chapters.Select(c => new CourseChaptersResponse
        //    {
        //        Id = c.Id,
        //        Title = c.Title,
        //        CourseId = c.CourseId,
        //        Order = c.Order

        //    }).ToList();

        //    return Ok(result);
        //}


 


        [HttpGet("{courseId}/chapters")]
        public async Task<ActionResult<IEnumerable<CourseChaptersResponse>>> GetChaptersForCourse(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            // check for employee
            var employee = _employeeRepository.GetOne(i => i.ApplicationUserId == userId);
            if (employee is null) return NotFound("Employee not found");


          
            var employeeCourse = (await _employeeCourseRepo.GetAsync(
    ec => ec.CourseId == courseId && ec.EmployeeId == employee.Id
)).FirstOrDefault();
            if (employeeCourse is null)
                return NotFound("Course not found or does not belong to your courses.");

            var chapters = await _chapterRepository.GetAsync(
                c => c.CourseId == courseId
            );
            var orderedChapters = chapters.OrderBy(c => c.Order);


            return Ok(orderedChapters.Adapt<IEnumerable<CourseChaptersResponse>>());
        }








        [HttpGet("my-course/{id}")]
        public IActionResult GetMyCourse([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var course = _employeeCourseRepo.GetOne(
                expression: e => e.Course.Id == id && e.Employee.ApplicationUserId == userId,
                includes: [
                    e => e.Course,
            e => e.Course.Instructor.ApplicationUser,
            e => e.Course.Category,
            e => e.Course.Company,
            e => e.Course.Chapters
                ]
            );

            if (course is null)
                return NotFound();

            var result = new EmployeeCourseDetailsResponse
            {
                CourseId = course.Course.Id,
                Title = course.Course.Title,
                InstructorName = course.Course.Instructor?.ApplicationUser?.UserName,

                CategoryName = course.Course.Category?.Name,
                AssignedAt = course.AssignedAt,
                CompletedAt = course.CompletedAt,
                IsCompleted = course.IsCompleted,
                ChapterTitles = course.Course.Chapters?.Select(c => c.Title).ToList()
            };

            return Ok(result);
        }


        //i have problem when i use adapt 

        //[HttpGet("SelectedCourse/{id}")]
        //public async Task<ActionResult<EmployeeCourseDetailsResponse>> GetOne(int id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (userId is null) return Unauthorized();

        //    var employee = _employeeRepository.GetOne(i => i.ApplicationUserId == userId);
        //    if (employee is null) return Unauthorized();


        //    var course = (await _employeeCourseRepo.GetAsync(
        //        c => c.CourseId == id && c.EmployeeId == employee.Id
        //    )).FirstOrDefault();

        //    if (course is null)
        //        return NotFound("Course not found or does not belong to this employee.");

        //    return Ok(course.Adapt<EmployeeCourseDetailsResponse>());
        //}


        // i want to view all lessons in a specified chapter
        [HttpGet("AllLessonsof_chapter/{chapterId}")]
        public async Task<ActionResult<IEnumerable<AllLessonsInChapterResponse>>> GetByChapter([FromRoute] int chapterId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var chapter = _chapterRepository.GetOne(
                c => c.Id == chapterId ,
                includes: [c => c.Course]);

            if (chapter is null)
                return NotFound("Chapter not found or not available for  you.");

            var lessons = await _lessonRepository.GetAsync(
                l => l.ChapterId == chapterId,
                includes: [l => l.Chapter]);

            return Ok(lessons.Adapt<IEnumerable<AllLessonsInChapterResponse>>());
        }


        //[HttpGet("OneLessonsof_chapter/{LessonId}")]
        //public ActionResult<IEnumerable<AllLessonsInChapterResponse>> GetLesson(int lessonId)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (userId is null) return Unauthorized();

        //    var lesson = _lessonRepository.GetOne(
        //        c => c.Id == lessonId,
        //        includes: [c => c.Chapter]);

        //    if (lesson is null)
        //        return NotFound("Lesson not found or not available for  you.");



        //    return Ok(lesson.Adapt<IEnumerable<AllLessonsInChapterResponse>>());
        //}



        [HttpGet("lesson-details/{lessonId}")]
        public async Task <ActionResult<LessonInChapter> >GetLesson([FromRoute] int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var lesson = _lessonRepository.GetOne(
                c => c.Id == lessonId,
                includes: [c => c.Chapter, c => c.Chapter.Course]);

            if (lesson is null)
                return NotFound("Lesson not found or not available to you.");

          
            var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null) return Unauthorized();


            var employeeCourses = await _employeeCourseRepo.GetAsync(
    ec => ec.EmployeeId == employee.Id && ec.CourseId == lesson.Chapter.CourseId
);

            if (!employeeCourses.Any())
                return Forbid("You are not enrolled in this course.");


            // Record lesson view progress (if not already viewed)
            var alreadyViewed = _lessonProgressRepo.GetOne(lp =>
                lp.EmployeeId == employee.Id && lp.LessonId == lesson.Id);

            if (alreadyViewed is null)
            {
                await _lessonProgressRepo.CreateAsync(new EmployeeLessonProgress
                {
                    EmployeeId = employee.Id,
                    LessonId = lesson.Id,
                    ViewedAt = DateTime.UtcNow
                });
            }

            await _progressService.UpdateEmployeeCourseProgress(employee.Id, lesson.Chapter.CourseId);



            //if employee progress >=85% mark course as completed for this employee and CompletedAt<now this is the deadline for course created by company ==>EmployeeCourse 
            var progress = _courseProgressRepo.GetOne(p =>
    p.EmployeeId == employee.Id && p.CourseId == lesson.Chapter.CourseId);

            if (progress is not null && progress.TotalProgress >= 85)
            {
                var employeeCourse = _employeeCourseRepo.GetOne(ec =>
                    ec.EmployeeId == employee.Id && ec.CourseId == lesson.Chapter.CourseId);

                if (employeeCourse is not null && !employeeCourse.IsCompleted && employeeCourse.CompletedAt< DateTime.UtcNow)
                {
                    employeeCourse.IsCompleted = true;
                   

                    await _employeeCourseRepo.EditAsync(employeeCourse);
                }
            }

            var response = lesson.Adapt<LessonInChapter>(); 
            return Ok(response);
        }


        [HttpGet("lesson/{lessonId}/assignments")]
        public async Task<ActionResult<IEnumerable<AssignmentsInLessonResponse>>> GetAssignmentsByLesson([FromRoute] int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null) return Unauthorized();

            
            var lesson = _lessonRepository.GetOne(
                l => l.Id == lessonId,
                includes: [l => l.Chapter, l => l.Chapter.Course]);

            if (lesson is null)
                return NotFound("Lesson not found.");

   
            var employeeCourses = await _employeeCourseRepo.GetAsync(
                ec => ec.EmployeeId == employee.Id && ec.CourseId == lesson.Chapter.CourseId
            );

            if (!employeeCourses.Any())
                return Ok("You are not enrolled in the course containing this lesson.");

       
            var assignments = await _assignmentRepository.GetAsync(
                a => a.LessonId == lessonId
            );

            return Ok(assignments.Adapt<IEnumerable<AssignmentsInLessonResponse>>());
        }


        [HttpGet("assignment/{assignmentId}")]
        public async Task<ActionResult<IEnumerable<AssignmentsInLessonResponse>>> GetAssignment([FromRoute] int assignmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null) return Unauthorized();


            var assignment = _assignmentRepository.GetOne(
                l => l.Id == assignmentId,
                includes: [l => l.Lesson, l => l.Lesson.Chapter, l => l.Lesson.Chapter.Course]);

            if (assignment is null)
                return NotFound("Assignment not found.");


            var employeeCourses = await _employeeCourseRepo.GetAsync(
                ec => ec.EmployeeId == employee.Id && ec.CourseId == assignment.Lesson.Chapter.CourseId
            );

            if (!employeeCourses.Any())
                return Ok("You are not enrolled in the course containing this Assignment.");

            var response = new AssignmentsInLessonResponse
            {
                Id = assignment.Id,
                Title = assignment.Title,
                Description = assignment.Description,
                FileUrl = assignment.FileUrl,
                Deadline = assignment.Deadline,
               
                LessonTitle = assignment.Lesson.Title
            };

            return Ok(response);


            //return Ok(assignment.Adapt<IEnumerable<AssignmentsInLessonResponse>>());
        }


        // i want to enable employee see all assignments belong to him 
      
        [HttpGet("assignments")]
        public async Task<ActionResult<IEnumerable<EmployeeAssignmentResponse>>> GetMyAssignments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null) return Unauthorized();

            
            var employeeAssignments = await _employeeAssignmentRepository.GetAsync(
                ea => ea.EmployeeId == employee.Id,
                includes: [ea => ea.Assignment, ea => ea.Assignment.Lesson]
            );

            var result = employeeAssignments.Select(ea => new EmployeeAssignmentResponse
            {
                AssignmentId = ea.AssignmentId,
                Title = ea.Assignment.Title,
                Description = ea.Assignment.Description,
                FileUrl = ea.Assignment.FileUrl,
                Deadline = ea.Assignment.Deadline,
                AssignedAt = ea.AssignedAt,
                CompletedAt = ea.CompletedAt,
                IsCompleted = ea.IsCompleted,
                LessonId = ea.Assignment.LessonId,
                LessonTitle = ea.Assignment.Lesson.Title
            });

            return Ok(result);
        }




        // i have a problem questions not be counted 
        //[HttpGet("examforchapter/{chapterId}")]
        //public async Task<IActionResult> GetExamsByChapter(int chapterId)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (userId == null)
        //        return Unauthorized();

        //    var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
        //    if (employee == null)
        //        return NotFound("Employee not found");

        //    // Optionally: Check if this employee is allowed to see exams of this chapter (e.g. enrolled in the course)

        //    var exams = await _examRepository.GetAsync(e => e.ChapterId == chapterId);

        //    var examResponses = exams.Adapt<IEnumerable<ExamInChapterResponse>>(); // use Mapster or any mapper

        //    return Ok(examResponses);
        //}



        [HttpGet("examforchapter/{chapterId}")]
        public async Task<IActionResult> GetExamsByChapter([FromRoute] int chapterId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
            if (employee == null) return NotFound("Employee not found");

            var exams = await _examRepository.GetAsync(
                e => e.ChapterId == chapterId,
                includes: new Expression<Func<Exam, object>>[] { e => e.Questions });  // <-- Include questions here because i want to count question i should load it 

            var examResponses = exams.Select(e => new ExamInChapterResponse
            {
                Id = e.Id,
                Title = e.Title,
                Deadline = e.Deadline,
                TotalQuestions = e.Questions?.Count ?? 0
            });

            return Ok(examResponses);
        }


        //[HttpGet("exam/{examid}")] 
        //public IActionResult GetExamByIdForEmployee([FromRoute] int examid)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized();

        //    var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
        //    if (employee == null)
        //        return NotFound("Employee not found");


        //    var exam = _examRepository.GetOne(
        //        e => e.Id == examid,
        //        includes: new Expression<Func<Exam, object>>[] { e => e.Questions, e => e.Chapter, e => e.Chapter.Course });

        //    if (exam == null)
        //        return NotFound("Exam not found");

        //    if (exam.Deadline < DateTime.UtcNow)
        //    {
        //        return Ok("The exam deadline has passed. You can no longer view the exam.");
        //    }

        //    var response = new ExamEmployeeResponse
        //    {
        //        Id = exam.Id,
        //        Title = exam.Title,
        //        Deadline = exam.Deadline,
        //        TotalQuestions = exam.Questions?.Count ?? 0,
        //        Questions = exam.Questions.Select(q => new QuestionEmployeeResponse
        //        {
        //            Id = q.Id,
        //            Text = q.Text,
        //            OptionA = q.OptionA,
        //            OptionB = q.OptionB,
        //            OptionC = q.OptionC,
        //            OptionD = q.OptionD
        //        }).ToList()
        //    };

        //    return Ok(response);
        //}




        [HttpGet("exam/{examid}")]
        public IActionResult GetExamByIdForEmployee([FromRoute] int examid)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
            if (employee == null)
                return NotFound("Employee not found");

            var exam = _examRepository.GetOne(
                e => e.Id == examid,
                includes: new Expression<Func<Exam, object>>[] { e => e.Questions, e => e.Chapter, e => e.Chapter.Course });

            if (exam == null)
                return NotFound("Exam not found");

            // ✅ Constrain: Ensure employee is enrolled in the course containing this exam
            with:

            var enrollment = _employeecourseRepository.GetOne(ec =>
                ec.EmployeeId == employee.Id &&
                ec.CourseId == exam.Chapter.CourseId);

            if (enrollment == null)
            {
                return Ok("You are not enrolled in the course for this exam.");
            }

            if (exam.Deadline < DateTime.UtcNow)
            {
                return Ok("The exam deadline has passed. You can no longer view the exam.");
            }

            var response = new ExamEmployeeResponse
            {
                Id = exam.Id,
                Title = exam.Title,
                Deadline = exam.Deadline,
                TotalQuestions = exam.Questions?.Count ?? 0,
                Questions = exam.Questions.Select(q => new QuestionEmployeeResponse
                {
                    Id = q.Id,
                    Text = q.Text,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD
                }).ToList()
            };

            return Ok(response);
        }




        //[HttpGet("EmployeeStats/{employeeId}")]
        //public async Task<IActionResult> GetEmployeeStats(string employeeId)
        //{
        //    var employeeCourses = await _employeeCourseRepo.GetAsync(
        //        expression: ec => ec.EmployeeId == employeeId
        //    );

        //    var enrolled = employeeCourses.Count();
        //    var completed = employeeCourses.Count(ec => ec.IsCompleted);
        //    var points = completed * 10;

        //    var result = new EmployeeCourseDtoResponse
        //    {
        //        CoursesEnrolled = enrolled,
        //        CoursesCompleted = completed,
        //        PointsEarned = points
        //    };

        //    return Ok(result);
        //}
        ///



        //MY WORKED CODE

        [HttpPost("submit-exam/{examId}")]
        public async Task<IActionResult> SubmitExam([FromRoute] int examId, [FromBody] ExamSubmissionRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null) return Unauthorized();


            var existingSubmission = _examSubmissionRepo.GetOne(
                es => es.ExamId == examId && es.EmployeeId == employee.Id
            );

            if (existingSubmission != null)
                return BadRequest("You have already submitted this exam.");


            var exam = _examRepository.GetOne(
      e => e.Id == examId,
      includes: [e => e.Questions, e => e.Chapter] // ✅ FIXED
  );
           


            if (exam == null)
                return NotFound("Exam not found");

            if (exam.Chapter == null)
                return Ok("Exam is not linked to a chapter.");


            int correctCount = 0;
            var questionAnswers = new List<QuestionAnswer>();

            foreach (var submitted in request.Answers)
            {
                var question = exam.Questions.FirstOrDefault(q => q.Id == submitted.QuestionId);
                if (question == null) continue;

                var isCorrect = string.Equals(
                    submitted.SelectedAnswer?.Trim(),
                    question.Answer?.Trim(),
                    StringComparison.OrdinalIgnoreCase
                );

                if (isCorrect) correctCount++;

                questionAnswers.Add(new QuestionAnswer
                {
                    QuestionId = question.Id,
                    Answer = submitted.SelectedAnswer,
                });
            }

            var submission = new ExamSubmission
            {
                ExamId = examId,
                EmployeeId = employee.Id,
                SubmittedAt = DateTime.UtcNow,
                Grade = Math.Round((decimal)correctCount / exam.Questions.Count * 100, 2), // Grade as percentage
                QuestionAnswers = questionAnswers
            };

            await _examSubmissionRepo.CreateAsync(submission);

            await _progressService.UpdateEmployeeCourseProgress(employee.Id, exam.Chapter.CourseId);
            return Ok(new
            {
                Message = "Exam submitted successfully.",
                Grade = submission.Grade,
                TotalQuestions = exam.Questions.Count,
                CorrectAnswers = correctCount
            });
        }



        //    [HttpPost("submit-exam/{examId}")]
        //    public async Task<IActionResult> SubmitExam([FromRoute] int examId, [FromBody] ExamSubmissionRequest request)
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (userId is null) return Unauthorized();

        //        var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
        //        if (employee is null) return Unauthorized();


        //        var existingSubmission =  _examSubmissionRepo.GetOne(
        //            es => es.ExamId == examId && es.EmployeeId == employee.Id
        //        );

        //        if (existingSubmission != null)
        //            return BadRequest("You have already submitted this exam.");


        //        var exam = _examRepository.GetOne(
        //            e => e.Id == examId,
        //            includes: [e => e.Questions]
        //        );

        //        if (exam == null)
        //            return NotFound("Exam not found");
        //        //
        //        if (exam.Deadline < DateTime.UtcNow)
        //        {
        //            return Ok("The exam deadline has passed. You can no longer submit your answers.");
        //        }

        //        int correctCount = 0;
        //        var questionAnswers = new List<QuestionAnswer>();

        //        foreach (var submitted in request.Answers)
        //        {
        //            var question = exam.Questions.FirstOrDefault(q => q.Id == submitted.QuestionId);
        //            if (question == null) continue; 

        //            var isCorrect = string.Equals(
        //                submitted.SelectedAnswer?.Trim(),
        //                question.Answer?.Trim(),
        //                StringComparison.OrdinalIgnoreCase
        //            );

        //            if (isCorrect) correctCount++;

        //            questionAnswers.Add(new QuestionAnswer
        //            {
        //                QuestionId = question.Id,
        //               Answer = submitted.SelectedAnswer,
        //            });
        //        }

        //        var submission = new ExamSubmission
        //        {
        //            ExamId = examId,
        //            EmployeeId = employee.Id,
        //            SubmittedAt = DateTime.UtcNow,
        //            Grade = Math.Round((decimal)correctCount / exam.Questions.Count * 100, 2), // Grade as percentage
        //            QuestionAnswers = questionAnswers
        //        };

        //        await _examSubmissionRepo.CreateAsync(submission);

        //        await _progressService.UpdateEmployeeCourseProgress(employee.Id, exam.Chapter.CourseId);

        //        //if employee progress >=85% mark course as completed for this employee ==>EmployeeCourse 
        //        var progress = _courseProgressRepo.GetOne(p =>
        //p.EmployeeId == employee.Id && p.CourseId == exam.Chapter.CourseId);

        //        if (progress is not null && progress.TotalProgress >= 85)
        //        {
        //            var employeeCourse = _employeeCourseRepo.GetOne(ec =>
        //                ec.EmployeeId == employee.Id && ec.CourseId == exam.Chapter.CourseId);

        //            if (!employeeCourse.IsCompleted && employeeCourse.CompletedAt > DateTime.UtcNow)
        //            {
        //                employeeCourse.IsCompleted = true;


        //                await _employeeCourseRepo.EditAsync(employeeCourse);
        //            }
        //        }


        //        return Ok(new
        //        {
        //            Message = "Exam submitted successfully.",
        //            Grade = submission.Grade,
        //            TotalQuestions = exam.Questions.Count,
        //            CorrectAnswers = correctCount
        //        });
        //    }




        [HttpGet("my-course-progress/{courseId}")]
        public IActionResult GetCourseProgress([FromRoute] int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var employee = _employeeRepository.GetOne(e => e.ApplicationUserId == userId);
            if (employee is null) return NotFound();

            var progress = _courseProgressRepo.GetOne(p =>
                p.EmployeeId == employee.Id && p.CourseId == courseId,
                includes: [p => p.Course]);

            if (progress is null) return NotFound("Progress not tracked yet.");

            return Ok(new
            {
                CourseTitle = progress.Course?.Title,
                LessonProgress = progress.LessonProgress,
                AssignmentProgress = progress.AssignmentProgress,
                ExamProgress = progress.ExamProgress,
                TotalProgress = progress.TotalProgress,
                LastUpdated = progress.LastUpdated
            });
        }





    }


}
