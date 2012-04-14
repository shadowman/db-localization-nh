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


namespace Localization
{
    [TestClass]
    public class LocalizationIntegrationTests
    {
        private static long LOCALIZED_ARTICLE_ID = 1;
        private static string DEFAULT_TITLE = "Default title";

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
                    Id = LOCALIZED_ARTICLE_ID, 
                    Title = DEFAULT_TITLE 
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
                Article article = session.Get<Article>(LOCALIZED_ARTICLE_ID);
                Assert.IsNotNull(article);
            }
        }
        
        [TestMethod]
        public void ArticleTitleCanBeLoadedUsingNHibernateAndSQLLiteDatabase()
        {
            using (ISession session = Factory.OpenSession())
            {
                Article article = session.Get<Article>(LOCALIZED_ARTICLE_ID);
                Assert.AreEqual(DEFAULT_TITLE, article.Title);
            }
        }

        public static ISessionFactory Factory { get; set; }
    }
}
