using NUnit.Framework;
using FilmsWebApi.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FilmsWebApi.Controllers;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FilmsWebApi.Model;
using Microsoft.AspNetCore.Mvc;
using FilmsWebApi.Service;

namespace FilmsWebApi.Tests
{
    public class Tests
    {
        //[Test]
        //public void CanGetActors()
        //{
        //    var context = new GetMockFilmContext();

        //    var target = new ActorService(context);
        //    var actors = target.GetAllActors();

        //    Assert.That(actors, Has.Exactly(2).Items);
        //}
        //[Test]
        //public void CanGetActorsWithFilms()
        //{
        //    var context = GetFilmContextMock();

        //    var target = new ActorsController(context);
        //    var actors = target.GetActorsWithFilms();

        //    Assert.That(actors, Has.All.Property("Films").Count.GreaterThan(0));
        //}

        //[Test]
        //public void CanGetSingleActorWithoutFilms()
        //{
        //    var context = GetFilmContextMock();

        //    var target = new ActorsController(context);
        //    var actor = target.GetActor(1);

        //    Assert.That(actor.Value, 
        //        Has.Property("FirstName").EqualTo("Robert")
        //        .And
        //        .Property("Films").Empty);
        //}

        //[Test]
        //public void CanGetSingleActorWithFilms()
        //{
        //    var context = GetFilmContextMock();

        //    var target = new ActorsController(context);
        //    var actor = target.GetActorWithFilms(2);

        //    Assert.That(actor.Value, 
        //        Has.Property("LastName").EqualTo("Johanson")
        //        .And
        //        .Property("Films").Count.EqualTo(1));
        //}

        //[Test]
        //public void CanAddActor()
        //{
        //    var actor = new Actor() { FirstName = "Chris", LastName = "Evans" };

        //    var context = GetFilmContextMock();
        //    var target = new ActorsController(context);

        //    target.PostActor(actor);
        //    var actors = context.Actors.ToList();

        //    Assert.That(actors, Has.Exactly(1).Matches<Actor>(
        //        a => a.FirstName == actor.FirstName && a.LastName == actor.LastName));
        //}

        //[Test]
        //public void CanAddActorWithNewFilm()
        //{
        //    var actor = new Actor() { FirstName = "Chris", LastName = "Evans" };
        //    actor.ActorFilms.Add(new ActorFilm()
        //    {
        //        Actor = actor,
        //        Film = new Film()
        //        {
        //            Title = "Captain America",
        //            ReleaseDate = new DateTime(2011, 08, 05)
        //        }
        //    });

        //    var context = GetFilmContextMock();
        //    var target = new ActorsController(context);

        //    target.PostActor(actor);
        //    var resultActor = context.Actors
        //        .FirstOrDefault(a => a.FirstName == actor.FirstName && a.LastName == actor.LastName);

        //    Assert.That(resultActor, 
        //        Is.Not.Null
        //        .And
        //        .Property("Films").Count.EqualTo(1));
        //}

        //[Test]
        //public void CanAddActorToExistingFilm()
        //{
        //    var actor = new Actor() { FirstName = "Chris", LastName = "Evans" };
        //    var filmId = 2;
        //    actor.ActorFilms.Add(new ActorFilm { FilmId = filmId });

        //    var context = GetFilmContextMock();
        //    var target = new ActorsController(context);

        //    target.PostActor(actor);
        //    var resultActor = context.Actors
        //        .FirstOrDefault(a => a.FirstName == actor.FirstName && a.LastName == actor.LastName);

        //    Assert.That(resultActor, 
        //        Is.Not.Null
        //        .And
        //        .Property("Films").Exactly(1).Matches<Film>(
        //            f => f.Id == filmId && f.Title == "Avengers"));
        //}

        //[Test]
        //public void CanUpdateActor()
        //{
        //    var id = 1;
        //    var newLastName = "Downey Junior";
        //    var actor = new Actor() { Id = id, FirstName = "Robert", LastName = newLastName};

        //    var context = GetFilmContextMock();
        //    var target = new ActorsController(context);

        //    target.PutActor(id, actor);
        //    var resultActor = context.Actors
        //        .FirstOrDefault(a => a.Id == id);

        //    Assert.That(resultActor, Has.Property("LastName").EqualTo(newLastName));
        //}

        //[Test]
        //public void CanDeleteActor()
        //{
        //    var id = 1;
        //    var context = GetFilmContextMock();
        //    var target = new ActorsController(context);

        //    target.DeleteActor(id);

        //    var actors = context.Actors.ToList();

        //    Assert.That(actors, Has.Exactly(0).Matches<Actor>(a => a.Id == id));
        //}

        //[Test]
        //public void CannotFindNonexistentActor()
        //{
        //    var context = GetFilmContextMock();
        //    var target = new ActorsController(context);
        //    var result = target.GetActor(44);

        //    Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        //}
    }
}