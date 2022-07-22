using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Envers;

namespace NHibernateCore.Enver
{
    public class RevInfoListener : IRevisionListener
    {
        public void NewRevision(object revisionEntity)
        {
            var revInfo = revisionEntity as RevInfo;
            if (revInfo == null)
            {
                return;
            }
        }
    }
}
