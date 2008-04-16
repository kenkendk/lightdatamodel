using System;
using System.Collections.Generic;
using System.Text;

namespace Datamodel.UnitTest
{
    partial class Project
    {
        public System.Guid Guid { get { return this.RelationManager.GetGuidForObject(this); } }
        public bool ExistsInDB { get { return this.RelationManager.ExistsInDb(this); } }

    }
}
