using FilmsWebApi.Model;
using FilmsWebApi.Service;
using NUnit.Framework;

namespace FilmsWebApi.Tests
{
    public class FilmServiceGetTests : TestsSetup
    {

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
        [TestCaseSource(typeof(TestData), "ExistingFilmsWithActors")]
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
        [TestCaseSource(typeof(TestData), "ExistingFilmsWithActors")]
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
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotGetFilmWhenPassingExtremeValues(int id)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var actor = service.GetFilm(id);

                Assert.That(actor, Is.Null);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotGetFilmWithActorsWhenPassingExtremeValues(int id)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);
                var actor = service.GetFilmWithActors(id);

                Assert.That(actor, Is.Null);
            }
        }
    }
}
