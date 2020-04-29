using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FilmsWebApi.Tests.ActorsControllerTests
{
    class ActorsControllerDeleteTests
    {
        [Test]
        [TestCaseSource(typeof(TestData), "ExistingActors")]
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
        [TestCaseSource(typeof(TestData), "NewActors")]
        public void CannotDeleteNonexistentActor(Actor source)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActor(source.Id)).Returns(null as Actor);

            var controller = new ActorsController(mock.Object);
            var response = controller.DeleteActor(source.Id);

            mock.Verify(m => m.DeleteActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<NotFoundResult>());
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotDeleteWhenPassingExtremeValues(int id)
        {
            var mock = new Mock<IActorService>();
            mock.Setup(m => m.GetActor(id)).Returns(null as Actor);

            var controller = new ActorsController(mock.Object);
            var response = controller.DeleteActor(id);

            mock.Verify(m => m.DeleteActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Has.Property("Result").TypeOf<NotFoundResult>());
        }
    }
}
