using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Model
{
    public class ActorFilm
    {
        public int ActorId { get; set; }
        public Actor Actor { get; set; }
        public int FilmId { get; set; }
        public Film Film { get; set; }

    }
}
