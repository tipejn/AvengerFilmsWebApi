using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;

namespace FilmsWebApi.Tests.ActorsControllerTests
{
    class ActorsControllerAddTests
    {
        [Test]
        [TestCaseSource(typeof(TestData), "NewActors")]
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
        [TestCaseSource(typeof(TestData), "NewActorWithNonexistingFilms")]
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
        [TestCaseSource(typeof(TestData), "BrokenActors")]
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
    }
}
