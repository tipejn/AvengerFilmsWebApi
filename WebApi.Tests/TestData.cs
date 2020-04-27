using FilmsWebApi.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilmsWebApi.Tests
{
    public class TestData
    {

        public static List<Film> ExistingFilmsWithoutActors
        {
            get
            {
                var film1 = new Film { Id = 1, Title = "Iron Man", ReleaseDate = new DateTime(2008, 4, 30) };
                var film2 = new Film { Id = 2, Title = "Avengers", ReleaseDate = new DateTime(2012, 4, 11) };

                return new List<Film> { film1, film2 };
            }
        }
        public static List<Film> ExistingFilms
        {
            get
            {
                var film1 = new Film { Id = 1, Title = "Iron Man", ReleaseDate = new DateTime(2008, 4, 30) };
                var film2 = new Film { Id = 2, Title = "Avengers", ReleaseDate = new DateTime(2012, 4, 11) };

                film1.ActorFilms.Add(new ActorFilm { Actor = ExistingActorsWithoutFilms[0] });
                film2.ActorFilms.Add(new ActorFilm { Actor = ExistingActorsWithoutFilms[0] });
                film2.ActorFilms.Add(new ActorFilm { Actor = ExistingActorsWithoutFilms[1] });

                return new List<Film> { film1, film2 };
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
        public static List<Actor> ExistingActors
        {
            get
            {
                var actor1 = new Actor { Id = 1, FirstName = "Robert", LastName = "Downey" };
                var actor2 = new Actor { Id = 2, FirstName = "Scarlett", LastName = "Johanson" };

                actor1.ActorFilms.Add(new ActorFilm { Film = ExistingFilmsWithoutActors[0] });
                actor1.ActorFilms.Add(new ActorFilm { Film = ExistingFilmsWithoutActors[1] });
                actor2.ActorFilms.Add(new ActorFilm { Film = ExistingFilmsWithoutActors[1] });

                return new List<Actor> { actor1, actor2 };
            }
        }

        public static List<Actor> ExistingActorsWithoutFilms
        {
            get
            {
                var actor1 = new Actor { Id = 1, FirstName = "Robert", LastName = "Downey" };
                var actor2 = new Actor { Id = 2, FirstName = "Scarlett", LastName = "Johanson" };

                return new List<Actor> { actor1, actor2 };
            }
        }

        public static List<Actor> NewActors
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

        public static IEnumerable<Film> NewFilmsWithExistingActors
        {
            get
            {
                var films = NewFilms;
                foreach (var film in films)
                {
                    film.ActorFilms.Add(new ActorFilm { Actor = ExistingActors[0] });
                    film.ActorFilms.Add(new ActorFilm { Actor = ExistingActors[1] });
                }
                return films;
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

        public static IEnumerable<Film> NewFilmsWithNonexistingActors
        {
            get
            {
                var films = NewFilms;
                foreach (var film in films)
                {
                    film.ActorFilms.Add(new ActorFilm { Actor = NewActors[0] });
                    film.ActorFilms.Add(new ActorFilm { Actor = NewActors[1] });
                }
                return films;
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

        public static IEnumerable<Film> BrokenFilms
        {
            get
            {
                var films = NewFilms;
                foreach (var film in films)
                {
                    film.Id = ExistingFilms.First().Id;
                }
                return films;
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

        public static IEnumerable<Film> UpdatedFilms
        {
            get
            {
                var films = ExistingFilms;
                foreach (var film in films)
                {
                    film.Title += "2";
                    film.ReleaseDate = film.ReleaseDate.AddDays(3);
                }
                return films;
            }
        }
    }
}