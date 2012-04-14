using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Localization.NHibernate
{
    public class LocalizationInterceptor: EmptyInterceptor
    {
        private System.Globalization.CultureInfo cultureInfo;

        public LocalizationInterceptor(System.Globalization.CultureInfo cultureInfo)
        {
            // TODO: Complete member initialization
            this.cultureInfo = cultureInfo;
        }


        public object Culture { get; set; }
    }
}
