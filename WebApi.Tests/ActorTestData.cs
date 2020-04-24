﻿using FilmsWebApi.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilmsWebApi.Tests
{
    public class ActorTestData
    {
        public static List<Film> ExistingFilms
        {
            get
            {
                return new List<Film>
                {
                    new Film { Id = 1, Title = "Iron Man", ReleaseDate = new DateTime(2008, 4, 30) },
                    new Film { Id = 2, Title = "Avengers", ReleaseDate = new DateTime(2012, 4, 11) }
                };
            }
        }

        public static List<Film> NewFilms
        {
            get
            {
                return new List<Film>
                {
                    new Film { Id = 3, Title = "Captain America", ReleaseDate = new DateTime(2011, 8, 5) },
                    new Film { Id = 4, Title = "Thor", ReleaseDate = new DateTime(2011, 4, 29) }
                };
            }
        }
        public static IEnumerable<Actor> ExistingActors
        {
            get
            {
                var actor1 = new Actor { Id = 1, FirstName = "Robert", LastName = "Downey" };
                var actor2 = new Actor { Id = 2, FirstName = "Scarlett", LastName = "Johanson" };

                actor1.ActorFilms.Add(new ActorFilm { Film = ExistingFilms[0] });
                actor1.ActorFilms.Add(new ActorFilm { Film = ExistingFilms[1] });
                actor2.ActorFilms.Add(new ActorFilm { Film = ExistingFilms[1] });

                return new List<Actor> { actor1, actor2 };
            }
        }

        public static IEnumerable<Actor> NewActors
        {
            get
            {
                return new List<Actor>
                {
                    new Actor { Id = 3, FirstName = "Chris", LastName = "Evans" },
                    new Actor { Id = 4, FirstName = "Gwyneth", LastName = "Paltrow" },
                    new Actor { Id = 5, FirstName = "Chris", LastName = "Hemsworth" }
                };
            }
        }

        public static IEnumerable<Actor> NewActorWithExistingFilms
        {
            get
            {
                var actors = NewActors;
                foreach (var actor in actors)
                {
                    actor.ActorFilms.Add(new ActorFilm { Film = ExistingFilms[0] });
                    actor.ActorFilms.Add(new ActorFilm { Film = ExistingFilms[1] });
                }
                return actors;
            }
        }

        public static IEnumerable<Actor> NewActorWithNonexistingFilms
        {
            get
            {
                var actors = NewActors;
                foreach (var actor in actors)
                {
                    actor.ActorFilms.Add(new ActorFilm { Film = NewFilms[0] });
                    actor.ActorFilms.Add(new ActorFilm { Film = NewFilms[1] });
                }
                return actors;
            }
        }

        public static IEnumerable<Actor> BrokenActors
        {
            get
            {
                var actors = NewActors;
                foreach (var actor in actors)
                {
                    actor.Id = ExistingActors.First().Id;
                }
                return actors;
            }
        }

        public static IEnumerable<Actor> UpdatedActors
        {
            get
            {
                var actors = ExistingActors;
                foreach (var actor in actors)
                {
                    actor.FirstName += "updated";
                    actor.LastName += "updated";
                }
                return actors;
            }
        }
    }
}