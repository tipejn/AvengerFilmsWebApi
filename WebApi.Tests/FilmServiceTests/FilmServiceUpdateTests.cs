using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace FilmsWebApi.Tests.FilmServiceTests
{
    public class FilmServiceUpdateTests : TestsSetup
    {
        [Test]
        [TestCaseSource(typeof(TestData), "UpdatedFilms")]
        public void CanUpdateActor(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                service.UpdateFilm(source);
            }

            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilm(source.Id);
                Assert.That(film,
                    Has.Property("Title").EqualTo(source.Title)
                    .And.Property("ReleaseDate").EqualTo(source.ReleaseDate));
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilmsWithActors")]
        public void CanRemoveActorFromFilm(Film source)
        {
            var actorToDelete = source.Actors.First();

            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilmWithActors(source.Id);
                var actorfilm = film.ActorFilms.Single(f => f.Actor.Id == actorToDelete.Id);
                film.ActorFilms.Remove(actorfilm);
                service.UpdateFilm(film);
            }

            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilmWithActors(source.Id);
                Assert.That(film.Actors,
                    Has.Exactly(source.Actors.Count - 1).Items
                    .And.Exactly(0).Matches<Actor>(a => a.Id == actorToDelete.Id));
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CannotUpdateNonexistentFilm(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(() => service.UpdateFilm(source), Throws.InstanceOf<DbUpdateConcurrencyException>());
            }
        }

        [Test]
        public void CannotUpdateNull()
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(() => service.UpdateFilm(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }
    }
}
