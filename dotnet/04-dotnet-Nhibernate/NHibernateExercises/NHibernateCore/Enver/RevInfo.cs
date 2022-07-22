using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernateCore.Enver
{
    public class RevInfo
    {
        [RevisionNumber]
        public virtual long Id { get; set; }

        [RevisionTimestamp]
        public virtual DateTime RevisionDate { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            var defaultRevisionEntity1 = obj as RevInfo;
            var defaultRevisionEntity2 = defaultRevisionEntity1;
            if (Id != defaultRevisionEntity2?.Id)
            {
                return false;
            }
            return RevisionDate == defaultRevisionEntity2.RevisionDate;
        }

        public override int GetHashCode()
        {
            var num1 = 31 * ((int)(Id ^ (int)(Id >> 32)));
            var revisionDate = RevisionDate;
            var ticks = revisionDate.Ticks;
            revisionDate = RevisionDate;
            var num2 = (long)((ulong)revisionDate.Ticks >> 32);
            var num3 = (int)(ticks ^ num2);
            return num1 + num3;
        }

        public virtual void UpdateData(IList<object> objects)
        {
            Id = (long)objects[0];
            RevisionDate = (DateTime)objects[2];
        }
    }
}
