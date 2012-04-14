using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Localization.NHibernate;
using System.Globalization;
using NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Localization
{
    [TestClass]
    public class LocalizationInterceptorTests
    {
        [TestInitialize]
        public void TestsInitialization()
        {
        }

        [TestMethod]
        public void LocalizationInterceptorHasAConstructorThatReceivesTheCultureHeShouldWorkIn()
        {
            new LocalizationInterceptor(new CultureInfo("es-ES"));
        }

        [TestMethod]
        public void LocalizationInterceptorStoresCulturePassedToHimInTheConstructor()
        {
            CultureInfo culture = new CultureInfo("es-ES");

            LocalizationInterceptor interceptor = new LocalizationInterceptor(culture);

            Assert.AreEqual(culture, interceptor.Culture);
        }
    }
}
