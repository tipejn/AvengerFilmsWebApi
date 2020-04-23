using FilmsWebApi.Data;
using FilmsWebApi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilmsWebApi.Service
{
    public class FilmService : IFilmService
    {
        private FilmContext _context;
        public FilmService(FilmContext context)
        {
            _context = context;
        }

        public void AddFilm(Film Film)
        {
            if (Film == null)
            {
                throw new ArgumentNullException();
            }

            EnsureHasActors(Film);
            _context.Films.Add(Film);
            _context.SaveChanges();
        }

        public void DeleteFilm(Film Film)
        {
            if (Film == null)
            {
                throw new ArgumentNullException();
            }
            _context.Films.Remove(Film);
            _context.SaveChanges();
        }

        public Film GetFilm(int id)
        {
            return _context.Films.Find(id);
        }

        public Film GetFilmWithFilms(int id)
        {
            return _context.Films
                .Include(f => f.ActorFilms)
                    .ThenInclude(a => a.Film)
                .FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Film> GetAllFilms()
        {
            return _context.Films.ToList();
        }

        public IEnumerable<Film> GetAllFilmsWithFilms()
        {
            return _context.Films
                .Include(f => f.ActorFilms)
                    .ThenInclude(a => a.Film)
                .ToList();
        }

        public void UpdateFilm(Film Film)
        {
            if (Film == null)
            {
                throw new ArgumentNullException();
            }

            EnsureHasActors(Film);
            _context.Entry(Film).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
        public bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
        private void EnsureHasActors(Film Film)
        {
            foreach (var actorFilm in Film.ActorFilms)
            {
                if (ActorExists(actorFilm.ActorId))
                {
                    actorFilm.Film = _context.Films.Single(f => f.Id == actorFilm.FilmId);
                }
            }
        }
    }
}
