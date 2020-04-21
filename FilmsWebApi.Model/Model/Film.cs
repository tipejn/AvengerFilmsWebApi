using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FilmsWebApi.Model
{
    public class Film
    {
        public Film()
        {
            ActorFilms = new List<ActorFilm>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        
        [JsonIgnore]
        public ICollection<ActorFilm> ActorFilms { get; set; }
        [NotMapped]
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public ICollection<Actor> Actors
        {
            get => ActorFilms.Select(a => a.Actor).ToList();
            set => ActorFilms = value.Select(a => new ActorFilm
            {
                ActorId = a.Id,
                Actor = a,
                FilmId = Id,
                Film = this
            }).ToList();
        }
        
    }
}
