using FluentMigrator;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Attribute for a migration
    /// </summary>
    public partial class NopMigrationAttribute : MigrationAttribute
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            this(new MigrationConfig
            {
                DateTime = dateTime,
                TargetMigrationProcess = targetMigrationProcess
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="description">The migration description</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, string description, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            this(new MigrationConfig
            {
                DateTime=dateTime,
                Description = description,
                TargetMigrationProcess = targetMigrationProcess
            })
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="nopVersion">nopCommerce full version</param>
        /// <param name="updateMigrationType">The update migration type</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, string nopVersion, UpdateMigrationType updateMigrationType, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            this(new MigrationConfig
            {
                DateTime = dateTime,
                NopVersion = nopVersion,
                UpdateMigrationType = updateMigrationType,
                TargetMigrationProcess = targetMigrationProcess,
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="config">The migration configuration data</param>
        public NopMigrationAttribute(MigrationConfig config) : base(config.Version, config.Description)
        {
            MigrationConfig = config;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Target migration process
        /// </summary>
        public MigrationConfig MigrationConfig { get; set; }

        public virtual MigrationProcessType TargetMigrationProcess => MigrationConfig.TargetMigrationProcess;

        #endregion
    }
}
