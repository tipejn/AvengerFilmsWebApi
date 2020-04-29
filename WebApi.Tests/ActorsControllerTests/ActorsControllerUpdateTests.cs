using FilmsWebApi.Controllers;
using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FilmsWebApi.Tests.ActorsControllerTests
{
    class ActorsControllerUpdateTests
    {
        [Test]
        [TestCaseSource(typeof(TestData), "UpdatedActors")]
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
        [TestCaseSource(typeof(TestData), "NewActors")]
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
        [TestCaseSource(typeof(TestData), "NewActors")]
        public void CannotUpdateWhenBrokenId(Actor source)
        {
            var mock = new Mock<IActorService>();

            var controller = new ActorsController(mock.Object);
            var response = controller.PutActor(0, source);

            mock.Verify(m => m.UpdateActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        [TestCaseSource(typeof(TestData), "IntExtremeValues")]
        public void CannotUpdateWhenPassingExtremeValues(int id)
        {
            var mock = new Mock<IActorService>();

            var controller = new ActorsController(mock.Object);
            var response = controller.PutActor(id, new Actor());

            mock.Verify(m => m.UpdateActor(It.IsAny<Actor>()), Times.Never);

            Assert.That(response, Is.TypeOf<BadRequestResult>().Or.TypeOf<NotFoundResult>());
        }
    }
}
