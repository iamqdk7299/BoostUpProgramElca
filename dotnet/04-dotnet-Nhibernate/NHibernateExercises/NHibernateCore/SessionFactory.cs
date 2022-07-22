using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Event;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Event;
using NHibernateCore.Enver;

namespace NHibernateCore
{
    public class SessionFactory
    {
        protected virtual string DefaultSchema => String.Empty;
        protected readonly string _connection;
        public ValidatorEngine Engine;

        public SessionFactory(string connection)
        {
            _connection = connection;
        }

        public ISessionFactory GetSessionFactory()
        {
            var config = new Configuration();
            config.DataBaseIntegration(DataBaseIntegrationConfiguration);
            config.SetProperty(global::NHibernate.Cfg.Environment.WrapResultSets, Boolean.TrueString);
            config.SetProperty(global::NHibernate.Cfg.Environment.GenerateStatistics, Boolean.FalseString);
            config.SetProperty(global::NHibernate.Cfg.Environment.QueryStartupChecking, Boolean.TrueString);
            config.SetProperty(global::NHibernate.Cfg.Environment.PrepareSql, Boolean.TrueString);
            config.SetProperty(global::NHibernate.Cfg.Environment.DefaultSchema, DefaultSchema);
            config.SetProperty("apply_to_ddl", Boolean.TrueString);
            config.SetProperty("autoregister_listeners", Boolean.TrueString);

            var mapper = new ModelMapper();
            MappingTable(mapper);
            var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
            config.AddDeserializedMapping(mappings, null);

            AdditionalConfiguration(config);
            if (SupportsAuditing)
            {
                ConfigureEnvers(config);
            }

            Engine = new ValidatorEngine();
            config.Initialize(Engine);


            return config.BuildSessionFactory();
        }

        protected void DataBaseIntegrationConfiguration(IDbIntegrationConfigurationProperties db)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            db.Dialect<MsSql2012Dialect>();
            db.Driver<Sql2008ClientDriver>();
            db.HqlToSqlSubstitutions = "true=1;false=0";
            db.LogSqlInConsole = false;
            db.LogFormattedSql = true;
            db.ConnectionString = _connection;
            db.KeywordsAutoImport = Hbm2DDLKeyWords.None;
            db.BatchSize = 1;
            db.Timeout = 150;
        }
        protected virtual bool SupportsAuditing
        {
            get
            {
                return true;
            }
        }

        private void ConfigureEnvers(Configuration config)
        {
            var enversConf = new FluentConfiguration();
            ConfigurationKey.DefaultSchema.SetUserValue(config, "dbo");
            ConfigurationKey.DoNotAuditOptimisticLockingField.SetUserValue(config, false);
            ConfigurationKey.RevisionOnCollectionChange.SetUserValue(config, false);
            ConfigurationKey.AuditTablePrefix.SetUserValue(config, String.Empty);
            ConfigurationKey.AuditTableSuffix.SetUserValue(config, "_AUD");
            ConfigurationKey.StoreDataAtDelete.SetUserValue(config, true);

            enversConf.SetRevisionEntity<RevInfo>(e => e.Id, e => e.RevisionDate, new RevInfoListener());

            AuditingTable(enversConf);
            config.IntegrateWithEnvers(new AuditEventListener(), enversConf);
        }

        public virtual void AuditingTable(FluentConfiguration enversConfig)
        {

        }

        /// <summary>
        /// Can be overridden to add additional filters, interceptors, ...
        /// </summary>
        protected virtual void AdditionalConfiguration(Configuration config) { }

        public virtual void MappingTable(ModelMapper mapper)
        {
            if (SupportsAuditing)
            {
                mapper.AddMapping(new RevInfoMap("dbo"));
            }
        }
    }
}
