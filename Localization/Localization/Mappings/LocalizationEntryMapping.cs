using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Localization.NHibernate;
using FluentNHibernate.Mapping;

namespace Localization.Mappings
{
    public class LocalizationEntryMapping: ClassMap<LocalizationEntry>
    {
        public LocalizationEntryMapping()
        {
            CompositeId()
                .ComponentCompositeIdentifier(x => x.Id)
                .KeyProperty(x => x.Id.Culture)
                .KeyProperty(x => x.Id.EntityId)
                .KeyProperty(x => x.Id.Property)
                .KeyProperty(x => x.Id.Type);
            Map(x => x.Message);
        }
    }

}
