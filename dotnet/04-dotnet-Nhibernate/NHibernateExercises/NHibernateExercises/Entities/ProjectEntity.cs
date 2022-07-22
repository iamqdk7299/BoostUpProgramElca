using NHibernateCore;
using System;
using NHibernate.Validator.Constraints;

namespace NHibernateExercises.Entities
{
    public class ProjectEntity : BaseEntity
    {
        public virtual int Number { get; set; }
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
    }
}
