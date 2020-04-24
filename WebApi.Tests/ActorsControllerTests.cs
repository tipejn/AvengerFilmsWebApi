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
    public class ActorControllerTests
    {
        [Test]
        public void CanGetAllActorsWithoutFilms()
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetAllActors()).Returns(ActorTestData.NewActors);

            var controller = new ActorsController(mock.Object);
            var actors = controller.GetActors();

            mock.Verify(m => m.GetAllActors(), Times.Once);

            Assert.That(actors, Has.Count.EqualTo(ActorTestData.NewActors.Count()));
        }

        [Test]
        public void CanGetAllActorsWithFilms()
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetAllActorsWithFilms()).Returns(ActorTestData.ExistingActors);

            var controller = new ActorsController(mock.Object);
            var actors = controller.GetActorsWithFilms();

            mock.Verify(m => m.GetAllActorsWithFilms(), Times.Once);

            Assert.That(actors, Has.Count.EqualTo(ActorTestData.ExistingActors.Count()));
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
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
        [TestCaseSource(typeof(ActorTestData), "ExistingActors")]
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

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CanAddActor(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.ActorExists(source.Id)).Returns(false);

            var controller = new ActorsController(mock.Object);
            var response = controller.PostActor(source);

            mock.Verify(m => m.AddActor(source), Times.Once);

            Assert.That(response, Has.Property("Result").TypeOf<CreatedAtActionResult>());
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActorWithNonexistingFilms")]
        public void CanCatchExceptionWhenAddingActorToNonExistingFilm(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.AddActor(source)).Throws(new InvalidOperationException());

            var controller = new ActorsController(mock.Object);
            var response = controller.PostActor(source);

            mock.Verify(m => m.AddActor(source), Times.Once);

            Assert.That(response, Has.Property("Result").TypeOf<BadRequestResult>());
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "BrokenActors")]
        public void CannotAddActorWithExistingId(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.ActorExists(source.Id)).Returns(true);

            var controller = new ActorsController(mock.Object);
            var response = controller.PostActor(source);

            mock.Verify(m => m.AddActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<BadRequestResult>());
        }

        [Test]
        public void CannotAddNull()
        {
            var mock = new Mock<IActorService>();
            var controller = new ActorsController(mock.Object);

            var response = controller.PostActor(null);

            mock.Verify(m => m.AddActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<BadRequestResult>());
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "UpdatedActors")]
        public void CanUpdateActor(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.ActorExists(source.Id)).Returns(true);

            var controller = new ActorsController(mock.Object);
            var response = controller.PutActor(source.Id, source);

            mock.Verify(m => m.UpdateActor(source), Times.Once);

            Assert.That(response, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CannotUpdateNonexistentActor(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.ActorExists(source.Id)).Returns(false);

            var controller = new ActorsController(mock.Object);
            var response = controller.PutActor(source.Id, source);

            mock.Verify(m => m.UpdateActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void CannotUpdateNull()
        {
            var mock = new Mock<IActorService>();

            var controller = new ActorsController(mock.Object);
            var response = controller.PutActor(0, null);

            mock.Verify(m => m.UpdateActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>());
        }


        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CannotUpdateWhenBrokenId(Actor source)
        {
            var mock = new Mock<IActorService>();

            var controller = new ActorsController(mock.Object);
            var response = controller.PutActor(0, source);

            mock.Verify(m => m.UpdateActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "ExistingActors")]
        public void CanDeleteActor(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActor(source.Id)).Returns(source);

            var controller = new ActorsController(mock.Object);
            var response = controller.DeleteActor(source.Id);

            mock.Verify(m => m.DeleteActor(source), Times.Once);

            Assert.That(response, Has.Property("Result").TypeOf<NoContentResult>());
        }

        [Test]
        [TestCaseSource(typeof(ActorTestData), "NewActors")]
        public void CannotDeleteNonexistentActor(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActor(source.Id)).Returns(null as Actor);

            var controller = new ActorsController(mock.Object);
            var response = controller.DeleteActor(source.Id);

            mock.Verify(m => m.DeleteActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<NotFoundResult>());
        }
    }
}
