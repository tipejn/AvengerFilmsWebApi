using FilmsWebApi.Data;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;

namespace FilmsWebApi.Tests
{
    public class FilmServiceTests
    {
        private DbContextOptions<FilmContext> _options;

        [SetUp]
        public void Setup()
        {
            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            _options = new DbContextOptionsBuilder<FilmContext>()
                .UseInMemoryDatabase("FilmDb")
                .UseInternalServiceProvider(provider)
                .Options;

            using (var context = GetContext())
            {
                var helper = new TestFilmContextHelper(context);
                helper.SeedContext();
                helper.ResetEntities();
            }
        }

        [Test]
        public void CanGetAllFilmsWithoutActors()
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var films = service.GetAllFilms();

                Assert.That(films,
                    Has.Count.EqualTo(2)
                    .And.All.Property("Actors").Empty);
            }
        }

        [Test]
        public void CanGetAllFilmsWithActors()
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var films = service.GetAllFilmsWithActors();

                Assert.That(films, Has.Some.Property("Actors").Not.Empty);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilms")]
        public void CanGetSingleFilmWithoutActors(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilm(source.Id);

                Assert.That(film,
                    Is.Not.Null
                    .And.Property("Actors").Empty
                    .And.Property("Title").EqualTo(source.Title)
                    .And.Property("ReleaseDate").EqualTo(source.ReleaseDate));
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilms")]
        public void CanGetSingleFilmWithActors(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilmWithActors(source.Id);

                Assert.That(film,
                    Is.Not.Null
                    .And.Property("Actors").Not.Empty
                    .And.Property("Title").EqualTo(source.Title)
                    .And.Property("ReleaseDate").EqualTo(source.ReleaseDate));
            }
        }

        [Test]
        public void CannotGetNonexistentFilmWithoutActors()
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilm(44);

                Assert.That(film, Is.Null);
            }
        }

        [Test]
        public void CannotGetNonexistentFilmWithActors()
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilmWithActors(44);

                Assert.That(film, Is.Null);
            }
        }

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
        [TestCaseSource(typeof(TestData), "ExistingFilms")]
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

        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilms")]
        public void CanDeleteFilm(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilm(source.Id);
                service.DeleteFilm(film);
            }

            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var film = service.GetFilm(source.Id);
                Assert.That(film, Is.Null);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CannotDeleteNonexistentFilm(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(() => service.DeleteFilm(source), Throws.InstanceOf<DbUpdateConcurrencyException>());
            }
        }

        [Test]
        public void CannotDeleteNull()
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(() => service.DeleteFilm(null), Throws.InstanceOf<ArgumentNullException>());
            }

        }

        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilms")]
        public void CanCheckThatFilmExists(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(service.FilmExists(source.Id));
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CanCheckThatFilmDoesNotExist(Film source)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(!service.FilmExists(source.Id));
            }
        }

        private FilmContext GetContext()
        {
            return new FilmContext(_options);
        }
    }
}
