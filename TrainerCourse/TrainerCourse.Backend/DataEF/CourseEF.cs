using Microsoft.EntityFrameworkCore;
using TrainerCourse.Backend.Data;
using TrainerCourse.Backend.DbMapper;
using TrainerCourse.Backend.Models;

namespace TrainerCourse.Backend.DataEF
{
    public class CourseEF : ICourse
    {
        private readonly ApplicationDbContext _context;
        public CourseEF(ApplicationDbContext context)
        {
            _context = context;
        }
        public Course AddCourse(Course course)
        {
            try
            {
                _context.Courses.Add(course);
                _context.SaveChanges();
                return course;
            }
            catch (Exception ex)
            {
                throw new Exception("tidak bisa", ex);
            }
        }

        public void DeleteCourse(int CourseId)
        {
            var course = GetCourseById(CourseId);
            if (course == null)
            {
                throw new Exception($"Course dengan ID {CourseId} tidak ditemukan");
            }

            try
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Tidak dapat menghapus course karena terdapat data terkait", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Terjadi kesalahan saat menghapus course: {ex.Message}", ex);
            }
        }

        public Course GetCourseById(int CourseId)
        {
            var course = _context.Courses.Include(c => c.Trainer).FirstOrDefault(c => c.CourseId == CourseId);
            if (course == null)
            {
                throw new Exception("Tidak ada");
            }
            return course;
        }

        public IEnumerable<Course> GetCourses()
        {
            var courses = _context.Courses.Include(c => c.Trainer).OrderByDescending(c => c.CourseId);
            return courses;
        }

        public IEnumerable<Course> SearchCourses(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetCourses();

            var courses = _context.Courses
                .Include(c => c.Trainer)
                .Where(c =>
                    c.CourseName.Contains(searchTerm) ||
                    c.CourseDescription.Contains(searchTerm) ||
                    c.CourseType.Contains(searchTerm) ||
                    (c.Trainer != null && c.Trainer.TrainerName.Contains(searchTerm))
                )
                .OrderByDescending(c => c.CourseId);

            return courses;
        }

        public Course UpdateCourse(Course course)
        {
            var existingCourse = GetCourseById(course.CourseId);
            if (existingCourse == null)
            {
                throw new Exception("not found");
            }

            try
            {
                existingCourse.CourseName = course.CourseName;
                existingCourse.CourseDescription = course.CourseDescription;
                existingCourse.Duration = course.Duration;
                existingCourse.TrainerId = course.TrainerId;
                existingCourse.CourseType = course.CourseType;

                // Jangan lupa update ImageFileName jika ada
                if (!string.IsNullOrEmpty(course.ImageFileName))
                {
                    existingCourse.ImageFileName = course.ImageFileName;
                }

                _context.Courses.Update(existingCourse);
                _context.SaveChanges();
                return existingCourse;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not update course", ex);
            }
        }

        public string uploadImage(IFormFile file)
        {
            // Extension validation
            List<string> validExtensions = new List<string>() { ".png", ".jpg", ".jpeg" };
            string extension = Path.GetExtension(file.FileName);
            if (!validExtensions.Contains(extension.ToLower()))
            {
                throw new Exception($"Extension is not valid ({string.Join(',', validExtensions)})");
            }

            // Size validation
            long size = file.Length;
            if (size > (5 * 1024 * 1024))
                throw new Exception("File size is too large (max 5MB)");

            // Name change and save
            string fileName = Guid.NewGuid().ToString() + extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Ensure directory exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
            file.CopyTo(stream);

            return fileName;
        }
    }
}
