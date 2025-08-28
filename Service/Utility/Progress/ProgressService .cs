using DataAccess.IRepository;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Utility.Progress
{
    public class ProgressService:IProgressService
    {
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<EmployeeLessonProgress> _lessonProgressRepo;
        private readonly IRepository<EmployeeAssignment> _employeeAssignmentRepository;
        private readonly IRepository<Exam> _examRepository;
        private readonly IRepository<ExamSubmission> _examSubmissionRepo;
        private readonly IRepository<EmployeeCourseProgress> _courseProgressRepo;

        public ProgressService(
            IRepository<Lesson> lessonRepository,
            IRepository<Assignment> assignmentRepository,
            IRepository<EmployeeLessonProgress> lessonProgressRepo,
            IRepository<EmployeeAssignment> employeeAssignmentRepository,
            IRepository<Exam> examRepository,
            IRepository<ExamSubmission> examSubmissionRepo,
            IRepository<EmployeeCourseProgress> courseProgressRepo)
        {
            _lessonRepository = lessonRepository;
            _assignmentRepository = assignmentRepository;
            _lessonProgressRepo = lessonProgressRepo;
            _employeeAssignmentRepository = employeeAssignmentRepository;
            _examRepository = examRepository;
            _examSubmissionRepo = examSubmissionRepo;
            _courseProgressRepo = courseProgressRepo;
        }

        public async Task UpdateEmployeeCourseProgress(string employeeId, int courseId)
        {
            var totalLessons = (await _lessonRepository.GetAsync(l => l.Chapter.CourseId == courseId)).Count();
            var viewedLessons = (await _lessonProgressRepo.GetAsync(lp =>
                lp.EmployeeId == employeeId && lp.Lesson.Chapter.CourseId == courseId)).Count();

            var totalAssignments = (await _assignmentRepository.GetAsync(a => a.Lesson.Chapter.CourseId == courseId)).Count();
            var completedAssignments = (await _employeeAssignmentRepository.GetAsync(ea =>
                ea.EmployeeId == employeeId &&
                ea.Assignment.Lesson.Chapter.CourseId == courseId &&
                ea.IsCompleted)).Count();

            var totalExams = (await _examRepository.GetAsync(e => e.Chapter.CourseId == courseId)).Count();
            var submittedExams = (await _examSubmissionRepo.GetAsync(es =>
                es.EmployeeId == employeeId && es.Exam.Chapter.CourseId == courseId)).Count();

            decimal lessonProgress = totalLessons == 0 ? 0 : Math.Round(((decimal)viewedLessons / totalLessons) * 50, 2);
            decimal assignmentProgress = totalAssignments == 0 ? 0 : Math.Round(((decimal)completedAssignments / totalAssignments) * 25, 2);
            decimal examProgress = totalExams == 0 ? 0 : Math.Round(((decimal)submittedExams / totalExams) * 25, 2);

            var progressRecord = _courseProgressRepo.GetOne(p =>
                p.EmployeeId == employeeId && p.CourseId == courseId);

            if (progressRecord is null)
            {
                progressRecord = new EmployeeCourseProgress
                {
                    EmployeeId = employeeId,
                    CourseId = courseId,
                    LessonProgress = lessonProgress,
                    AssignmentProgress = assignmentProgress,
                    ExamProgress = examProgress,
                    LastUpdated = DateTime.UtcNow
                };

                await _courseProgressRepo.CreateAsync(progressRecord);
            }
            else
            {
                progressRecord.LessonProgress = lessonProgress;
                progressRecord.AssignmentProgress = assignmentProgress;
                progressRecord.ExamProgress = examProgress;
                progressRecord.LastUpdated = DateTime.UtcNow;

                await _courseProgressRepo.EditAsync(progressRecord);
            }
        }
    }
}
