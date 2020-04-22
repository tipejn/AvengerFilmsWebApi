using FilmsWebApi.Model;
using Microsoft.EntityFrameworkCore;

namespace FilmsWebApi.Data
{
    public class FilmContext : DbContext
    {
        public FilmContext(DbContextOptions<FilmContext> options) : base(options) { }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Film> Films { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActorFilm>()
                .HasKey(a => new { a.ActorId, a.FilmId });
        }
    }
}
