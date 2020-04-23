using FilmsWebApi.Model;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilmsWebApi.Tests
{
    class ContainsAllFilmTitlesConstraint : Constraint
    {
        private IEnumerable<Film> _source;
        public ContainsAllFilmTitlesConstraint(IEnumerable<Film> source)
        {
            _source = source;
        }
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var films = actual as IEnumerable<Film>;

            if (films is null)
            {
                return new ConstraintResult(this, actual, ConstraintStatus.Error);
            }

            if (_source.All(s => films.Any(f => f.Title == s.Title)))
            {
                return new ConstraintResult(this, actual, ConstraintStatus.Success);
            }

            return new ConstraintResult(this, actual, ConstraintStatus.Failure);
        }
    }
}
