namespace NHibernateCore
{
    public interface IBaseEntity
    {
        int Id { get; set; }
        int RowVersion { get; set; }
    }
}
