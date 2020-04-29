using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FilmsWebApi.Tests.FilmsControllerTests
{
    public class FilmsControllerUpdateTests
    {
        [Test]
        [TestCaseSource(typeof(TestData), "UpdatedFilms")]
        public void CanUpdateFilm(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.FilmExists(source.Id)).Returns(true);

            var controller = new FilmsController(mock.Object);
            var response = controller.PutFilm(source.Id, source);

            mock.Verify(m => m.UpdateFilm(source), Times.Once);

            Assert.That(response, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CannotUpdateNonexistentFilm(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.FilmExists(source.Id)).Returns(false);

            var controller = new FilmsController(mock.Object);
            var response = controller.PutFilm(source.Id, source);

            mock.Verify(m => m.UpdateFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void CannotUpdateNull()
        {
            var mock = new Mock<IFilmService>();

            var controller = new FilmsController(mock.Object);
            var response = controller.PutFilm(0, null);

            mock.Verify(m => m.UpdateFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>());
        }


        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CannotUpdateWhenBrokenId(Film source)
        {
            var mock = new Mock<IFilmService>();

            var controller = new FilmsController(mock.Object);
            var response = controller.PutFilm(0, source);

            mock.Verify(m => m.UpdateFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotUpdateWhenPassingExtremeValues(int id)
        {
            var mock = new Mock<IFilmService>();

            var controller = new FilmsController(mock.Object);
            var response = controller.PutFilm(id, new Film());

            mock.Verify(m => m.UpdateFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>().Or.TypeOf<NotFoundResult>());
        }
    }
}
