using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;

namespace FilmsWebApi.Tests.FilmsControllerTests
{
    public class FilmsControllerAddTests
    {
        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CanAddFilm(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.FilmExists(source.Id)).Returns(false);

            var controller = new FilmsController(mock.Object);
            var response = controller.PostFilm(source);

            mock.Verify(m => m.AddFilm(source), Times.Once);

            Assert.That(response, Has.Property("Result").TypeOf<CreatedAtActionResult>());
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilmsWithNonexistingActors")]
        public void CanCatchExceptionWhenAddingFilmToNonExistingActor(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.AddFilm(source)).Throws(new InvalidOperationException());

            var controller = new FilmsController(mock.Object);
            var response = controller.PostFilm(source);

            mock.Verify(m => m.AddFilm(source), Times.Once);

            Assert.That(response, Has.Property("Result").TypeOf<BadRequestResult>());
        }

        [Test]
        [TestCaseSource(typeof(TestData), "BrokenFilms")]
        public void CannotAddFilmWithExistingId(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.FilmExists(source.Id)).Returns(true);

            var controller = new FilmsController(mock.Object);
            var response = controller.PostFilm(source);

            mock.Verify(m => m.AddFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<BadRequestResult>());
        }

        [Test]
        public void CannotAddNull()
        {
            var mock = new Mock<IFilmService>();
            var controller = new FilmsController(mock.Object);

            var response = controller.PostFilm(null);

            mock.Verify(m => m.AddFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<BadRequestResult>());
        }
    }
}
