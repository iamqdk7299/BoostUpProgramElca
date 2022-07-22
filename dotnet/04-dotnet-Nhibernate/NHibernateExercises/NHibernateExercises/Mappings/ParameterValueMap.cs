using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernateExercises.Entities;

namespace NHibernateExercises.Mappings
{
    public class ParameterValueMap : ClassMapping<ParameterValueEntity>
    {
        public ParameterValueMap()
        {
            Schema("dbo");
            Lazy(true);
            Table("ParameterValue");
            Id(x => x.Id);
            Property(x => x.Value);
            Property(x => x.Year);
            ManyToOne(x => x.ParameterDefinition, map =>
            {
                map.Column("ParameterDefinitionId");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
            });
            Version(e => e.RowVersion, versionMapper => versionMapper.Generated(VersionGeneration.Never));
        }
    }
}
