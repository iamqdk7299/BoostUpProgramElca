using NHibernateExercises.Entities;

namespace NHibernateExercises.Services
{
    public interface IParameterService
    {
        void CreateAllParameters();
        void DeleteAllParameters();
        void InsertParameter(ParameterDefinitionEntity parameter);
        ParameterDefinitionEntity LoadParameter(int id);
        void UpdateParameter(ParameterDefinitionEntity parameter);
    }
}
