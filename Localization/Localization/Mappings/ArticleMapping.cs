using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace Localization.Mappings
{
    public class ArticleMapping: ClassMap<Article>
    {
        public ArticleMapping()
        {
            Id(x => x.Id);
            Map(x => x.Title);
        }
    }
}
