namespace TrainerCourse.Backend.DTO
{
    public class CourseAddDTO
    {
        public string CourseName { get; set; } = null!;
        public string? CourseDescription { get; set; }
        public string CourseType { get; set; } = null!;
        public double Duration { get; set; }
        public int TrainerId { get; set; }
        public string? ImageFileName { get; set; }
    }
}
