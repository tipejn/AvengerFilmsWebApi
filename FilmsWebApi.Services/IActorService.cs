using FilmsWebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmsWebApi.Service
{
    public interface IActorService
    {
        IEnumerable<Actor> GetAllActors();
        IEnumerable<Actor> GetAllActorsWithFilms();
        Actor GetActor(int id);
        Actor GetActorWithFilms(int id);
        void AddActor(Actor actor);
        void UpdateActor(Actor actor);
        void DeleteActor(Actor actor);
        bool ActorExists(int id);

    }
}
