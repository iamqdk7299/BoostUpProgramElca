using NHibernate.Criterion;
using NHibernateCore;
using NHibernateExercises.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NHibernateExercises.Repositories
{
    public class ProjectRepository : BaseRepository<ProjectEntity>, IProjectRepository
    {
        public IList<int> GetAllProjectNumbers()
        {
            var query = Session.QueryOver<ProjectEntity>().SelectList(v => v.Select(m => m.Number));
            return query.List<int>().Distinct().ToList();
        }

        public int GetMaxNumber()
        {
            var query = QueryOver.Of<ProjectEntity>().Select(Projections.Max("Number"));
            var result = query.GetExecutableQueryOver(Session).List<int?>();
            return result.SingleOrDefault() ?? 0;
        }

        public IList<ProjectEntity> LoadProjectByNumbers(List<int> numbers)
        {
            var query = QueryOver.Of<ProjectEntity>().Where(s => s.Number.IsIn(numbers));
            return FindByCriteria(query, 0);
        }
    }
}
