using System;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Mapping.ByCode;
using NHibernateCore;
using NHibernateExercises.Entities;
using NHibernateExercises.Mappings;

namespace NHibernateExercises.Infrastructure
{
    public class ExerciseSessionFactory : SessionFactory
    {
        public ExerciseSessionFactory(string connection) : base(connection)
        {
        }

        public override void MappingTable(ModelMapper mapper)
        {
            base.MappingTable(mapper);
            mapper.AddMapping<ProjectMap>();
            mapper.AddMapping<ParameterDefinitionMap>();
            mapper.AddMapping<ParameterValueMap>();
        }

        public override void AuditingTable(FluentConfiguration enversConfig)
        {
            enversConfig.Audit<ProjectEntity>();
        }

    }
}
