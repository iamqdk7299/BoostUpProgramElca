using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernateCore.Enver
{
    public class RevInfoMap : ClassMapping<RevInfo>
    {

        public RevInfoMap(string schema)
        {
            Schema(schema);
            Lazy(true);
            Id(x => x.Id, map =>
            {
                map.Column("REV");
                map.Generator(Generators.Identity);
            });
            Property(x => x.RevisionDate, map =>
            {
                map.Column("REVTSTMP");
                map.Type(NHibernateUtil.UtcDateTime);
            });
        }
    }
}
