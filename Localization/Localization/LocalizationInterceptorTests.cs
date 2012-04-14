using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Localization.NHibernate;
using System.Globalization;
using NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Type;

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

        [TestMethod]
        public void LocalizationInterceptorOnLoadTriesToFindLocalizedValuesForTheProvidedEntityFromTheCurrentSessionIfPossible()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void LocalizationInterceptorOnLoadSetsTheArticlesTitlePropertyToItsLocalizedValue()
        {
            CultureInfo culture = new CultureInfo("es-ES");
            LocalizationInterceptor interceptor = new LocalizationInterceptor(culture);

            object[] values     = new object[2];
            string[] properties = new string[] { "Id", "Title" };
            IType[] types       = new IType[] { null, null };

            interceptor.OnLoad(
                new Article(),
                ArticlesMotherObject.LOCALIZED_ARTICLE_ID,
                values,
                properties,
                types
            );

            Assert.AreEqual(values[1], ArticlesMotherObject.ES_TITLE);
        }
    }
}
