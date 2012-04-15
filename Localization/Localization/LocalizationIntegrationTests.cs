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
using Localization.NHibernate;


namespace Localization
{
    [TestClass]
    public class LocalizationIntegrationTests
    {
        private static CultureInfo DEFAULT_CULTURE = new CultureInfo("en-US");
        private static CultureInfo SPANISH_CULTURE = new CultureInfo("es-ES");
        private static CultureInfo NOT_FOUND_CULTURE = new CultureInfo("fr-FR");
        private static string LOCALIZED_PROPERTY_NAME = "Title";

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
                    Id      = ArticlesMotherObject.LOCALIZED_ARTICLE_ID,
                    Title   = ArticlesMotherObject.DEFAULT_TITLE 
                };
                session.Save(article);

                LocalizationEntry entry = new LocalizationEntry()
                {
                    Id = new LocalizationEntryId()
                    {
                        EntityId = ArticlesMotherObject.LOCALIZED_ARTICLE_ID.ToString(),
                        Culture  = SPANISH_CULTURE.ThreeLetterISOLanguageName,
                        Property = LOCALIZED_PROPERTY_NAME,
                        Type     = typeof(Article).ToString()
                    },
                    Message = ArticlesMotherObject.SPANISH_TITLE
                };
                session.Save(entry);
                session.Flush();
            }
        }

        public static void BuildDatabase(Configuration configuration)
        {
            configuration.SetProperty("current_session_context_class", "thread_static");
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
        public void LocalizationEntryCanBeLoadedUsingNHibernateAndSQLLiteDatabase()
        {
            using (ISession session = Factory.OpenSession())
            {
                LocalizationEntry entry = session.Get<LocalizationEntry>(
                    new LocalizationEntryId() {
                        Type     = typeof(Article).ToString(),
                        Culture  = SPANISH_CULTURE.ThreeLetterISOLanguageName,
                        EntityId = ArticlesMotherObject.LOCALIZED_ARTICLE_ID.ToString(),
                        Property = LOCALIZED_PROPERTY_NAME
                    }
                );
                Assert.IsNotNull(entry);
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
        public void EnlgishVersionOfTheArticlesTitleWillBeLoadedUsingProvidedCultureIfAvailable()
        {
            using (ISession session = BuildLocalizedSession(DEFAULT_CULTURE))
            {
                Article article = session.Get<Article>(ArticlesMotherObject.LOCALIZED_ARTICLE_ID);
                Assert.AreEqual(ArticlesMotherObject.DEFAULT_TITLE, article.Title);
            }
        }

        [TestMethod]
        public void SpanishVersionOfTheArticlesTitleWillBeLoadedUsingProvidedCultureIfAvailable()
        {
            using (ISession session = BuildLocalizedSession(SPANISH_CULTURE))
            {
                Article article = session.Get<Article>(ArticlesMotherObject.LOCALIZED_ARTICLE_ID);
                Assert.AreEqual(ArticlesMotherObject.SPANISH_TITLE, article.Title);
            }
        }

        [TestMethod]
        public void DefaultVersionOfTheArticlesTitleWillBeLoadedIfProvidedCultureIsNotAvailable()
        {
            
            using (ISession session = BuildLocalizedSession(NOT_FOUND_CULTURE))
            {
                Article article = session.Get<Article>(ArticlesMotherObject.LOCALIZED_ARTICLE_ID);
                Assert.AreEqual(ArticlesMotherObject.DEFAULT_TITLE, article.Title);
            }
        }

        private static ISession BuildLocalizedSession(CultureInfo culture)
        {
            return Factory.OpenSession(new LocalizationInterceptor(culture, Factory));
        }

        public static ISessionFactory Factory { get; set; }
    }
}
