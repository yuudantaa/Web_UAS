namespace TrainerCourse.Backend.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public string? CourseDescription { get; set; }
        public string CourseType { get; set; } = null!;
        public double Duration { get; set; }
        public int TrainerId { get; set; }
        public DateTime Createdate { get; set; } = DateTime.Now;
        public Trainer? Trainer { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageUrl
        {
            get
            {
                return string.IsNullOrEmpty(ImageFileName)
                    ? null
                    : $"/uploads/{ImageFileName}";
            }
        }
    }
}
