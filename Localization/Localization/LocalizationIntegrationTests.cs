using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.IO;
using System.Threading;
using System.Globalization;


namespace Localization
{
    [TestClass]
    public class LocalizationIntegrationTests
    {
        

        [ClassInitialize]
        public static void StaticTestsInitialization(TestContext context)
        {
            InitializeDatabaseProvider();

            PopulateDatabaseWithReadOnlyTestData();
        }

        private static void InitializeDatabaseProvider()
        {
            Factory = Fluently.Configure()
                .Database(
                    SQLiteConfiguration.Standard
                        .UsingFile("sample.db")
                        .ShowSql()
                )
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<LocalizationIntegrationTests>())
                .ExposeConfiguration(BuildDatabase)
                .Cache(
                    x => x.UseSecondLevelCache()
                          .UseQueryCache()
                          .ProviderClass<global::NHibernate.Cache.HashtableCacheProvider>())
                .BuildSessionFactory();
        }

        private static void PopulateDatabaseWithReadOnlyTestData()
        {
            using (ISession session = Factory.OpenSession())
            {
                Article article = new Article() { 
                    Id = ArticlesMotherObject.LOCALIZED_ARTICLE_ID,
                    Title = ArticlesMotherObject.DEFAULT_TITLE 
                };
                session.Save(article);
            }
        }

        public static void BuildDatabase(Configuration configuration)
        {
            if (File.Exists("sample.db"))
            {
                File.Delete("sample.db");
            }
            new SchemaExport(configuration).Create(true, true);
        }

        [TestInitialize]
        public void TestInitialization()
        {
        }

        [TestMethod]
        public void ArticleCanBeLoadedUsingNHibernateAndSQLLiteDatabase()
        {
            using (ISession session = Factory.OpenSession())
            {
                Article article = session.Get<Article>(ArticlesMotherObject.LOCALIZED_ARTICLE_ID);
                Assert.IsNotNull(article);
            }
        }
        
        [TestMethod]
        public void ArticleTitleCanBeLoadedUsingNHibernateAndSQLLiteDatabase()
        {
            using (ISession session = Factory.OpenSession())
            {
                Article article = session.Get<Article>(ArticlesMotherObject.LOCALIZED_ARTICLE_ID);
                Assert.AreEqual(ArticlesMotherObject.DEFAULT_TITLE, article.Title);
            }
        }

        [TestMethod]
        public void DefaultVersionOfTheArticlesTitleWillBeLoadedUsingProvidedCultureIfAvailable()
        {
            CultureInfo culture = new CultureInfo("en-EN");
            using (ISession session = BuildLocalizedSession(culture))
            {
                Article article = session.Get<Article>(ArticlesMotherObject.LOCALIZED_ARTICLE_ID);
                Assert.AreEqual(ArticlesMotherObject.DEFAULT_TITLE, article.Title);
            }
        }

        [TestMethod]
        public void LocalizedVersionOfTheArticlesTitleWillBeLoadedUsingProvidedCultureIfAvailable()
        {
            CultureInfo culture = new CultureInfo("es-ES");
            using (ISession session = BuildLocalizedSession(culture))
            {
                Article article = session.Get<Article>(ArticlesMotherObject.LOCALIZED_ARTICLE_ID);
                Assert.AreEqual(ArticlesMotherObject.ES_TITLE, article.Title);
            }
        }

        private static ISession BuildLocalizedSession(CultureInfo culture)
        {
            return Factory.OpenSession();
        }

        public static ISessionFactory Factory { get; set; }
    }
}
