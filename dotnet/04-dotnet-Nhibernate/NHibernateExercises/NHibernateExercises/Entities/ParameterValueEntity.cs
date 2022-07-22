using NHibernateCore;

namespace NHibernateExercises.Entities
{
    public class ParameterValueEntity : BaseEntity
    {
        public virtual string? Value { get; set; }
        public virtual int? Year { get; set; }
        public virtual ParameterDefinitionEntity ParameterDefinition { get; set; }
    }
}
