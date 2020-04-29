using FilmsWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FilmsWebApi.Tests
{
    public class TestsSetup
    {
        private DbContextOptions<FilmContext> _options;

        [SetUp]
        public void Setup()
        {
            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            _options = new DbContextOptionsBuilder<FilmContext>()
                .UseInMemoryDatabase("FilmDb")
                .UseInternalServiceProvider(provider)
                .Options;

            using (var context = GetContext())
            {
                var helper = new TestFilmContextHelper(context);
                helper.SeedContext();
                helper.ResetEntities();
            }
        }

        private protected FilmContext GetContext()
        {
            return new FilmContext(_options);
        }

    }
}
