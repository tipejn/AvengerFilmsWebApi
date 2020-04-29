using FilmsWebApi.Model;
using FilmsWebApi.Service;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace FilmsWebApi.Tests
{
    public class ActorServiceDeleteTests : TestsSetup
    {
        [Test]
        [TestCaseSource(typeof(TestData), "ExistingActors")]
        public void CanDeleteActor(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(source.Id);
                service.DeleteActor(actor);
            }

            using (var context = GetContext())
            {
                var service = new ActorService(context);
                var actor = service.GetActor(source.Id);
                Assert.That(actor, Is.Null);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), "NewActors")]
        public void CannotDeleteNonexistentActor(Actor source)
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.DeleteActor(source), Throws.InstanceOf<DbUpdateConcurrencyException>());
            }
        }

        [Test]
        public void CannotDeleteNull()
        {
            using (var context = GetContext())
            {
                var service = new ActorService(context);

                Assert.That(() => service.DeleteActor(null), Throws.InstanceOf<ArgumentNullException>());
            }
        }
    }
}
