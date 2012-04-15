using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Localization.NHibernate
{
    public class LocalizationEntryId
    {
        public virtual string Culture { get; set; }
        public virtual string Type { get; set; }
        public virtual string Property { get; set; }
        public virtual string EntityId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                LocalizationEntryId other = obj as LocalizationEntryId;
                if (other != null)
                {
                    return this.Type      == other.Type &&
                            this.Property == other.Property &&
                            this.EntityId == other.EntityId &&
                            this.Culture  == other.Culture;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class LocalizationEntry
    {
        public virtual LocalizationEntryId Id { get; set; }
        public virtual string Message { get; set; }
    }
}
