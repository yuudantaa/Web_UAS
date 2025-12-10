using Microsoft.EntityFrameworkCore;
using TrainerCourse.Backend.Data;
using TrainerCourse.Backend.DbMapper;
using TrainerCourse.Backend.Models;

namespace TrainerCourse.Backend.DataEF
{
    public class TrainerEF : ITrainer
    {
        private readonly ApplicationDbContext _context;
        public TrainerEF(ApplicationDbContext context)
        {
            _context = context;
        }
        public Trainer AddTrainer(Trainer trainer)
        {
            try
            {
                _context.Trainers.Add(trainer);
                _context.SaveChanges();
                return trainer;
            }
            catch (Exception ex)
            {
                throw new Exception("tidak bisa", ex);
            }
        }

        public void DeleteTrainer(int trainerId)
        {
            var trainer = GetTrainerById(trainerId);
            if (trainer == null)
            {
                throw new Exception($"Trainer dengan ID {trainerId} tidak ditemukan");
            }

            try
            {
                
                var hasCourses = _context.Courses.Any(c => c.TrainerId == trainerId);
                if (hasCourses)
                {
                    throw new InvalidOperationException("Tidak dapat menghapus trainer karena masih memiliki course terkait");
                }

                _context.Trainers.Remove(trainer);
                _context.SaveChanges();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Tidak dapat menghapus trainer karena terdapat data terkait", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Terjadi kesalahan saat menghapus trainer: {ex.Message}", ex);
            }
        }

        public IEnumerable<Trainer> GetTrainer()
        {
            var trainer = _context.Trainers.OrderByDescending(c => c.TrainerId);
            return trainer;
        }

        public Trainer GetTrainerById(int trainerId)
        {
            var trainer = _context.Trainers.FirstOrDefault(c => c.TrainerId == trainerId);
            if (trainer == null)
            {
                throw new Exception("Tidak ada");
            }
            return trainer;
        }

        public IEnumerable<Trainer> SearchTrainers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetTrainer();

            var trainers = _context.Trainers
                .Where(t =>
                    t.TrainerName.Contains(searchTerm) ||
                    t.TrainerEmail.Contains(searchTerm) ||
                    t.TrainerSpecialization.Contains(searchTerm) ||
                    t.TrainerAddress.Contains(searchTerm) ||
                    t.TrainerPhone.Contains(searchTerm)
                )
                .OrderByDescending(t => t.TrainerId);

            return trainers;
        }

        public Trainer UpdateTrainer(Trainer trainer)
        {
            var existingTrainer = GetTrainerById(trainer.TrainerId);
            if (existingTrainer == null)
            {
                throw new Exception("not found");
            }

            try
            {
                existingTrainer.TrainerPhone = trainer.TrainerPhone;
                existingTrainer.TrainerName = trainer.TrainerName;
                existingTrainer.TrainerAddress = trainer.TrainerAddress;
                existingTrainer.TrainerEmail = trainer.TrainerEmail;
                existingTrainer.TrainerSpecialization = trainer.TrainerSpecialization;
                _context.Trainers.Update(existingTrainer);
                _context.SaveChanges();
                return existingTrainer;
            }

            catch (Exception ex)
            {
                throw new Exception("Could not update trainers", ex);
            }
        }
    }
}
