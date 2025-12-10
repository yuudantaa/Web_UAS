using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainerCourse.Shared.Model
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseDescription { get; set; }
        public string CourseType { get; set; } = string.Empty;
        public double? Duration { get; set; }
        public int? TrainerId { get; set; }
        public DateTime Createdate { get; set; } = DateTime.Now;
        public Trainer? Trainer { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageFileName { get; set; }
    }
}
