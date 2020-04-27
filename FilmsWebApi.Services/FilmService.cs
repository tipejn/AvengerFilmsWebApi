using FilmsWebApi.Data;
using FilmsWebApi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmsWebApi.Service
{
    public class FilmService : IFilmService
    {
        private FilmContext _context;
        public FilmService(FilmContext context)
        {
            _context = context;
        }

        public void AddFilm(Film film)
        {
            if (film == null)
            {
                throw new ArgumentNullException();
            }

            EnsureHasActors(film);
            _context.Films.Add(film);
            _context.SaveChanges();
        }

        public void DeleteFilm(Film film)
        {
            if (film == null)
            {
                throw new ArgumentNullException();
            }
            _context.Films.Remove(film);
            _context.SaveChanges();
        }

        public Film GetFilm(int id)
        {
            return _context.Films.Find(id);
        }

        public Film GetFilmWithActors(int id)
        {
            return _context.Films
                .Include(f => f.ActorFilms)
                    .ThenInclude(a => a.Actor)
                .FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Film> GetAllFilms()
        {
            return _context.Films.ToList();
        }

        public IEnumerable<Film> GetAllFilmsWithActors()
        {
            return _context.Films
                .Include(f => f.ActorFilms)
                    .ThenInclude(a => a.Actor)
                .ToList();
        }

        public void UpdateFilm(Film film)
        {
            if (film == null)
            {
                throw new ArgumentNullException();
            }

            EnsureHasActors(film);
            _context.Entry(film).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public bool FilmExists(int id)
        {
            return _context.Films.Any(f => f.Id == id);
        }
        private void EnsureHasActors(Film film)
        {
            foreach (var actorFilm in film.ActorFilms)
            {
                actorFilm.Actor = _context.Actors.Single(a => a.Id == actorFilm.Actor.Id);
            }
        }
    }
}
