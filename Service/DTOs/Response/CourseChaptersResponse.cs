using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class CourseChaptersResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }

       // public int CourseId { get; set; }



        public int Order { get; set; }
    }
}
