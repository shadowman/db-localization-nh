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

        [ClassInitialize]
        public static void StaticTestsInitialization(TestContext context)
        {
            ISessionFactory factory = Fluently.Configure()
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

        public static void BuildDatabase(Configuration configuration)
        {
            if (File.Exists("sample.db"))
            {
                File.Delete("sample.db");
            }
            new SchemaExport(configuration).Create(true, true);
        }

        [TestMethod]
        public void LocalizedEntityCanBeLoadedUsingNHibernateAndSQLLiteDatabase()
        {
            Assert.Fail();
        }
    }
}
