using FilmsWebApi.Model;
using System.Collections.Generic;

namespace FilmsWebApi.Service
{
    public interface IFilmService
    {
        IEnumerable<Film> GetAllFilms();
        IEnumerable<Film> GetAllFilmsWithActors();
        Film GetFilm(int id);
        Film GetFilmWithActors(int id);
        void AddFilm(Film Film);
        void UpdateFilm(Film Film);
        void DeleteFilm(Film Film);
        bool FilmExists(int id);

    }
}
