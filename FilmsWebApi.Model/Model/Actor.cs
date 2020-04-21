using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FilmsWebApi.Model
{
    public class Actor
    {
        public Actor()
        {
            ActorFilms = new List<ActorFilm>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore]
        public ICollection<ActorFilm> ActorFilms { get; set; }
        [NotMapped]
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public ICollection<Film> Films
        {
            get => ActorFilms.Select(f => f.Film).ToList();
            set => ActorFilms = value.Select(f => new ActorFilm()
            {
                FilmId = f.Id,
                Film = f,
                ActorId = Id,
                Actor = this
            }).ToList();
        }

    }
}
