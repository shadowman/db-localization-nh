using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Globalization;

namespace Localization.NHibernate
{
    public class LocalizationInterceptor: EmptyInterceptor
    {
        
        public LocalizationInterceptor(System.Globalization.CultureInfo cultureInfo)
        {
            this.Culture = cultureInfo;
        }


        public CultureInfo Culture { get; set; }
    }
}
