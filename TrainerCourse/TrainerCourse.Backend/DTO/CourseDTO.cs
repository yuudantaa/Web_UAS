using TrainerCourse.Backend.Models;

namespace TrainerCourse.Backend.DTO
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public string? CourseDescription { get; set; }
        public string CourseType { get; set; } = null!;
        public double Duration { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageFileName { get; set; }
        public DateTime Createdate { get; set; } = DateTime.Now;
        public TrainerDTO? Trainer { get; set; }

    }
}
