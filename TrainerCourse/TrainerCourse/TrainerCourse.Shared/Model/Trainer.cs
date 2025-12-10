using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainerCourse.Shared.Model
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public string TrainerEmail { get; set; } = string.Empty;
        public string TrainerAddress { get; set; } = string.Empty;
        public string TrainerPhone { get; set; } = string.Empty;
        public string TrainerSpecialization { get; set; } = string.Empty;
    }
}
