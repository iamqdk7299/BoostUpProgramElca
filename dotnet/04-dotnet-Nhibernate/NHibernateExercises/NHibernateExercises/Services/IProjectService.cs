using System.Collections.Generic;
using NHibernateCore.Enver;
using NHibernateExercises.Entities;

namespace NHibernateExercises.Services
{
    public interface IProjectService
    {
        void ProcessProject(int number);
        void ProcessProjectInParallel(List<int> numbers);
        IList<int> GetAllProjectNumbers();
        void ImportProjects();

        List<ProjectEntity> GetAllProjectAudit();
    }
}
