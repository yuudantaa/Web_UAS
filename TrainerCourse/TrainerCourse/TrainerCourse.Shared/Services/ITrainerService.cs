using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainerCourse.Shared.Model;

namespace TrainerCourse.Shared.Services
{
    public interface ITrainerService
    {
        Task<List<Trainer>> GetTrainersAsync();
        Task<Trainer> GetTrainerByIdAsync(int id);
        Task<Trainer> AddTrainerAsync(Trainer trainer);
        Task<Trainer> UpdateTrainerAsync(Trainer trainer);
        Task DeleteTrainerAsync(int id);
        Task<List<Trainer>> SearchTrainersAsync(string searchTerm);
    }
}
