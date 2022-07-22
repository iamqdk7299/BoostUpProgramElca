using NHibernateCore;
using NHibernateExercises.Entities;
using System.Collections.Generic;

namespace NHibernateExercises.Repositories
{
    public interface IProjectRepository : IBaseRepository<ProjectEntity>
    {
        IList<ProjectEntity> LoadProjectByNumbers(List<int> numbers);
        int GetMaxNumber();
        IList<int> GetAllProjectNumbers();
    }
}
