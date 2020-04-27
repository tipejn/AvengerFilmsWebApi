using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace FilmsWebApi.Tests
{
    public class FilmControllerTests
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

        [Test]
        [TestCaseSource(typeof(TestData), "UpdatedFilms")]
        public void CanUpdateFilm(Film source)
        {
            var mock = new Mock<IFilmService>();
            mock.Setup(m => m.FilmExists(source.Id)).Returns(true);

            var controller = new FilmsController(mock.Object);
            var response = controller.PutActor(source.Id, source);

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
            var response = controller.PutActor(source.Id, source);

            mock.Verify(m => m.UpdateFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void CannotUpdateNull()
        {
            var mock = new Mock<IFilmService>();

            var controller = new FilmsController(mock.Object);
            var response = controller.PutActor(0, null);

            mock.Verify(m => m.UpdateFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>());
        }


        [Test]
        [TestCaseSource(typeof(TestData), "NewFilms")]
        public void CannotUpdateWhenBrokenId(Film source)
        {
            var mock = new Mock<IFilmService>();

            var controller = new FilmsController(mock.Object);
            var response = controller.PutActor(0, source);

            mock.Verify(m => m.UpdateFilm(It.IsAny<Film>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>());
        }

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
    }
}
