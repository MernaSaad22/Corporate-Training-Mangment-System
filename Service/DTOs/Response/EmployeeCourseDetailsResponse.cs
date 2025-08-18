using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class EmployeeCourseDetailsResponse
    {
     public int CourseId { get; set; }
    public string Title { get; set; }
    public string InstructorName { get; set; }
   
    public string CategoryName { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
    public List<string> ChapterTitles { get; set; }  // Optional
    }
}
