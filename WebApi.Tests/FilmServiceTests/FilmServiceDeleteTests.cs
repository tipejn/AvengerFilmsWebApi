using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace FilmsWebApi.Tests.FilmServiceTests
{
    public class FilmServiceDeleteTests : TestsSetup
    {
        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilmsWithActors")]
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
    }
}
