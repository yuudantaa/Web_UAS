using TrainerCourse.Backend.Models;

namespace TrainerCourse.Backend.Data
{
    public interface ICourse
    {
        IEnumerable<Course> GetCourses();
        Course GetCourseById(int CourseId);
        Course AddCourse(Course course);
        Course UpdateCourse(Course course);
        void DeleteCourse(int CourseId);
        IEnumerable<Course> SearchCourses(string searchTerm);
        string uploadImage(IFormFile file);
    }
}
