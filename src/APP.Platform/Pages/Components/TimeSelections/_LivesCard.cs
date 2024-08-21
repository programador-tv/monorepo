using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class LivesCardPageModel
    {
        public Guid Id { get; set; }
        public string? Titulo { get; set; }
        public string? TempoRestante { get; set; }
    }
}
