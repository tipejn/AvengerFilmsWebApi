using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FilmsWebApi.Tests
{
    public class ActorsControllerGetTests
    {
        [Test]
        public void CanGetAllActorsWithoutFilms()
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetAllActors()).Returns(TestData.NewActors);

            var controller = new ActorsController(mock.Object);
            var actors = controller.GetActors();

            mock.Verify(m => m.GetAllActors(), Times.Once);

            Assert.That(actors, Has.Count.EqualTo(TestData.NewActors.Count()));
        }

        [Test]
        public void CanGetAllActorsWithFilms()
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetAllActorsWithFilms()).Returns(TestData.ExistingActors);

            var controller = new ActorsController(mock.Object);
            var actors = controller.GetActorsWithFilms();

            mock.Verify(m => m.GetAllActorsWithFilms(), Times.Once);

            Assert.That(actors, Has.Count.EqualTo(TestData.ExistingActors.Count()));
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewActors")]
        public void CanGetSingleActorWithoutFilms(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActor(source.Id)).Returns(source);

            var controller = new ActorsController(mock.Object);
            var response = controller.GetActor(source.Id);

            mock.Verify(m => m.GetActor(source.Id), Times.Once);

            Assert.That(response, Has.Property("Value").Not.Null);
        }

        [Test]
        [TestCaseSource(typeof(TestData), "ExistingActors")]
        public void CanGetSingleActorWithFilms(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActorWithFilms(source.Id)).Returns(source);

            var controller = new ActorsController(mock.Object);
            var response = controller.GetActorWithFilms(source.Id);

            mock.Verify(m => m.GetActorWithFilms(source.Id), Times.Once);

            Assert.That(response, Has.Property("Value").Not.Null);
        }

        [Test]
        public void CannotGetNonexistentActorWithoutFilms()
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActor(44)).Returns(null as Actor);

            var controller = new ActorsController(mock.Object);
            var response = controller.GetActor(44);

            mock.Verify(m => m.GetActor(44), Times.Once);

            Assert.That(response, 
                Has.Property("Result").TypeOf<NotFoundResult>()
                .And.Property("Value").Null);
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotGetActorWhenPassingExtremeValues(int id)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActor(id)).Returns(null as Actor);

            var controller = new ActorsController(mock.Object);
            var response = controller.GetActor(id);

            mock.Verify(m => m.GetActor(id), Times.Once);

            Assert.That(response,
                Has.Property("Result").TypeOf<NotFoundResult>()
                .And.Property("Value").Null);
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotGetActorWithFilmsWhenPassingExtremeValues(int id)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActorWithFilms(id)).Returns(null as Actor);

            var controller = new ActorsController(mock.Object);
            var response = controller.GetActorWithFilms(id);

            mock.Verify(m => m.GetActorWithFilms(id), Times.Once);

            Assert.That(response,
                Has.Property("Result").TypeOf<NotFoundResult>()
                .And.Property("Value").Null);
        }

        [Test]
        public void CannotGetNonexistentActorWithFilms()
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActorWithFilms(44)).Returns(null as Actor);

            var controller = new ActorsController(mock.Object);
            var response = controller.GetActorWithFilms(44);

            mock.Verify(m => m.GetActorWithFilms(44), Times.Once);

            Assert.That(response,
                Has.Property("Result").TypeOf<NotFoundResult>()
                .And.Property("Value").Null);
        }
    }
}
