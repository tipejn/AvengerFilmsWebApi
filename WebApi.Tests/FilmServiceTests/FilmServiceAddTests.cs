using FilmsWebApi.Model;
using FilmsWebApi.Service;
using NUnit.Framework;
using System;
using System.Linq;

namespace FilmsWebApi.Tests
{
    public class FilmServiceAddTests : TestsSetup
    {
        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CanAddFilmWithoutAnyActor(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                service.AddFilm(source);
            }

            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilmWithActors(source.Id);
                Assert.That(film,
                    Is.Not.Null
                    .And.Property("Title").EqualTo(source.Title)
                    .And.Property("ReleaseDate").EqualTo(source.ReleaseDate)
                    .And.Property("Actors").Empty);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilmsWithExistingActors")]
        public void CanAddFilmToExistingActor(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                service.AddFilm(source);
            }

            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilmWithActors(source.Id);
                Assert.That(film.Actors, Has.Count.EqualTo(source.Actors.Count()));
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilmsWithNonexistingActors")]
        public void CannotAddFilmToNonexistentActor(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(() => service.AddFilm(source), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "BrokenFilms")]
        public void CannotAddFilmWithExistingId(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(() => service.AddFilm(source), Throws.InstanceOf<ArgumentException>());
            }
        }

        [Test]
        public void CannotAddNull()
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(() => service.AddFilm(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }
    }
}
