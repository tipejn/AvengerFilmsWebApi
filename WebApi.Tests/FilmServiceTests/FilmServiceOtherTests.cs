using FilmsWebApi.Model;
using FilmsWebApi.Service;
using NUnit.Framework;

namespace FilmsWebApi.Tests.FilmServiceTests
{
    public class FilmServiceOtherTests : TestsSetup
    {
        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilmsWithActors")]
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

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CanCheckIfFilmExistsWhenPassingExtremeValues(int id)
        {
            using (var context = GetContext())
            {
                var service = new FilmService(context);

                Assert.That(!service.FilmExists(id));
            }
        }
    }
}
