using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using System.Globalization;
using System.Threading;

namespace Localization.NHibernate
{
    public class LocalizationInterceptor: EmptyInterceptor
    {
        public LocalizationInterceptor(ISessionFactory factory)
            :this(Thread.CurrentThread.CurrentCulture, factory)
        {
        }

        public LocalizationInterceptor(CultureInfo culture, ISessionFactory factory)
        {
            this.Culture = culture;
            this.Factory = factory;
        }

        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, global::NHibernate.Type.IType[] types)
        {
            using (ISession session = Factory.GetCurrentSession())
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    var message = session.Get<LocalizationEntry>(
                            new LocalizationEntryId
                            {
                                Culture = this.Culture.ThreeLetterISOLanguageName,
                                EntityId = id.ToString(),
                                Property = propertyNames[i],
                                Type = entity.GetType().FullName
                            }
                        );
                    if (message != null)
                    {
                        state[i] = message.Message;
                    }
                }
            }
            return base.OnLoad(entity, id, state, propertyNames, types);
        }

        public CultureInfo Culture { get; set; }

        public ISessionFactory Factory { get; set; }
    }
}
