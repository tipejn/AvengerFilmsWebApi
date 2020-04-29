using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FilmsWebApi.Tests
{
    public class FilmsControllerGetTests
    {
        [Test]
        public void CanGetAllFilmsWithoutActors()
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetAllFilms()).Returns(TestData.NewFilms);

            var controller = new FilmsController(mock.Object);
            var actors = controller.GetFilms();

            mock.Verify(m => m.GetAllFilms(), Times.Once);

            Assert.That(actors, Has.Count.EqualTo(TestData.NewFilms.Count()));
        }

        [Test]
        public void CanGetAllFilmsWithActors()
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetAllFilmsWithActors()).Returns(TestData.ExistingFilmsWithActors);

            var controller = new FilmsController(mock.Object);
            var actors = controller.GetFilmsWithActors();

            mock.Verify(m => m.GetAllFilmsWithActors(), Times.Once);

            Assert.That(actors, Has.Count.EqualTo(TestData.ExistingFilmsWithActors.Count()));
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CanGetSingleFilmWithoutActors(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilm(source.Id)).Returns(source);

            var controller = new FilmsController(mock.Object);
            var response = controller.GetFilm(source.Id);

            mock.Verify(m => m.GetFilm(source.Id), Times.Once);

            Assert.That(response, Has.Property("Value").Not.Null);
        }

        [Test]
        [TestCaseSource(typeof(TestData), "ExistingFilmsWithActors")]
        public void CanGetSingleFilmWithActors(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilmWithActors(source.Id)).Returns(source);

            var controller = new FilmsController(mock.Object);
            var response = controller.GetFilmWithActors(source.Id);

            mock.Verify(m => m.GetFilmWithActors(source.Id), Times.Once);

            Assert.That(response, Has.Property("Value").Not.Null);
        }

        [Test]
        public void CannotGetNonexistentFilmWithoutActors()
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilm(44)).Returns(null as Film);

            var controller = new FilmsController(mock.Object);
            var response = controller.GetFilm(44);

            mock.Verify(m => m.GetFilm(44), Times.Once);

            Assert.That(response,
                Has.Property("Result").TypeOf<NotFoundResult>()
                .And.Property("Value").Null);
        }

        [Test]
        public void CannotGetNonexistentFilmWithActors()
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilmWithActors(44)).Returns(null as Film);

            var controller = new FilmsController(mock.Object);
            var response = controller.GetFilmWithActors(44);

            mock.Verify(m => m.GetFilmWithActors(44), Times.Once);

            Assert.That(response,
                Has.Property("Result").TypeOf<NotFoundResult>()
                .And.Property("Value").Null);
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotGetFilmWhenPassingExtremeValues(int id)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilm(id)).Returns(null as Film);

            var controller = new FilmsController(mock.Object);
            var response = controller.GetFilm(id);

            mock.Verify(m => m.GetFilm(id), Times.Once);

            Assert.That(response,
                Has.Property("Result").TypeOf<NotFoundResult>()
                .And.Property("Value").Null);
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotGetFilmWithActorsWhenPassingExtremeValues(int id)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.GetFilmWithActors(id)).Returns(null as Film);

            var controller = new FilmsController(mock.Object);
            var response = controller.GetFilmWithActors(id);

            mock.Verify(m => m.GetFilmWithActors(id), Times.Once);

            Assert.That(response,
                Has.Property("Result").TypeOf<NotFoundResult>()
                .And.Property("Value").Null);
        }
    }
}
