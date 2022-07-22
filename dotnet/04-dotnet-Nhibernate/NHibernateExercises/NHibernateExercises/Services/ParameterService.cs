using NHibernateCore;
using NHibernateExercises.Entities;
using NHibernateExercises.Repositories;
using System;

namespace NHibernateExercises.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IParameterDefinitionRepository _parameterDefinitionRepository;
        private readonly IUnitOfWorkProvider _unitOfWorkProvider;

        public ParameterService(IParameterDefinitionRepository parameterDefinitionRepository, IUnitOfWorkProvider unitOfWorkProvider)
        {
            _parameterDefinitionRepository = parameterDefinitionRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public void CreateAllParameters()
        {
            using var uow = _unitOfWorkProvider.Provide();
            for (int i = 1; i <= 100000; i++)
            {
                var parameter = new Entities.ParameterDefinitionEntity
                {
                    Id = i,
                    Name = $"Parameter{i}",
                    Type = "Integer",
                };
                var parameterValue = new ParameterValueEntity
                {
                    Id = i,
                    Value = i.ToString(),
                    Year = DateTime.Now.Year,
                    ParameterDefinition = parameter
                };
                parameter.ParameterValues.Add(parameterValue);
                _parameterDefinitionRepository.Add(parameter);
            }
            uow.Complete();
        }

        public void DeleteAllParameters()
        {
            using var uow = _unitOfWorkProvider.Provide();
            var parameters = _parameterDefinitionRepository.GetAll();
            _parameterDefinitionRepository.Delete(parameters);
            uow.Complete();
        }

        public void InsertParameter(ParameterDefinitionEntity parameter)
        {
            using var uow = _unitOfWorkProvider.Provide();
            _parameterDefinitionRepository.Add(parameter);
            uow.Complete();
        }

        public ParameterDefinitionEntity LoadParameter(int id)
        {
            using var uow = _unitOfWorkProvider.Provide();
            var result = _parameterDefinitionRepository.LoadParameter(id);
            uow.Complete();
            return result;
        }

        public void UpdateParameter(ParameterDefinitionEntity parameter)
        {
            using var uow = _unitOfWorkProvider.Provide();
            _parameterDefinitionRepository.Merge(parameter);
            uow.Complete();
        }
    }
}
