using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FilmsWebApi.Tests.FilmsControllerTests
{
    public class FilmsControllerDeleteTests
    {
        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilmsWithActors")]
        public void CanDeleteFilm(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilm(source.Id)).Returns(source);

            var controller = new FilmsController(mock.Object);
            var response = controller.DeleteFilm(source.Id);

            mock.Verify(m => m.DeleteFilm(source), Times.Once);

            Assert.That(response, Has.Property("Result").TypeOf<NoContentResult>());
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CannotDeleteNonexistentFilm(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilm(source.Id)).Returns(null as Film);

            var controller = new FilmsController(mock.Object);
            var response = controller.DeleteFilm(source.Id);

            mock.Verify(m => m.DeleteFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<NotFoundResult>());
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotDeleteWhenPassingExtremeValues(int id)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilm(id)).Returns(null as Film);

            var controller = new FilmsController(mock.Object);
            var response = controller.DeleteFilm(id);

            mock.Verify(m => m.DeleteFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<NotFoundResult>());
        }
    }
}
