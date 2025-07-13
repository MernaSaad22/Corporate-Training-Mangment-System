using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Response
{
    public class InstructorResponse
    {
        public string Id { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string FullName { get; set; } = null!; //UserName

        public string ApplicationUserId { get; set; } = null!;
        public string CompanyId { get; set; } = null!;
    }
}
