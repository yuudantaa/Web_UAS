using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainerCourse.Shared.Model
{
    public class ImageUploadResponse
    {
        public bool Success { get; set; }
        public string? FileName { get; set; }
        public string? Message { get; set; }
        public string? ImageUrl { get; set; }
    }
}
