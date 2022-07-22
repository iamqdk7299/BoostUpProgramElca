namespace NHibernateCore
{
    public class BaseEntity : IBaseEntity
    {
        public virtual int Id { get; set; }
        public virtual int RowVersion { get; set; }
    }
}
