using NHibernateCore;
using NHibernateExercises.Entities;

namespace NHibernateExercises.Repositories
{
    public interface IParameterDefinitionRepository : IBaseRepository<ParameterDefinitionEntity>
    {
        ParameterDefinitionEntity LoadParameter(int id);
    }
}
