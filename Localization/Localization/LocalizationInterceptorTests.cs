using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Localization.NHibernate;
using System.Globalization;
using NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Type;
using System.Threading;
using Moq;
using Moq.Language;

namespace Localization
{
    [TestClass]
    public class LocalizationInterceptorTests
    {
        private Mock<ISession> session;
        private Mock<ISessionFactory> factory;
        private object[] Values = new object[2];
        private string[] Properties = new string[] { "Id", "Title" };
        private IType[] Types = new IType[] { null, null };
        private CultureInfo CULTURE_SPANISH = new CultureInfo("es-ES");
        private LocalizationInterceptor interceptor;

        [TestInitialize]
        public void TestsInitialization()
        {
            factory = new Mock<ISessionFactory>();
            session = new Mock<ISession>();
            factory.Setup(x => x.GetCurrentSession()).Returns(session.Object);

            interceptor = new LocalizationInterceptor(CULTURE_SPANISH, factory.Object);

        }

        [TestMethod]
        public void LocalizationInterceptorHasAConstructorThatReceivesTheCultureAndSessionsFactoryHeShouldWorkWith()
        {
            new LocalizationInterceptor((CultureInfo)null, (ISessionFactory)null);
        }

        [TestMethod]
        public void LocalizationInterceptorStoresCulturePassedToHimInTheConstructor()
        {
            LocalizationInterceptor interceptor = new LocalizationInterceptor(CULTURE_SPANISH, null);

            Assert.AreEqual(CULTURE_SPANISH, interceptor.Culture);
        }

        [TestMethod]
        public void LocalizationInterceptorUsesCurrentThreadCultureIfNoCultureIsPassed()
        {
            LocalizationInterceptor interceptor = new LocalizationInterceptor(null);

            Assert.AreEqual(Thread.CurrentThread.CurrentCulture, interceptor.Culture);
        }

        [TestMethod]
        public void LocalizationInterceptorStoresSessionsFactoryPassedToHimInTheConstructor()
        {
            LocalizationInterceptor interceptor = new LocalizationInterceptor(null, factory.Object);

            Assert.AreEqual(factory.Object, interceptor.Factory);
        }

        [TestMethod]
        public void LocalizationInterceptorOnLoadCallsOnFactoryToGetTheCurrentSession()
        {
            interceptor.OnLoad(
                new Article(),
                ArticlesMotherObject.LOCALIZED_ARTICLE_ID,
                Values,
                Properties,
                Types
            );

            factory.Verify(x=>x.GetCurrentSession(), Times.AtLeastOnce());
        }

        [TestMethod]
        public void LocalizationInterceptorOnLoadSetsTheArticlesTitlePropertyToItsLocalizedValueIfItExists()
        {
            session.Setup(
                x => x.Get<LocalizationEntry>(It.IsAny<LocalizationEntryId>())
            ).Returns(
                new LocalizationEntry()
                {
                    Message = ArticlesMotherObject.ES_TITLE
                }
            );
            
            interceptor.OnLoad(
                new Article(),
                ArticlesMotherObject.LOCALIZED_ARTICLE_ID,
                Values,
                Properties,
                Types
            );

            Assert.AreEqual(ArticlesMotherObject.ES_TITLE, Values[1]);
        }
    }
}
