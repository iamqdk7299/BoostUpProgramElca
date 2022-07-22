using NHibernate;
using NHibernateCore;
using NHibernateExercises.Entities;
using System.Linq;

namespace NHibernateExercises.Repositories
{
    public class ParameterDefinitionRepository : BaseRepository<ParameterDefinitionEntity>, IParameterDefinitionRepository
    {
        public ParameterDefinitionEntity LoadParameter(int id)
        {
            var query = Session.QueryOver<ParameterDefinitionEntity>()
                .Where(v => v.Id == id)
                .Fetch(SelectMode.Fetch, v => v.ParameterValues)
                .Future();
            return query.FirstOrDefault();
        }
    }
}
