using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Exam
    {
        public int Id { get; set; }

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
