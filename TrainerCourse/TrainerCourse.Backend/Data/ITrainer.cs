using TrainerCourse.Backend.Models;

namespace TrainerCourse.Backend.Data
{
    public interface ITrainer
    {
        Trainer AddTrainer(Trainer trainer);
        Trainer UpdateTrainer(Trainer trainer);
        void DeleteTrainer(int trainerId);
        IEnumerable<Trainer> GetTrainer();
        Trainer GetTrainerById(int trainerId);
        IEnumerable<Trainer> SearchTrainers(string searchTerm);
    }
}
