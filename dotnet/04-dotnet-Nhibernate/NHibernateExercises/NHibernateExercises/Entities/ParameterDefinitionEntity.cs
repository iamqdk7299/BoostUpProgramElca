using NHibernateCore;
using System.Collections.Generic;

namespace NHibernateExercises.Entities
{
    public class ParameterDefinitionEntity : BaseEntity
    {
        public ParameterDefinitionEntity()
        {
            ParameterValues = new List<ParameterValueEntity>();
        }

        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
        public virtual IList<ParameterValueEntity> ParameterValues { get; set; }
    }
}
