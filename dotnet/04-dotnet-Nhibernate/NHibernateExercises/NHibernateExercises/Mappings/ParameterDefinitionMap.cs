using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernateExercises.Entities;

namespace NHibernateExercises.Mappings
{
    public class ParameterDefinitionMap : ClassMapping<ParameterDefinitionEntity>
    {
        public ParameterDefinitionMap()
        {
            Schema("dbo");
            Lazy(true);
            Table("ParameterDefinition");
            Id(x => x.Id);
            Property(x => x.Name);
            Property(x => x.Type);
            Bag(x => x.ParameterValues, colmap =>
            {
                colmap.Key(x => x.Column(col => col.Name("ParameterDefinitionId")));
                colmap.Cascade(Cascade.All);
                colmap.Fetch(CollectionFetchMode.Subselect);
                colmap.Inverse(true);
            }, map => map.OneToMany());
            Version(e => e.RowVersion, versionMapper => versionMapper.Generated(VersionGeneration.Never));
        }
    }
}
