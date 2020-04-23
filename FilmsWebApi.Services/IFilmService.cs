using FilmsWebApi.Model;
using System.Collections.Generic;

namespace FilmsWebApi.Service
{
    public interface IFilmService
    {
        IEnumerable<Film> GetAllFilms();
        IEnumerable<Film> GetAllFilmsWithFilms();
        Film GetFilm(int id);
        Film GetFilmWithFilms(int id);
        void AddFilm(Film Film);
        void UpdateFilm(Film Film);
        void DeleteFilm(Film Film);
        bool FilmExists(int id);

    }
}
