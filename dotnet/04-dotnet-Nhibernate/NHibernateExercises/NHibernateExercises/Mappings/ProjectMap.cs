using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernateExercises.Entities;

namespace NHibernateExercises.Mappings
{
    public class ProjectMap : ClassMapping<ProjectEntity>
    {
        public ProjectMap()
        {
            Schema("dbo");
            Lazy(true);
            Table("Project");
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Number, map => map.NotNullable(true));
            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
            });
            Property(x => x.StartDate, map => map.NotNullable(true));
            Property(x => x.EndDate);
            Version(e => e.RowVersion, versionMapper => versionMapper.Generated(VersionGeneration.Never));
        }
    }
}
